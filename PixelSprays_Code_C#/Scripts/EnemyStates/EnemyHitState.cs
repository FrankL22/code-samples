using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 受击逃离玩家
/// </summary>
class EnemyHitState : EnemyStateBase
{
    /// <summary>
    /// 当前移动方向 * MOVE_SPEED
    /// </summary>
    private Vector3 mMove;

    private float mFleeTimer = 0;

    public EnemyHitState()
    {
        mNextState = typeof(EnemyPatrolState);
    }

    public override void EnterState(EnemyControl pControl)
    {
        base.EnterState(pControl);
        FleeFromPlayer();
    }

    public override void OnUpdate(float deltaTime)
    {
        mControl.Move(mMove * deltaTime);
        mFleeTimer -= deltaTime;
        if (mFleeTimer <= 0)
        {
            mControl.GoToState(mNextState);
            return;
        }

        base.OnUpdate(deltaTime);
    }

    public override Type OnLeaveState(EnemyControl pControl)
    {
        return base.OnLeaveState(pControl);                                                                                                                                       
    }

    public override void OnCollide(Collision2D pCollision)
    {
        mMove = Vector3.zero;
        base.OnCollide(pCollision);
    }

    private void FleeFromPlayer()
    {
        var playerPos = PlayerControl.Current.Position;
        var currPos = mControl.Position;
        mMove = -1 * (playerPos - currPos).normalized * Utilities.ENEMY_MOVE_SPEED;
        mFleeTimer = Utilities.DAMAGE_COOLDOWN;
    }
}
