using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 追击玩家
/// </summary>
class EnemyAttackState : EnemyStateBase
{

    /// <summary>
    /// 当前移动方向 * MOVE_SPEED
    /// </summary>
    private Vector3 mMove;
    private float mSign = 1;

    private const float ATTACK_INTERVAL = 2;
    private float mAttackTimer = 0;

    public EnemyAttackState()
    {
        mNextState = typeof(EnemyChaseState);
    }

    public override void EnterState(EnemyControl pControl)
    {
        base.EnterState(pControl);
        mSign = Utilities.RandomSign();
        TargetPlayer();
        mAttackTimer = ATTACK_INTERVAL;
    }

    public override void OnUpdate(float deltaTime)
    {
        TargetPlayer();
        mControl.Move(mMove * deltaTime);
        if (!CheckWithinRange(Utilities.ENEMY_ATTACK_RANGE))
        {
            mControl.GoToState(mNextState);
            return;
        }

        mAttackTimer -= deltaTime;
        if (mAttackTimer <= 0)
        {
            Attack();
            mAttackTimer = ATTACK_INTERVAL;
        }

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
        mMove = Vector3.Cross(toPlayer, Vector3.forward) * mSign * Utilities.ENEMY_MOVE_SPEED / 3;
    }

    private void Attack()
    {
        var blockPrefab = PrefabManager.Instance.GetPrefab(Utilities.RESOURCE_BLOCK_NAME);
        Vector3 launchDirection = PlayerControl.Current.Position - mControl.Position;
        var block = GameObject.Instantiate(blockPrefab, mControl.Position, Quaternion.identity);
        block.GetComponent<PixelBlock>().Launch(
            launchDirection.normalized * Utilities.LAUNCH_SPEED, -1f, true, false);
    }
}
