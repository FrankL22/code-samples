using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���ƻ���������ƻ�����������
/// </summary>
public class Destructible : MonoBehaviour
{
    #region ����
    private const float BLOCK_SIZE = 1f;
    private const float LAUNCH_SPEED_MIN = 3f;
    private const float LAUNCH_SPEED_MAX = 5f;
    private const float LAUNCH_TIME_MIN = .5f;
    private const float LAUNCH_TIME_MAX = 1f;
    #endregion

    #region ����
    /// <summary>�Ƿ����������ը</summary>
    public bool CauseChainDestruct = false;
    /// <summary>�ݻ�ʱ���ỹ��������Ƭ</summary>
    public bool Spray = false;
    private RaycastHit2D[] mCast = new RaycastHit2D[8];
    #endregion

    private void OnCollisionEnter2D(Collision2D collision)
    {
        
    }

    private void Awake()
    {
        
    }
    #region Public����
    public void Destruct(bool pByPlayer = true)
    {
        OnDestruct(pByPlayer);
    }

    public void ChainDestruct(int pPower, bool pByPlayer = false)
    {
        if (pPower < 1) return;
        if (!CauseChainDestruct)
        {
            OnDestruct(pByPlayer);
            return;
        }

        float delay = Utilities.CHAIN_DESTRUCT_DELAY * (Utilities.CHAIN_DESTRUCT_POWER - pPower);
        StartCoroutine(DelayedDestruct(delay, pByPlayer));

        if (Physics2D.CircleCastNonAlloc(transform.position,
            Utilities.CHAIN_DESTRUCT_RADIUS, Vector2.zero, mCast) > 0)
        {
            for (int i = 0; i < mCast.Length; i++)
            {
                if (mCast[i].transform == null || mCast[i].transform == transform) continue;

                var damagable = mCast[i].transform.gameObject.GetComponent<Damagable>();
                damagable?.Damage();
                var destructible = mCast[i].transform.gameObject.GetComponent<Destructible>();
                destructible?.ChainDestruct(pPower - 1, pByPlayer);
            }
        }
    }

    private IEnumerator DelayedDestruct(float pDelay, bool pByPlayer)
    {
        yield return new WaitForSeconds(pDelay);

        // ���ɱ�ը��Ч
        var destructAnim = PrefabManager.Instance.GetPrefab(Utilities.ANIM_DESTRUCT_NAME);
        GameObject.Instantiate(destructAnim, transform.position, Quaternion.identity);

        OnDestruct(pByPlayer);
    }
    #endregion

    #region Private����
    private void OnDestruct(bool pByPlayer)
    {
        int width = (int)transform.localScale.x;
        int height = (int)transform.localScale.y;

        if (Spray)
        {
            var sprayAnim = PrefabManager.Instance.GetPrefab(Utilities.ANIM_SPRAY_NAME);
            GameObject.Instantiate(sprayAnim, transform.position, Quaternion.identity);
            FloorManager.Current?.OnSpray(transform.position, pByPlayer, 8);
        }
        else
        {
            // ��������ת�����½����꣬��Ϊ��ʼ����λ��
            Vector3 pivot = transform.position
                - new Vector3(width / 2f - 0.5f, height / 2f - 0.5f, 0f) * BLOCK_SIZE;
            LaunchBlocks(pivot, width, height);
        }

        Destroy(gameObject);
    }

    private void LaunchBlocks(in Vector3 pPivot, int pWidth, int pHeight)
    {
        var blockPrefab = PrefabManager.Instance.GetPrefab(Utilities.RESOURCE_BLOCK_NAME);
        var blockPos = pPivot;
        Random.InitState(System.DateTime.Now.Millisecond);

        for (int i = 0; i < pWidth; i++)
        {
            for (int j = 0; j < pHeight; j++)
            {
                var block = Instantiate(blockPrefab, blockPos, Quaternion.identity);
                Vector3 direction = blockPos - transform.position;
                float speed = Random.Range(LAUNCH_SPEED_MIN, LAUNCH_SPEED_MAX);
                float time = Random.Range(LAUNCH_TIME_MIN, LAUNCH_TIME_MAX);
                block.GetComponent<PixelBlock>().Launch(direction * speed, time);

                blockPos.y += BLOCK_SIZE;
            }
            blockPos.x += BLOCK_SIZE;
            blockPos.y = pPivot.y;
        }
    }
    #endregion
}
