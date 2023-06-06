using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseSkill : MonoBehaviour
{
    #region Control
    [Header("Control")]
    public KeyCode key;
    protected float CDTimer = 0f;
    #endregion

    #region Status
    [Space, Header("Status")]
    public int level = 0;
    public bool locked = false;
    #endregion

    protected ChampionBehavior behavior;
    protected CommonStatistics statistics;

    protected virtual void Start()
    {
        behavior = transform.parent?.GetComponent<ChampionBehavior>();
        statistics = transform.parent?.GetComponent<CommonStatistics>();
        behavior?.RegisterKeyDelegate(key, OnPressKey, OnReleaseKey);
    }

    public virtual void OnPressKey()
    {

    }

    public virtual void OnReleaseKey()
    {

    }

    protected virtual void Update()
    {
        if (CDTimer > 0f)
            CDTimer -= Time.deltaTime;
        if (CDTimer <= 0f)
        {
            locked = false;
        }
    }
}
