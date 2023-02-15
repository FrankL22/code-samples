using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 处理角色移动和指令输入
/// </summary>
public class PlayerControl : MonoBehaviour
{
    private static PlayerControl mCurrent;
    public static PlayerControl Current
    {
        get { return mCurrent; }
    }

    #region 常量
    private const float HOLD_OFFSET = 1f;
    #endregion

    #region 变量
    // 移动
    /// <summary>角色移动方向（已标准化）</summary>
    private Vector3 mForward = Vector3.left;
    /// <summary>角色移动方向（已标准化）</summary>
    public Vector3 Forward
    {
        get { return mForward; }
    }
    /// <summary>角色位置</summary>
    public Vector3 Position
    {
        get { return transform.position; }
    }

    // 附加物体
    /// <summary>当前抓住的物体，没有时重置为null</summary>
    public GameObject mHoldObj;
    /// <summary>当前发生碰撞的物体，没有时重置为null</summary>
    public GameObject mCollideObj;
    private Transform mGun;
    private Transform mArm;
    private Animator mWheelAnim;
    private GameObject mSprayAnim;
    private SpriteRenderer mRenderer;
    private GameObject mBubble;

    // 特殊状态
    private bool mIsDiving = false;
    private bool mIsMoving = false;
    private bool mIsOnSpray = false;

    // 角色行为集
    private Dictionary<TouchEvent, PlayerActionBase> mActions = new Dictionary<TouchEvent, PlayerActionBase>();
    #endregion


    private void Awake()
    {
        mCurrent = this;
        mGun = transform.Find("Gun");
        mArm = transform.Find("Arm");
        mWheelAnim = transform.Find("Wheels").GetComponent<Animator>();
        mWheelAnim.speed = 0;
        mSprayAnim = transform.Find("SprayAnim").gameObject;
        mBubble = transform.Find("Bubble").gameObject;
        mBubble.GetComponent<Animator>().speed = 0;
        mRenderer = GetComponent<SpriteRenderer>();

        InitActions();
    }

    private void Start()
    {
        CameraFollow.Current.Init(this);
        GameManager.Instance.StartGame((int)(Utilities.WORLD_WIDTH / Utilities.PIXELS_PER_UNIT),
            (int)(Utilities.WORLD_HEIGHT / Utilities.PIXELS_PER_UNIT), Utilities.PIXELS_PER_UNIT);
        GameManager.IsPlaying = false;
        HUD.Current.StartGameDelayed(Utilities.GAME_START_DELAY);
    }

    private void OnDestroy()
    {
        if (mCurrent == this) mCurrent = null;
    }

