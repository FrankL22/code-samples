using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 可破坏物组件，破坏生成像素粒
/// </summary>
public class Damagable : MonoBehaviour
{
    #region 常量
    private const int DAMAGE_LOSE = 2;
    private const float LAUNCH_OFFSET = 2f;
    private const float LAUNCH_SPEED = 4f;
    private const float LAUNCH_TIME = 1f;
    private const float BLINK_INTERVAL = .5f;
    #endregion

    #region 变量
    private float mDamageTimer = 0f;
    private float mBlinkTimer = 0f;
    private SpriteRenderer mRenderer;
    private Color mColor;

    public bool OnCooldown = false;
    #endregion

    private void Awake()
    {
        mRenderer = GetComponent<SpriteRenderer>();
        if (mRenderer != null) mColor = mRenderer.color;
    }

    private void FixedUpdate()
    {
        if (!OnCooldown) return;

        if (mRenderer != null)
        {
            mBlinkTimer -= Time.fixedDeltaTime;
            if (mBlinkTimer <= 0)
            {
                mColor.a = mColor.a == 1 ? .3f : 1;
                mRenderer.color = mColor;
                mBlinkTimer = BLINK_INTERVAL;
            }
        }

        mDamageTimer -= Time.fixedDeltaTime;
        if (mDamageTimer <= 0)
        {
            mColor.a = 1;
            mRenderer.color = mColor;

            var magnetic = GetComponentInChildren<Magnetic>();
            if (magnetic != null)
            {
                magnetic.OnCooldown = false;
            }
            OnCooldown = false;
        }
    }

    #region Public方法
    public void Damage()
    {
        if (OnCooldown) return;
        if (gameObject.CompareTag("Player"))
        {
            CameraFollow.Current.CameraShake();
            var pc = gameObject.GetComponent<PlayerControl>();
            if (pc.mHoldObj != null)
            {
                var obj = pc.mHoldObj;
                pc.OnLoseObject(Throwable.THROW_SPEED / 3);
                obj.GetComponent<Destructible>()?.ChainDestruct(Utilities.CHAIN_DESTRUCT_POWER);
                return;
            }
        }

        OnDamaged();
    }
    #endregion

    #region Private方法
    private void OnDamaged()
    {
        OnCooldown = true;
        var magnetic = GetComponentInChildren<Magnetic>();
        if (magnetic != null)
        {
            magnetic.OnCooldown = true;
            mDamageTimer = Utilities.DAMAGE_COOLDOWN;
            mBlinkTimer = BLINK_INTERVAL;
        }

        LaunchBlocks();
    }

    private void LaunchBlocks()
    {
        if (gameObject.CompareTag("Player"))
        {
            if (!Inventory.Instance.UseItem(Utilities.RESOURCE_BLOCK_NAME, DAMAGE_LOSE))
            {
                return;
            }
        }

        var blockPrefab = PrefabManager.Instance.GetPrefab(Utilities.RESOURCE_BLOCK_NAME);
        for (int i = 0; i < DAMAGE_LOSE; i++)
        {
            var direction = Utilities.RandomNormalizedDirection();
            var block = GameObject.Instantiate(blockPrefab, 
                transform.position + direction * LAUNCH_OFFSET, Quaternion.identity);
            block.GetComponent<PixelBlock>().Launch(direction * LAUNCH_SPEED, LAUNCH_TIME, false, false);
        }
    }
    #endregion
}
