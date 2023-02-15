using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    private bool mIsActive = false;
    public bool IsActive
    {
        get { return mIsActive; }
    }

    private Vector3 mExitPosition;
    public Vector3 ExitPosition
    {
        get { return mExitPosition; }
    }

    /// <summary>
    /// 激活所在墙体的传送门 
    /// </summary>
    public void Activate()
    {
        if (mIsActive) return;

        mIsActive = true;
        mExitPosition = GameManager.Instance.GetClosestEmptyPos(transform.position);
        GameManager.Instance.AddPortal(transform.position, this);

        var animPrefab = PrefabManager.Instance.GetPrefab(Utilities.ANIM_PORTAL_NAME);
        GameObject.Instantiate(animPrefab, transform.position, Quaternion.identity);

        transform.Find("Portal").gameObject.SetActive(true);
    }

    /// <summary>
    /// 关闭所在墙体的传送门 
    /// </summary>
    public void Deactivate()
    {
        if (!mIsActive) return;

        mIsActive = false;
        GameManager.Instance?.RemovePortal(transform.position);

        transform.Find("Portal").gameObject.SetActive(false);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!mIsActive) return;
        if (collision.gameObject.CompareTag("Player"))
        {
            GameManager.Instance.EnterPortalView();
        }
    }

    private void OnDestroy()
    {
        Deactivate();
    }
}
