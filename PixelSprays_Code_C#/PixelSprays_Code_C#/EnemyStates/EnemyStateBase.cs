using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStateBase
{
    /// <summary>
    /// ״̬ʱ�䣬�������ʱ��Ϊ-1
    /// </summary>
    protected float mTime = -1;
    protected float mTimer = 0;

    protected Type mNextState = null;
    protected EnemyControl mControl = null;

    /// <summary>
    /// ����״̬��������س�ʼ��
    /// </summary>
    /// <param name="pControl">״̬�����ĵ���</param>
    public virtual void EnterState(EnemyControl pControl)
    {
        mControl = pControl;
    }

    /// <summary>
    /// ÿ֡����״̬
    /// </summary>
    public virtual void OnUpdate(float deltaTime)
    {
        if (mTime > 0)
        {
            mTimer -= deltaTime;
            if (mTimer <= 0)
            {
                mControl?.GoToState(mNextState);
            }
        }
    }

    public virtual void OnCollide(Collision2D pCollision)
    {

    }

    /// <summary>
    /// �뿪״̬
    /// </summary>
    /// <param name="pControl">״̬�����ĵ���</param>
    public virtual Type OnLeaveState(EnemyControl pControl)
    {
        return mNextState;
    }

    protected bool CheckWithinRange(float pRange)
    {
        var dist = PlayerControl.Current.Position - mControl.Position;
        return (dist.magnitude <= pRange);
    }
}