    private void FixedUpdate()
    {
        if (!GameManager.IsPlaying) return;

        // 更新要求更新的行为
        foreach (var action in mActions)
        {
            if (action.Value.RequireUpdate)
            {
                action.Value.OnUpdate(Time.fixedDeltaTime);
            }
        }

        // 更新枪的朝向
        mGun.rotation = Quaternion.LookRotation(Vector3.forward, mForward);

        // 更新潜行按钮可用提示
        if (FloorManager.Current.CheckIsOnSpray(transform.position, true))
        {
            mIsOnSpray = true;
            TouchControls.Current.ToggleButtonUsableHint("Dive", true);
        }
        else
        {
            mIsOnSpray = false;
            TouchControls.Current.ToggleButtonUsableHint("Dive", false);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<Throwable>() != null)
        {
            TouchControls.Current.ToggleButtonUsableHint("Grab", true);
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (mCollideObj == null) mCollideObj = collision.gameObject;

        //var touch = TouchControls.Current;
        //touch.SetButton("Dive", false);
        //touch.SetButton("Destruct", true);
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (mCollideObj == collision.gameObject) mCollideObj = null;

        if (mHoldObj == null)
        {
            TouchControls.Current.ToggleButtonUsableHint("Grab", false);
        }
        // var touch = TouchControls.Current;
        //touch.SetButton("Destruct", false);
        //touch.SetButton("Dive", true);
    }

    #region Public方法
    public void InitActions()
    {
        RegisterAction(new GrabAction());
        //RegisterAction(new DestructAction());
        RegisterAction(new ShootAction());
        //RegisterAction(new BuildAction());
        RegisterAction(new SprayAction());
        RegisterAction(new DiveAction());
    }

    public void RegisterAction(PlayerActionBase pAction)
    {
        mActions.Add(pAction.Event, pAction);
    }

    /// <summary>
    /// 通知PlayerControl处理移动事件
    /// </summary>
    /// <param name="pHorizontal">水平读数</param>
    /// <param name="pVertical">垂直读数</param>
    public void TakeTouchInput(float pHorizontal, float pVertical)
    {
        if (!GameManager.IsPlaying) return;

        // 移动
        if (pHorizontal != 0 || pVertical != 0)
        {
            mForward = new Vector3(pHorizontal, pVertical, 0);
            var move = mForward * Utilities.PLAYER_MOVE_SPEED * Time.fixedDeltaTime;
            if (mIsDiving) move *= Utilities.SPRAY_MOVE_BOOST;
            transform.position += move;
        }
        mForward = mForward.normalized;

        UpdateSpriteFacing();
        // 更新轮子动画
        if (!mIsMoving && (pHorizontal > 0 || pVertical > 0))
        {
            mIsMoving = true;
            mWheelAnim.speed = 1;
            mBubble.GetComponent<Animator>().speed = 1;
        }
        else if (mIsMoving && (pHorizontal == 0 && pVertical == 0))
        {
            mIsMoving = false;
            mWheelAnim.speed = 0;
            mBubble.GetComponent<Animator>().speed = 0;
        }

        // 更新手持物体的位置
        if (mHoldObj != null)
        {
            mHoldObj.transform.position = transform.position + mForward * HOLD_OFFSET;
            mArm.rotation = Quaternion.LookRotation(Vector3.forward, -1 * mForward);
        }
    }

    /// <summary>
    /// 通知PlayerControl处理按键事件
    /// </summary>
    /// <param name="pEvent">事件类型枚举</param>
    public void TakeTouchInput(TouchEvent pEvent, TouchType pType)
    {
        if (!GameManager.IsPlaying) return;
        if (!mActions.ContainsKey(pEvent)) return;

        switch (pType)
        {
            case TouchType.Down:
                mActions[pEvent].OnKeyDown(mHoldObj, transform.position);
                break;
            case TouchType.Up:
                mActions[pEvent].OnKeyUp(mHoldObj, transform.position);
                break;
        }
    }

    /// <summary>
    /// 捡起当前碰撞的物体
    /// </summary>
    public void OnPickupObject()
    {
        mHoldObj = mCollideObj;
        mCollideObj = null;
        mHoldObj.GetComponent<BoxCollider2D>().enabled = false;
        mGun.gameObject.SetActive(false);
        mArm.gameObject.SetActive(true);

        TouchControls.Current.SetButton("Grab", "Grab", "Throw");
        TouchControls.Current.ToggleButtonUsableHint("Grab", true);
    }

    /// <summary>
    /// 丢掉当前持有的物体
    /// </summary>
    /// <param name="pThrowSpeed">丢出的速度</param>
    public void OnLoseObject(float pThrowSpeed)
    {
        var obj = mHoldObj;
        mHoldObj = null;
        ThrowObject(obj, pThrowSpeed);
        mGun.gameObject.SetActive(true);
        mArm.gameObject.SetActive(false);

        TouchControls.Current.SetButton("Grab", "Throw", "Grab");
        TouchControls.Current.ToggleButtonUsableHint("Grab", true);
    }

    public void ToggleDiving(bool pIsDiving)
    {
        mIsDiving = pIsDiving;
        GetComponent<Collider2D>().enabled = !pIsDiving;
        mBubble.SetActive(pIsDiving);
    }

    public void ToggleSprayAnim(bool pPlay)
    {
        mSprayAnim.SetActive(pPlay);
    }
    public void UpdateSprayAnimDirection(Vector3 pDirection)
    {
        mSprayAnim.transform.rotation = Quaternion.LookRotation(Vector3.forward, pDirection);
    }

    public void UpdateResourceBar(float pAmount)
    {
        var percentage = pAmount / Utilities.RESOURCE_DISPLAY_MAX;
        percentage = Mathf.Clamp(percentage, 0, 1);
    }
    #endregion

    #region Private方法
    private void ThrowObject(GameObject pObj, float pSpeed)
    {
        var throwable = pObj.GetComponent<Throwable>();
        if (throwable != null)
        {
            throwable.Launch(mForward * pSpeed, Throwable.THROW_DECAY_TIME, true);
            pObj.GetComponent<BoxCollider2D>().enabled = true;
        }
    }

    private void UpdateSpriteFacing()
    {
        if (mForward.x > 0) // 向右移动
        {
            mRenderer.sprite = PrefabManager.Instance.GetSprite("PlayerFront");
            transform.localScale = new Vector3(-2, 2, 1);
        }
        else // 向左移动
        {
            mRenderer.sprite = PrefabManager.Instance.GetSprite("PlayerFront");
            mRenderer.flipX = false;
            transform.localScale = new Vector3(2, 2, 1);
        }
    }
    #endregion
}
