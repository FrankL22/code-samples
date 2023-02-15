using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 预先加载prefab，按名字索引获取
/// </summary>
public class PrefabManager
{
    #region 常量
    #endregion

    #region 变量
    private static PrefabManager mInstance;
    private Dictionary<string, GameObject> mPrefabs = new Dictionary<string, GameObject>();
    private Dictionary<string, Sprite> mSprites = new Dictionary<string, Sprite>();
    #endregion

    #region Public方法
    public static PrefabManager Instance
    {
        get
        {
            if (mInstance == null)
            {
                mInstance = new PrefabManager();
            }
            return mInstance;
        }
    }

    /// <summary>
    /// 初始化PrefabManager，加载所有prefab
    /// </summary>
    public void Init()
    {
        LoadAllPrefabs();
        LoadAllSprites();
    }

    /// <summary>
    /// 返回加载好的名为pName的prefab<br/>
    /// 如果没有这个名字的prefab，返回null
    /// </summary>
    public GameObject GetPrefab(string pName)
    {
        if (!mPrefabs.ContainsKey(pName)) return null;
        else return mPrefabs[pName];
    }

    public Sprite GetSprite(string pName)
    {
        if (!mSprites.ContainsKey(pName)) return null;
        else return mSprites[pName];
    }
    #endregion

    #region Private方法
    /// <summary>
    /// 提前加载所有prefab，保存在dictionary中供取用
    /// </summary>
    private void LoadAllPrefabs()
    {
        var objs = Resources.LoadAll<GameObject>(Utilities.PREFAB_DIR);
        for (int i = 0; i < objs.Length; i++)
        {
            mPrefabs[objs[i].name] = objs[i];
        }
    }

    /// <summary>
    /// 提前加载所有贴图资源
    /// </summary>
    private void LoadAllSprites()
    {
        var objs = Resources.LoadAll<Sprite>(Utilities.SPRITE_DIR);
        for (int i = 0; i < objs.Length; i++)
        {
            mSprites[objs[i].name] = objs[i];
        }
    }
    #endregion
}
