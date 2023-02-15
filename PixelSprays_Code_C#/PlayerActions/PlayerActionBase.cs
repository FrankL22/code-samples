using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerActionBase
{
    protected TouchEvent mEvent;
    /// <summary>
    /// 关联的按钮事件类型
    /// </summary>
    public TouchEvent Event
    {
        get { return mEvent; }
    }

    protected bool mRequireUpdate = false;
    /// <summary>
    /// 是否为需要每帧更新的持续行为
    /// </summary>
    public bool RequireUpdate
    {
        get { return mRequireUpdate; }
    }

    public virtual void OnKeyDown(GameObject pObj = null, Vector3 pPosition = new Vector3())
    {

    }

    public virtual void OnKeyUp(GameObject pObj = null, Vector3 pPosition = new Vector3())
    {

    }

    public virtual void OnUpdate(float deltaTime)
    {

    }
}
