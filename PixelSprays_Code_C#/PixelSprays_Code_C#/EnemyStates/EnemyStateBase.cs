using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStateBase
{
    /// <summary>
    /// 状态时间，如果不限时则为-1
    /// </summary>
    protected float mTime = -1;
    protected float mTimer = 0;

    protected Type mNextState = null;
    protected EnemyControl mControl = null;

    /// <summary>
    /// 进入状态并进行相关初始化
    /// </summary>
    /// <param name="pControl">状态所属的敌人</param>
    public virtual void EnterState(EnemyControl pControl)
    {
        mControl = pControl;
    }

    /// <summary>
    /// 每帧更新状态
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
    /// 离开状态
    /// </summary>
    /// <param name="pControl">状态所属的敌人</param>
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
