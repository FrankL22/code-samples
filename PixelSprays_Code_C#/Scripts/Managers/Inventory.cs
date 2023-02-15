using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 收集物品相关
/// </summary>
public class Inventory
{
    #region 常量
    #endregion

    #region 变量
    private Dictionary<string, float> mItems = new Dictionary<string, float>();
    #endregion

    #region Public方法
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
    /// 初始化Inventory
    /// </summary>
    public void Init()
    {
        mItems.Clear();
    }

    /// <summary>
    /// 添加可收集物品，名称与对应prefab名称相同
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
    /// 尝试使用物品，如果数量不足则返回false
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

    #region Private方法

    #endregion
}
