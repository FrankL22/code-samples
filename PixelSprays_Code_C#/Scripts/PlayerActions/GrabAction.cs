using UnityEngine;

/// <summary>
/// 拆除物体}
/// </summary>
public class GrabAction : PlayerActionBase
{
    private bool mIsGrabbing = false;

    public GrabAction()
    {
        mEvent = TouchEvent.Grab;
        mRequireUpdate = true;
    }

    public override void OnKeyDown(GameObject pObj = null, Vector3 pPosition = new Vector3())
    {
        if (pObj != null) // 已有手持物体，丢出
        {
            PlayerControl.Current.OnLoseObject(Throwable.THROW_SPEED);
        }
        else // 没有手持物体，开始检测
        {
            mIsGrabbing = true;
        }
        base.OnKeyDown();
    }

    public override void OnKeyUp(GameObject pObj = null, Vector3 pPosition = new Vector3())
    {
        mIsGrabbing = false;
        base.OnKeyUp();
    }

    public override void OnUpdate(float deltaTime)
    {
        if (!mIsGrabbing) return;
        if (PlayerControl.Current.mCollideObj == null) return;

        var throwable = PlayerControl.Current.mCollideObj.GetComponent<Throwable>();
        if (throwable == null) return;

        mIsGrabbing = false;
        PlayerControl.Current.OnPickupObject();

        base.OnUpdate(deltaTime);
    }
}
