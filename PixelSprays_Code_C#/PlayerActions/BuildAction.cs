using UnityEngine;

public class BuildAction : PlayerActionBase
{
    private const float BUILD_OFFSET = 3f;

    public BuildAction()
    {
        mEvent = TouchEvent.Build;
    }

    public override void OnKeyDown(GameObject pObj = null, Vector3 pPosition = new Vector3())
    {
        if (Inventory.Instance.UseItem(Utilities.RESOURCE_BLOCK_NAME, Utilities.BUILD_COST))
        {
            var player = PlayerControl.Current;
            Vector3 buildPos = pPosition + player.Forward * BUILD_OFFSET;
            var obj = PrefabManager.Instance.GetPrefab(Utilities.BUILD_BASIC_NAME);
            if (obj != null)
            {
                var buildObj = GameObject.Instantiate(obj,
                    Utilities.SnapToGrid(buildPos, pPosition), Quaternion.identity);
            }
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
