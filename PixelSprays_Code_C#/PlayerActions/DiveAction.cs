using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiveAction : PlayerActionBase
{
    private bool mIsDiving = false;
    private bool mIsHolding = false;

    public DiveAction()
    {
        mEvent = TouchEvent.Dive;
        mRequireUpdate = true;
    }

    public override void OnKeyDown(GameObject pObj = null, Vector3 pPosition = new Vector3())
    {
        mIsHolding = true;
        base.OnKeyDown();
    }

    public override void OnKeyUp(GameObject pObj = null, Vector3 pPosition = new Vector3())
    {
        mIsDiving = false;
        mIsHolding = false;
        PlayerControl.Current.ToggleDiving(false);
        base.OnKeyUp();
    }

    public override void OnUpdate(float deltaTime)
    {
        if (!mIsHolding) return;

        if (FloorManager.Current.CheckIsOnSpray(PlayerControl.Current.Position, true))
        {
            if (!mIsDiving)
            {
                mIsDiving = true;
                PlayerControl.Current.ToggleDiving(true);
            }
        }
        else
        {
            if (mIsDiving)
            {
                mIsDiving = false;
                PlayerControl.Current.ToggleDiving(false);
            }
        }

        base.OnUpdate(deltaTime);
    }
}
