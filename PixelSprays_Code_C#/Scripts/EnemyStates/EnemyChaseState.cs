using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 追击玩家
/// </summary>
class EnemyChaseState : EnemyStateBase
{

    /// <summary>
    /// 当前移动方向 * MOVE_SPEED
    /// </summary>
    private Vector3 mMove;
    private float mSign;

    public EnemyChaseState()
    {
        mNextState = typeof(EnemyPatrolState);
    }

    public override void EnterState(EnemyControl pControl)
    {
        base.EnterState(pControl);
        mSign = Utilities.RandomSign();
        TargetPlayer();
    }

    public override void OnUpdate(float deltaTime)
    {
        TargetPlayer();
        mControl.Move(mMove * deltaTime);
        if (CheckWithinRange(Utilities.ENEMY_ATTACK_RANGE))
        {
            mControl.GoToState(typeof(EnemyAttackState));
            return;
        }
        else if (!CheckWithinRange(Utilities.ENEMY_CHASE_RANGE))
        {
            mControl.GoToState(mNextState);
            return;
        }

        FloorManager.Current.OnSpray(mControl.Position, false);

        base.OnUpdate(deltaTime);
    }

    public override Type OnLeaveState(EnemyControl pControl)
    {
        return base.OnLeaveState(pControl);                                                                                                                                       
    }

    public override void OnCollide(Collision2D pCollision)
    {
        base.OnCollide(pCollision);
    }

    private void TargetPlayer()
    {
        var playerPos = PlayerControl.Current.Position;
        var currPos = mControl.Position;
        var toPlayer = (playerPos - currPos).normalized;
        mMove = toPlayer * Utilities.ENEMY_MOVE_SPEED;

        var tangent = Vector3.Cross(toPlayer, Vector3.forward);
        mMove += tangent * mSign * Utilities.ENEMY_MOVE_SPEED / 2;
    }
}
