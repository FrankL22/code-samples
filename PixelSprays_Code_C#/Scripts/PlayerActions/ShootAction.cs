using UnityEngine;

public class ShootAction : PlayerActionBase
{
    public ShootAction()
    {
        mEvent = TouchEvent.Shoot;
    }

    public override void OnKeyDown(GameObject pObj = null, Vector3 pPosition = new Vector3())
    {
        if (Inventory.Instance.UseItem(Utilities.RESOURCE_BLOCK_NAME, 1))
        {
            var blockPrefab = PrefabManager.Instance.GetPrefab(Utilities.RESOURCE_BLOCK_NAME);
            Vector3 launchPos = pPosition;
            Vector3 launchDirection = PlayerControl.Current.Forward;
            var block = GameObject.Instantiate(blockPrefab, launchPos, Quaternion.identity);
            block.GetComponent<PixelBlock>().Launch(
                launchDirection * Utilities.LAUNCH_SPEED, -1f, true, true);
        }
        base.OnKeyDown();
    }

    public override void OnKeyUp(GameObject pObj = null, Vector3 pPosition = new Vector3())
    {
        base.OnKeyUp();
    }

    public override void OnUpdate(float deltaTime)
    {
        base.OnUpdate(deltaTime);
    }
}
