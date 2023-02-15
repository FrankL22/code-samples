using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SprayAction : PlayerActionBase
{
    private bool mIsSpraying = false;
    private RaycastHit2D[] mRayCast = new RaycastHit2D[1];
    private int mLayerMask;

    public SprayAction()
    {
        mEvent = TouchEvent.Spray;
        mRequireUpdate = true;

        mLayerMask = LayerMask.GetMask("Wall");
    }

    public override void OnKeyDown(GameObject pObj = null, Vector3 pPosition = new Vector3())
    {
        mIsSpraying = true;
        PlayerControl.Current.ToggleSprayAnim(true);
        base.OnKeyDown();
    }

    public override void OnKeyUp(GameObject pObj = null, Vector3 pPosition = new Vector3())
    {
        mIsSpraying = false;
        PlayerControl.Current.ToggleSprayAnim(false);
        base.OnKeyUp();
    }

    public override void OnUpdate(float deltaTime)
    {
        if (!mIsSpraying) return;

        if (Inventory.Instance.UseItem(Utilities.RESOURCE_BLOCK_NAME,
            Utilities.SPRAY_COST_PER_SEC * deltaTime))
        {
            var pos = PlayerControl.Current.Position + PlayerControl.Current.Forward * Utilities.SPRAY_OFFSET;
            FloorManager.Current.OnSpray(pos, true);
            PlayerControl.Current.UpdateSprayAnimDirection(PlayerControl.Current.Forward);

            // 检测喷漆至墙体的情况
            var hit = Physics2D.LinecastNonAlloc(PlayerControl.Current.Position, pos, mRayCast, mLayerMask);
            if (hit > 0)
            {
                mRayCast[0].transform.GetComponent<Portal>()?.Activate();
            }
        }
        else
        {
            mIsSpraying = false;
            PlayerControl.Current.ToggleSprayAnim(false);
        }

        base.OnUpdate(deltaTime);
    }
}
