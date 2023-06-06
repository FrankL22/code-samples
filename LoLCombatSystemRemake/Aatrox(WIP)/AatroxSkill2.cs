using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utility;

public class AatroxSkill2 : BaseSkill
{
    private float[] CD = { 1f, 18f, 16f, 14f, 12f };
    private float[] damage = { 30f, 40f, 50f, 60f, 70f };
    private float bonus = .4f;
    private float[] slow = { .25f, .275f, .3f, .325f, .35f };

    private float range = 765f;
    private float castTime = .25f;
    private float chainDuration = 1.5f;
    private float speed = 1800f;
    private float projectileLife;

    ChampionBehavior targetChained;
    private float chainTimer = 0f;
    bool chainBroken = true;

    protected override void Start()
    {
        base.Start();

        projectileLife = range / speed;
    }

    protected override void Update()
    {
        base.Update();

        if (chainTimer > 0f)
        {
            chainBroken = false;
            chainTimer -= Time.deltaTime;
            if (chainTimer <= 0f)
            {

            }
        }
    }

    public override void OnPressKey()
    {
        base.OnPressKey();

        if (level < 1 || locked)
            return;

        // stop nav agent
        behavior.agent.isStopped = true;
        behavior.agent.destination = behavior.transform.position;
        locked = true;

        // set launch direction based on cursor position
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        Vector3 target = behavior.transform.position + behavior.transform.forward;
        if (Physics.Raycast(ray, out hit, 1000f, LayerMask.GetMask("Terrain")))
        {
            target = hit.point;
            target.y = 0;
        }
        Vector3 direction = target - behavior.transform.position;
        direction.Normalize();
        StartCoroutine(DelayedLaunchProjectile(direction));
    }

    private IEnumerator DelayedLaunchProjectile(Vector3 direction)
    {
        yield return new WaitForSeconds(castTime);
        CDTimer = CD[level - 1];
        GameObject spawn = Instantiate(((AatroxBehavior)behavior).wProjectilePrefab, 
            behavior.transform.position, Quaternion.LookRotation(direction, Vector3.up));
        BaseProjectile projectile = spawn.GetComponent<BaseProjectile>();
        projectile.InitializeProjectile(direction, speed * Measurements.UNIT_TO_UNITY, 
            projectileLife, behavior, true);
        projectile.d_OnProjectileHit += OnChainHit;
    }

    public void OnChainHit(BaseProjectile projectile, MonoBehaviour target, System.Type type)
    {
        if (type == typeof(ChampionBehavior))
        {
            targetChained = (ChampionBehavior)target;
            targetChained?.commonStats.ApplyRawDamage(
                damage[level - 1] + statistics.attack * bonus, DamageType.Physical);
            targetChained?.effects.ApplyEffect(EffectType.Slow, chainDuration);
            chainTimer = chainDuration;
            Destroy(projectile.gameObject);
        }
    }
}
