using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 随机巡逻，默认状态
/// </summary>
class EnemyPatrolState : EnemyStateBase
{
    private const float TURN_TIME_INTERVAL = 3;
    private const float TURN_TIME_VARIATION = 1;

    /// <summary>
    /// 当前移动方向 * MOVE_SPEED
    /// </summary>
    private Vector3 mMove;
    private float mTurnTimer = 0;

    public override void EnterState(EnemyControl pControl)
    {
        base.EnterState(pControl);

        // TODO: Patrol Enter
        Turn(Vector2.zero);
    }

    public override void OnUpdate(float deltaTime)
    {
        // TODO: Patrol Update
        mControl.Move(mMove * deltaTime);
        if (CheckWithinRange(Utilities.ENEMY_CHASE_RANGE))
        {
            mControl.GoToState(typeof(EnemyChaseState));
            return;
        }

        mTurnTimer -= deltaTime;
        if (mTurnTimer <= 0)
        {
            Turn(Vector2.zero);
        }

        FloorManager.Current.OnSpray(mControl.Position, false);

        base.OnUpdate(deltaTime);
    }

    public override Type OnLeaveState(EnemyControl pControl)
    {
        // TODO: Patrol Leave State

        return base.OnLeaveState(pControl);
    }

    public override void OnCollide(Collision2D pCollision)
    {
        // TODO: Patrol On Collide
        Turn(pCollision.relativeVelocity * -1);

        base.OnCollide(pCollision);
    }

    public void Turn(Vector2 pDirection)
    {
        if (pDirection == Vector2.zero)
        {
            mMove = RandomMove();
        }
        else
        {
            mMove = pDirection;
        }
        ResetTurnTimer();
    }

    private Vector3 RandomMove()
    {
        mMove = Utilities.RandomNormalizedDirection();
        mMove = mMove * Utilities.ENEMY_MOVE_SPEED;
        return mMove;
    }

    private void ResetTurnTimer()
    {
        mTurnTimer = UnityEngine.Random.Range(TURN_TIME_INTERVAL - TURN_TIME_VARIATION,
            TURN_TIME_INTERVAL + TURN_TIME_VARIATION);
    }
}
