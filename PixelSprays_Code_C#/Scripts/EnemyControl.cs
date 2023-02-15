using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 敌人行为状态机
/// </summary>
public class EnemyControl : MonoBehaviour
{

    #region 常量
    private const float MOVE_SPEED = 10f;
    #endregion

    #region 变量
    // 移动
    /// <summary>角色移动方向（不会标准化）</summary>
    private Vector3 mForward = Vector3.left;
    public Vector3 Forward
    {
        get { return mForward; }
    }
    public Vector3 Position
    {
        get { return transform.position; }
    }

    // 附加物体
    /// <summary>当前发生碰撞的物体，没有时重置为null</summary>
    public GameObject mCollideObj;
    private Transform mGun;
    private SpriteRenderer mRenderer;

    // 敌人状态集
    private Dictionary<Type, EnemyStateBase> mStates = new Dictionary<Type, EnemyStateBase>();
    private EnemyStateBase mCurrState = null;
    private readonly Type DEFAULT_STATE_TYPE = typeof(EnemyPatrolState);

    private Damagable mDamagable;
    private bool mIsHit = false;
    #endregion


    private void Awake()
    {
        mDamagable = GetComponent<Damagable>();
        mGun = transform.Find("Gun");
        mRenderer = GetComponent<SpriteRenderer>();
        InitStates();
    }

    private void Start()
    {
        HUD.Current?.RegisterEnemy(transform);
    }

    private void OnDestroy()
    {
        HUD.Current?.RemoveEnemy(transform);
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(Position, Position + mForward * 20);
    }
#endif

    private void FixedUpdate()
    {
        if (!GameManager.IsPlaying) return;

        if (mDamagable.OnCooldown && !mIsHit)
        {
            GoToState(typeof(EnemyHitState));
            mIsHit = true;
        }
        mCurrState?.OnUpdate(Time.fixedDeltaTime);

        // 更新枪的朝向
        var gunLookat = PlayerControl.Current.Position - transform.position;
        mGun.position = transform.position + gunLookat.normalized * mGun.localScale.y;
        mGun.rotation = Quaternion.LookRotation(Vector3.forward, gunLookat);

        if (Mathf.Sign(gunLookat.x) == Mathf.Sign(transform.localScale.x))
        {
            mGun.localScale = new Vector3(-.8f, .8f, 1);
        }
        else
        {
            mGun.localScale = new Vector3(.8f, .8f, 1);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (mCollideObj == null) mCollideObj = collision.gameObject;

        mCurrState?.OnCollide(collision);
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (mCollideObj == collision.gameObject) mCollideObj = null;
    }

    #region Public方法
    public void InitStates()
    {
        RegisterState(new EnemyPatrolState());
        RegisterState(new EnemyChaseState());
        RegisterState(new EnemyAttackState());
        RegisterState(new EnemyHitState());

        GoToState(typeof(EnemyPatrolState));
    }

    public void RegisterState(EnemyStateBase pState)
    {
        mStates.Add(pState.GetType(), pState);
    }

    public void GoToState(Type pStateType)
    {
        mCurrState?.OnLeaveState(this);
        mCurrState = pStateType == null ? mStates[DEFAULT_STATE_TYPE] : mStates[pStateType];
        mCurrState?.EnterState(this);
        mIsHit = false;
    }

    /// <summary>
    /// 移动角色
    /// </summary>
    /// <param name="pMove">移动方向和距离</param>
    public void Move(Vector3 pMove)
    {
        if (FloorManager.Current.CheckIsOnSpray(transform.position, true))
        {
            pMove /= Utilities.SPRAY_MOVE_BOOST;
        }
        mForward = pMove;
        transform.position += pMove;

        UpdateSpriteFacing();
    }
    #endregion

    #region Private方法
    private void UpdateSpriteFacing()
    {
        if (mForward.x > 0) // 向右移动
        {
            mRenderer.sprite = PrefabManager.Instance.GetSprite("EnemyFront");
            transform.localScale = new Vector3(-2, 2, 1);
        }
        else // 向左移动
        {
            mRenderer.sprite = PrefabManager.Instance.GetSprite("EnemyFront");
            mRenderer.flipX = false;
            transform.localScale = new Vector3(2, 2, 1);
        }
    }
    #endregion
}
