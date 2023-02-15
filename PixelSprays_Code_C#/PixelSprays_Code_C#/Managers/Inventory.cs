using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// �ռ���Ʒ���
/// </summary>
public class Inventory
{
    #region ����
    #endregion

    #region ����
    private Dictionary<string, float> mItems = new Dictionary<string, float>();
    #endregion

    #region Public����
    private static Inventory mInstance;
    public static Inventory Instance
    {
        get
        {
            if (mInstance == null)
            {
                mInstance = new Inventory();
            }
            return mInstance;
        }
    }

    /// <summary>
    /// ��ʼ��Inventory
    /// </summary>
    public void Init()
    {
        mItems.Clear();
    }

    /// <summary>
    /// ��ӿ��ռ���Ʒ���������Ӧprefab������ͬ
    /// </summary>
    public void AddItem(string pName, float pCount = 1)
    {
        if (!mItems.ContainsKey(pName))
        {
            mItems[pName] = 0;
        }
        mItems[pName] += pCount;

        var resource = mItems[Utilities.RESOURCE_BLOCK_NAME];
        HUD.Current.UpdateResourceBar(resource);
        PlayerControl.Current.UpdateResourceBar(resource);
    }

    /// <summary>
    /// ����ʹ����Ʒ��������������򷵻�false
    /// </summary>
    public bool UseItem(string pName, float pCount = 1)
    {
        if (!mItems.ContainsKey(pName)) return false;
        if (mItems[pName] < pCount) return false;

        mItems[pName] -= pCount;

        var resource = mItems[Utilities.RESOURCE_BLOCK_NAME];
        HUD.Current.UpdateResourceBar(resource);
        PlayerControl.Current.UpdateResourceBar(resource);

        return true;
    }
    #endregion

    #region Private����

    #endregion
}
