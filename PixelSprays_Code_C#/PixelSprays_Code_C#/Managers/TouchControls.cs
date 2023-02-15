using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TouchControls : MonoBehaviour
{
    private static TouchControls mCurrent;
    public static TouchControls Current
    {
        get { return mCurrent; }
    }

    // 虚拟摇杆输入值比较低，提供补正
    private const float JOYSTICK_INPUT_FACTOR = 3;

    // 按钮颜色
    private readonly Color mActiveButtonColor = new Color(1, 1, 1, .8f);
    private readonly Color mInactiveButtonColor = new Color(.5f, .5f, .5f, .8f);

    private Joystick mJoystick;
    private Camera mCamera;

    private bool mPortalMode = false;

    private void Awake()
    {
        mCurrent = this;
        mJoystick = GetComponentInChildren<Joystick>();
        mCamera = Camera.main;
    }

    private void OnDestroy()
    {
        if (mCurrent == this) mCurrent = null;   
    }

    private void FixedUpdate()
    {
        if (!mPortalMode)
        {
            PlayerControl.Current.TakeTouchInput(
                mJoystick.Horizontal * JOYSTICK_INPUT_FACTOR, mJoystick.Vertical * JOYSTICK_INPUT_FACTOR);
        }
        else
        {
            if (Input.touchCount > 0)
            {
                var touch = Input.touches[0];
                switch (touch.phase)
                {
                    case TouchPhase.Began:
                        var touchPos = mCamera.ScreenToWorldPoint(touch.position);
                        var portal = GameManager.Instance.GetPortal(touchPos);
                        if (portal != null) OnSelectedPortal(portal);
                        break;
                    case TouchPhase.Moved:
                        var drag = touch.deltaPosition;
                        CameraFollow.Current.TouchDragCamera(drag);
                        break;
                }

                
            }
        }
    }
    
    /// <summary>
    /// 设置按钮是否显示
    /// </summary>
    public void SetButton(string pName, bool pActive)
    {
        transform.Find(pName).gameObject.SetActive(pActive);
    }

    /// <summary>
    /// 设置按钮图片
    /// </summary>
    public void SetButton(string pName, string pOld, string pImage)
    {
        var button = transform.Find(pName);
        button.Find(pOld).gameObject.SetActive(false);
        button.Find(pImage).gameObject.SetActive(true);
    }

    public void ToggleButtonUsableHint(string pName, bool pUsable)
    {
        var image = transform.Find(pName)?.GetComponent<Image>();
        if (image == null) return;
        if (pUsable)
        {
            image.color = mActiveButtonColor;
        }
        else
        {
            image.color = mInactiveButtonColor;
        }
    }

    public void EnterPortalMode()
    {
        mPortalMode = true;
        GetComponent<Canvas>().enabled = false;
    }

    public void ExitPortalMode()
    {
        mPortalMode = false;
        GetComponent<Canvas>().enabled = true;
    }

    #region 按钮事件
    public void OnButtonDown(int button)
    {
        PlayerControl.Current.TakeTouchInput((TouchEvent)button, TouchType.Down);
    }

    public void OnButtonUp(int button)
    {
        PlayerControl.Current.TakeTouchInput((TouchEvent)button, TouchType.Up);
    }
    #endregion

    private void OnSelectedPortal(Portal pPortal)
    {
        var tpToPos = pPortal.ExitPosition;
        pPortal.Deactivate();
        PlayerControl.Current.transform.position = tpToPos;

        GameManager.Instance.EnterNormalView();
    }
}

public enum TouchEvent
{
    Shoot = 1,
    Destruct = 2,
    Grab = 3,
    Build = 4,
    Spray = 5,
    Dive = 6
}
public enum TouchType
{
    Down = 1,
    Up = 2
}
