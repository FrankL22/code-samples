using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EffectType
{
    Slow, 
    Root,
    LockInCast,
    Stun,
    Knockup,
    Silence,
    Fear,
    Blind,
    Charm,
    Invincible,
    Untargetable
}

public class Effects : MonoBehaviour
{
    private ChampionBehavior behavior;
    private CommonStatistics statistics;

    private static System.Array allTypes = System.Enum.GetValues(typeof(EffectType));
    public static List<EffectType> blocksAttack = new List<EffectType>
        { EffectType.LockInCast, EffectType.Stun, EffectType.Knockup, EffectType.Fear, EffectType.Charm };

    public Dictionary<EffectType, float> activeEffects;

    #region Status
    [Header("Status")]
    public bool canMove = true;
    public bool canAttack = true;
    public bool canCast = true;
    #endregion

    private void Awake()
    {
        activeEffects = new Dictionary<EffectType, float>();
        foreach (EffectType type in allTypes)
        {
            activeEffects[type] = 0f;
        }
    }

    private void Start()
    {
        behavior = GetComponent<ChampionBehavior>();
        statistics = GetComponent<CommonStatistics>();
    }

    public void ApplyEffect(EffectType type, float duration)
    {
        activeEffects[type] = Mathf.Max(activeEffects[type], duration);
        Debug.Log($"{gameObject.name} is {System.Enum.GetName(typeof(EffectType), type)}ed for {duration} seconds");
    }

    private void Update()
    {
        canMove = true;
        canAttack = true;
        canCast = true;
        foreach (EffectType type in allTypes)
        {
            if (activeEffects[type] > 0f)
                activeEffects[type] -= Time.deltaTime;
            if (blocksAttack.Contains(type) && activeEffects[type] > 0f)
            {
                canMove = false;
                canAttack = false;
                canCast = false;
            }
            else if (type == EffectType.Root && activeEffects[type] > 0f)
            {
                canMove = false;
            }
            else if (type == EffectType.Silence && activeEffects[type] > 0f)
            {
                canCast = false;
            }
        }
        behavior.agent.isStopped = !canMove;
    }
}
