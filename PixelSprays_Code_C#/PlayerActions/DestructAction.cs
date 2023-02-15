using UnityEngine;

/// <summary>
/// ²ð³ýÎïÌå}
/// </summary>
public class DestructAction : PlayerActionBase
{
    private bool mIsDestructing = false;

    public DestructAction()
    {
        mEvent = TouchEvent.Destruct;
        mRequireUpdate = true;
    }

    public override void OnKeyDown(GameObject pObj = null, Vector3 pPosition = new Vector3())
    {
        mIsDestructing = true;
        base.OnKeyDown();
    }

    public override void OnKeyUp(GameObject pObj = null, Vector3 pPosition = new Vector3())
    {
        mIsDestructing = false;
        base.OnKeyUp();
        
    }

    public override void OnUpdate(float deltaTime)
    {
        if (!mIsDestructing) return;
        if (PlayerControl.Current.mCollideObj == null) return;

        var destructible = PlayerControl.Current.mCollideObj.GetComponent<Destructible>();
        if (destructible == null) return;
        PlayerControl.Current.mCollideObj = null;
        destructible.Destruct();
        mIsDestructing = false;

        base.OnUpdate(deltaTime);
    }
}
