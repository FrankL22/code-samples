using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ��������������������뷶Χ��������
/// </summary>
public class Magnetic : MonoBehaviour
{
    #region ����
    public const float MAGNET_PULL_SPEED = 20f;
    #endregion

    #region ����
    public bool OnCooldown = false;
    #endregion


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (OnCooldown) return;
        // ֻ����PixelBlock����
        if (collision.gameObject.CompareTag(Utilities.RESOURCE_BLOCK_NAME))
        {
            var pixelBlock = collision.gameObject.GetComponent<PixelBlock>();
            pixelBlock?.MagnetPull(gameObject.transform.parent);
        }
    }
    #region Public����

    #endregion

    #region Private����
    #endregion
}
