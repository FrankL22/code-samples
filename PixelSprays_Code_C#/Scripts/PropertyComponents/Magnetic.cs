using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 磁力吸引组件，吸引进入范围的像素粒
/// </summary>
public class Magnetic : MonoBehaviour
{
    #region 常量
    public const float MAGNET_PULL_SPEED = 20f;
    #endregion

    #region 变量
    public bool OnCooldown = false;
    #endregion


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (OnCooldown) return;
        // 只吸引PixelBlock物体
        if (collision.gameObject.CompareTag(Utilities.RESOURCE_BLOCK_NAME))
        {
            var pixelBlock = collision.gameObject.GetComponent<PixelBlock>();
            pixelBlock?.MagnetPull(gameObject.transform.parent);
        }
    }
    #region Public方法

    #endregion

    #region Private方法
    #endregion
}
