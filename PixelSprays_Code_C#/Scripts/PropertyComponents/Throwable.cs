using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 可投掷物组件，处理被投掷相关逻辑
/// </summary>
public class Throwable : MonoBehaviour
{
    #region 常量

    public const float THROW_SPEED = 40f;
    public const float THROW_DECAY_TIME = 0.7f;
    #endregion

    #region 变量
    // 投掷相关
    private Vector3 mLaunchSpeed = Vector3.zero;
    private float mLaunchTimer = 0f;

    private bool mLaunched = false;
    private bool mHasDecay = false;
    private bool mResetRigid = true;
    private bool mByPlayer = false;
    #endregion

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!mLaunched) return;
        if (mByPlayer && collision.gameObject.CompareTag("Player")) return;
        if (!mByPlayer && collision.gameObject.CompareTag("Enemy")) return;

        mLaunched = false;

        var damagable = collision.gameObject.GetComponent<Damagable>();
        damagable?.Damage();

        var destructibleSelf = GetComponent<Destructible>();
        var destructible = collision.gameObject.GetComponent<Destructible>();
        if (destructibleSelf != null && destructibleSelf.CauseChainDestruct)
        {
            destructible?.ChainDestruct(Utilities.CHAIN_DESTRUCT_POWER, mByPlayer);
        }
        else
        {
            destructible?.ChainDestruct(Utilities.CHAIN_DESTRUCT_POWER, mByPlayer);
        }
        destructibleSelf?.ChainDestruct(Utilities.CHAIN_DESTRUCT_POWER, mByPlayer);
    }

    private void FixedUpdate()
    {
        if (!mHasDecay)
        {
            transform.position += mLaunchSpeed * Time.fixedDeltaTime;
        }
        else
        {
            if (mLaunchTimer > 0f)
            {
                float prevTime = mLaunchTimer;
                mLaunchTimer -= Time.fixedDeltaTime;
                mLaunchSpeed *= mLaunchTimer / prevTime;

                transform.position += mLaunchSpeed * Time.fixedDeltaTime;
            }
            else
            {
                mLaunched = false;
                if (mResetRigid)
                {
                    mResetRigid = false;
                    var rigidbody = GetComponent<Rigidbody2D>();
                    if (rigidbody != null)
                    {
                        rigidbody.bodyType = RigidbodyType2D.Static;
                    }
                }
            }
        }
    }
    #region Public方法
    public void Launch(in Vector3 pSpeed, float pTime, bool pByPlayer)
    {
        mLaunchSpeed = pSpeed;
        mLaunchTimer = pTime;
        if (pTime > 0f) mHasDecay = true;

        var rigidbody = GetComponent<Rigidbody2D>();
        if (rigidbody != null)
        {
            rigidbody.gravityScale = 0;
            rigidbody.freezeRotation = true;
            rigidbody.bodyType = RigidbodyType2D.Dynamic;
            mResetRigid = true;
        }
        mLaunched = true;
        mByPlayer = pByPlayer;
    }
    #endregion

    #region Private方法
    #endregion
}
