using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerActionBase
{
    protected TouchEvent mEvent;
    /// <summary>
    /// �����İ�ť�¼�����
    /// </summary>
    public TouchEvent Event
    {
        get { return mEvent; }
    }

    protected bool mRequireUpdate = false;
    /// <summary>
    /// �Ƿ�Ϊ��Ҫÿ֡���µĳ�����Ϊ
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
