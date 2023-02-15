using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Ԥ�ȼ���prefab��������������ȡ
/// </summary>
public class PrefabManager
{
    #region ����
    #endregion

    #region ����
    private static PrefabManager mInstance;
    private Dictionary<string, GameObject> mPrefabs = new Dictionary<string, GameObject>();
    private Dictionary<string, Sprite> mSprites = new Dictionary<string, Sprite>();
    #endregion

    #region Public����
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
    /// ��ʼ��PrefabManager����������prefab
    /// </summary>
    public void Init()
    {
        LoadAllPrefabs();
        LoadAllSprites();
    }

    /// <summary>
    /// ���ؼ��غõ���ΪpName��prefab<br/>
    /// ���û��������ֵ�prefab������null
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

    #region Private����
    /// <summary>
    /// ��ǰ��������prefab��������dictionary�й�ȡ��
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
    /// ��ǰ����������ͼ��Դ
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
