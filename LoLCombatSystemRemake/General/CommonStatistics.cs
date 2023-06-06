using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utility;

public enum ResourceType
{
    Mana,
    Energy,
    None
}
public enum DamageType
{
    Physical,
    Magic,
    True
}

public delegate void OnHurt(float amount);
public delegate void OnUseResource(float amount);

public class CommonStatistics : MonoBehaviour
{
    #region Base Stats
    // base stats only change when leveling up
    [Header("Base Stats")]
    public float baseHP;
    public float baseHPPerLevel;
    public float baseResource;
    public float baseResourcePerLevel;
    public ResourceType resourceType;

    public float baseHPRegen; 
    public float baseHPRegenPerLevel;
    public float baseResourceRegen;
    public float baseResourceRegenPerLevel;
    
    public float baseAttack;
    public float baseAttackPerLevel;
    public float baseMagic;
    public float baseArmor;
    public float baseArmorPerLevel;
    public float baseResist;
    public float baseResistPerLevel;

    public float baseAttackSpeed;
    public float baseAttackSpeedPerLevel;
    public float baseAttackRange;
    public float baseMoveSpeed;
    #endregion

    #region True Stats
    // true stats take into account active effects and items
    [Space, Header("True Stats")]
    public float maxHP;
    public float maxResource;
    public float HPRegen;
    public float resourceRegen;
    public float attack;
    public float magic;
    public float armor;
    public float resist;
    public float attackSpeed;
    public float attackRange;
    public float moveSpeed;
    #endregion

    #region Current Stats
    // dynamic stats that are modified constantly in combat
    [Space, Header("Current Stats")]
    public float currentHP;
    public float currentResource;
    [HideInInspector] public int level = 1;
    #endregion

    #region Combat Status
    #endregion

    #region Delegates
    public OnHurt d_OnHurt;
    public OnUseResource d_OnUseResource;
    #endregion

    private void Awake()
    {
        SyncTrueStats();
    }

    private void FixedUpdate()
    {
        // natural regens
        if (currentHP < maxHP)
        {
            currentHP = Mathf.Min(currentHP + HPRegen * Time.fixedDeltaTime, maxHP);
        }
        if (currentResource < maxResource)
        {
            currentResource = Mathf.Min(currentResource + resourceRegen * Time.fixedDeltaTime, maxResource);
        }
    }

    public void ApplyRawDamage(float rawDamage, DamageType type)
    {
        float trueDamage = rawDamage;
        if (type == DamageType.Physical)
        {
            trueDamage = CombatCalculation.DamageAfterResistance(trueDamage, armor);
        }
        else if (type == DamageType.Magic)
        {
            trueDamage = CombatCalculation.DamageAfterResistance(trueDamage, resist);
        }
        ModifyHPAndResource(-trueDamage, 0);
    }

    public void ModifyHPAndResource(float HPChange, float resourceChange)
    {
        if (HPChange != 0f)
        {
            currentHP = Mathf.Clamp(currentHP + HPChange, 0f, maxHP);
            if (HPChange < 0f)
            {
                d_OnHurt?.Invoke(HPChange);
            }
        }
        if (resourceChange != 0f)
        {
            currentResource = Mathf.Clamp(currentResource + resourceChange, 0f, maxResource);
            if (resourceChange < 0f)
            {
                d_OnUseResource?.Invoke(resourceChange);
            }
        }
    }

    public void LevelUp()
    {
        if (level > 17)
            return;

        // stats growth by level
        baseHP += baseHPPerLevel;
        maxHP += baseHPPerLevel;
        currentHP += baseHPPerLevel;
        baseResource += baseResourcePerLevel;
        maxResource += baseResourcePerLevel;
        currentResource += baseResourcePerLevel;

        baseHPRegen += baseHPRegenPerLevel;
        HPRegen += baseHPRegenPerLevel;
        baseResourceRegen += baseResourceRegenPerLevel;
        resourceRegen += baseResourceRegenPerLevel;

        baseAttack += baseAttackPerLevel;
        attack += baseAttackPerLevel;
        baseArmor += baseArmorPerLevel;
        armor += baseArmorPerLevel;
        baseResist += baseResistPerLevel;
        resist += baseResistPerLevel;

        baseAttackSpeed += baseAttackSpeedPerLevel;
        attackSpeed += baseAttackSpeedPerLevel;
    }

    public void SyncCurrentStats()
    {
        currentHP = Mathf.Min(currentHP, maxHP);
        currentResource = Mathf.Min(currentResource, maxResource);
    }
    
    private void SyncTrueStats()
    {
        maxHP = baseHP;
        maxResource = baseResource;
        HPRegen = baseHPRegen;
        resourceRegen = baseResourceRegen;
        attack = baseAttack;
        magic = baseMagic;
        armor = baseArmor;
        resist = baseResist;
        attackSpeed = baseAttack;
        attackRange = baseAttackRange;
        moveSpeed = baseMoveSpeed;
    }
}
