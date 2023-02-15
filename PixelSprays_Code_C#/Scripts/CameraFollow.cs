using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    private static CameraFollow mCurrent;
    public static CameraFollow Current
    {
        get { return mCurrent; }
    }

    private readonly Vector3 mOffset = new Vector3(0, 0, -10);
    private Transform mPlayer;
    private Camera mCamera;
    private Transform mParent;
    private GameObject mWarpAnim;
    private Vector2 mCameraSize;


    private Transform mDragHints;
    private GameObject mLeft;
    private GameObject mRight;
    private GameObject mUp;
    private GameObject mDown;

    private bool mPortalView = false;
    private float mZoomTimer = 0;

    private void Awake()
    {
        mCurrent = this;
        mCamera = GetComponent<Camera>();
        mParent = transform.parent;
        mWarpAnim = transform.Find("WarpAnimObj").gameObject;

        mDragHints = transform.Find("Hints");
        mLeft = mDragHints.Find("Left").gameObject;
        mRight = mDragHints.Find("Right").gameObject;
        mUp = mDragHints.Find("Up").gameObject;
        mDown = mDragHints.Find("Down").gameObject;

        UpdateCameraSize();
    }

    private void OnDestroy()
    {
        if (mCurrent == this) mCurrent = null;
    }

    public void Init(PlayerControl pPlayer)
    {
        mPlayer = pPlayer.gameObject.transform;
    }

    public void EnterPortalView()
    {
        mPortalView = true;
        mZoomTimer = Utilities.ZOOM_PORTAL_TIME;

        // 把玩家角色移出地图
        PlayerControl.Current.transform.position = new Vector3(-200, 0, 0);

        // 开始播放特效
        mWarpAnim.SetActive(true);

        StartCoroutine(DelayForZoom());
    }

    public void ExitPortalView()
    {
        mPortalView = false;
        mZoomTimer = 0;
        mCamera.orthographicSize = Utilities.CAMERA_SIZE_ZOOMIN;
        UpdateCameraSize();
        mDragHints.gameObject.SetActive(false);
        GameManager.Instance.ToggleShowPortalArrows(false);
        HUD.Current.ToggleShowHUD(true);
        TouchControls.Current?.ExitPortalMode();
    }

    public void TouchDragCamera(Vector3 pDrag)
    {
        mCamera.transform.position -= pDrag * Utilities.CAMERA_DRAG_SPEED;
        ClampCameraPosition();
    }

    public void CameraShake()
    {
        StartCoroutine(Shake());
    }
    private IEnumerator Shake()
    {
        var timer = Utilities.HIT_SHAKE_TIME;
        while (timer > 0)
        {
            timer -= Utilities.HIT_SHAKE_INTERVAL;
            if (mParent.position.x > 0)
            {
                mParent.position -= Vector3.right * Random.Range(0, Utilities.HIT_SHAKE_RANGE);
            }
            else
            {
                mParent.position += Vector3.right * Random.Range(0, Utilities.HIT_SHAKE_RANGE);
            }

            if (mParent.position.y > 0)
            {
                mParent.position -= Vector3.up * Random.Range(0, Utilities.HIT_SHAKE_RANGE);
            }
            else
            {
                mParent.position += Vector3.up * Random.Range(0, Utilities.HIT_SHAKE_RANGE);
            }
            yield return new WaitForSeconds(Utilities.HIT_SHAKE_INTERVAL);
        }
        mParent.position = Vector3.zero;
    }

    private IEnumerator DelayForZoom()
    {
        HUD.Current.ToggleShowHUD(false);
        yield return new WaitForSeconds(Utilities.ZOOM_PORTAL_TIME);

        TouchControls.Current?.EnterPortalMode();
        GameManager.Instance.ToggleShowPortalArrows(true);
        UpdateHintsPos();
        mDragHints.gameObject.SetActive(true);
        ClampCameraPosition();
    }

    private void UpdateCameraSize()
    {
        mCameraSize.y = mCamera.orthographicSize;
        mCameraSize.x = mCameraSize.y * mCamera.aspect;
        mWarpAnim.transform.localScale = new Vector3(mCameraSize.x * 2, mCameraSize.y * 2, 1);

        float ratio = mCameraSize.y / Utilities.CAMERA_SIZE_ZOOMIN;
    }

    private void UpdateHintsPos()
    {
        UpdateCameraSize();
        mLeft.transform.localPosition = new Vector3(-mCameraSize.x, 0, 10);
        mRight.transform.localPosition = new Vector3(mCameraSize.x, 0, 10);
        mUp.transform.localPosition = new Vector3(0, mCameraSize.y, 10);
        mDown.transform.localPosition = new Vector3(0, -mCameraSize.y, 10);
    }

    private void ClampCameraPosition()
    {
        var pos = transform.position;

        mLeft.SetActive(true);
        mRight.SetActive(true);
        mUp.SetActive(true);
        mDown.SetActive(true);

        var camWidth = Utilities.WORLD_WIDTH / 2 - mCameraSize.x + Utilities.CAMERA_OFFSET;
        var camHeight = Utilities.WORLD_HEIGHT / 2 - mCameraSize.y + Utilities.CAMERA_OFFSET;

        if (pos.x <= -camWidth)
        {
            pos.x = -camWidth;
            mLeft.SetActive(false);
        }
        else if (pos.x >= camWidth)
        {
            pos.x = camWidth;
            mRight.SetActive(false);
        }

        if (pos.y <= -camHeight)
        {
            pos.y = -camHeight;
            mDown.SetActive(false);
        }
        else if (pos.y >= camHeight)
        {
            pos.y = camHeight;
            mUp.SetActive(false);
        }

        mCamera.transform.position = pos;
    }

    private void FixedUpdate()
    {
        if (mPlayer == null) return;

        if (!mPortalView)
        {
            transform.position = mPlayer.position + mOffset;
            ClampCameraPosition();
        }
        else
        {
            if (mZoomTimer > 0)
            {
                var ratio = Time.fixedDeltaTime / mZoomTimer;
                if (ratio > 1) ratio = 1;
                mZoomTimer -= Time.fixedDeltaTime;

                mCamera.orthographicSize += (Utilities.CAMERA_SIZE_ZOOMOUT - mCamera.orthographicSize) * ratio;

                UpdateCameraSize();
                ClampCameraPosition();
            }
        }
    }
}
