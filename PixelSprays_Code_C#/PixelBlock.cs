using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ������
/// </summary>
public class PixelBlock : MonoBehaviour
{
    #region ����
    private float COLLECT_TIME_THRESHOLD = .2f;
    #endregion

    #region ����
    // ��ʼ�Ӵݻٵ㷢�����
    private float mCollectTimer = 0f;

    private bool mIsBullet = false;
    private bool mByPlayer = false;

    private bool mIsPulled = false;
    private Transform mPullTarget;
    #endregion

    private void FixedUpdate()
    {
        if (!Utilities.CheckWithinBoundaries(transform.position))
        {
            Destroy(gameObject);
            return;
        }
        if (mCollectTimer > 0f)
        {
            mCollectTimer -= Time.fixedDeltaTime;
        }
        if (mIsPulled)
        {
            var direction = mPullTarget.position - transform.position;
            transform.position += direction.normalized * Magnetic.MAGNET_PULL_SPEED * Time.fixedDeltaTime;
        }
    }

    /// <summary>
    /// ��������Ϊ��Դ�鱻�ռ������
    /// </summary>
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (mIsBullet) return;
        if (mCollectTimer > COLLECT_TIME_THRESHOLD) return;
        if (collision.gameObject.CompareTag("Magnet")) return;

        var damagable = collision.gameObject.GetComponent<Damagable>();
        if (damagable != null && damagable.OnCooldown) return;

        if (collision.gameObject.CompareTag("Player"))
        {
            Inventory.Instance.AddItem(Utilities.RESOURCE_BLOCK_NAME, 1);
            Destroy(gameObject);
        }
        else if (collision.gameObject.CompareTag("Enemy"))
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// ��������Ϊ�ӵ�������������
    /// </summary>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!mIsBullet) return;
        if (collision.gameObject.CompareTag("Magnet")) return;
        if (mByPlayer && collision.gameObject.CompareTag("Player")) return;
        if (!mByPlayer && collision.gameObject.CompareTag("Enemy")) return;

        var destructible = collision.gameObject.GetComponent<Destructible>();
        var damagable = collision.gameObject.GetComponent<Damagable>();
        if (destructible != null) destructible.ChainDestruct(Utilities.CHAIN_DESTRUCT_POWER, mByPlayer);
        else if (damagable != null) damagable.Damage();
        else Destroy(collision.gameObject);

        Destroy(gameObject);
    }

    #region Public����
    public void Launch(in Vector3 pSpeed, float pTime, bool pIsBullet = false, bool pByPlayer = false)
    {
        mCollectTimer = pTime;
        mIsBullet = pIsBullet;
        mByPlayer = pByPlayer;

        GetComponent<Throwable>().Launch(pSpeed, pTime, pByPlayer);
    }

    public void MagnetPull(Transform pTarget)
    {
        if (mIsBullet) return;
        mPullTarget = pTarget;
        mIsPulled = true;
    }
    #endregion

    #region Private����
    #endregion
}
