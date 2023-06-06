using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utility;

public class AatroxSkill1 : BaseSkill
{
    private float[] CD = { 1f, 12f, 10f, 8f, 6f };
    private float[,] damage = { { 10f, 30f, 50f, 70f, 90f },
                                { 12.5f, 37.5f, 62.5f, 87.5f, 112.5f},
                                { 15f, 45f, 75f, 105f, 135f} };
    private float[,] bonus = { { .6f, .65f, .7f, .75f, .8f },
                                     { .75f, .815f, .875f, .9375f, 1f},
                                     { .9f, .975f, 1.05f, 1.125f, 1.2f} };

    private Vector2 cast1Range = new Vector2(625f, 360f);
    private Vector2 cast2Range = new Vector2(575f, 500f);
    private float cast2Offset = -100f;
    private float cast3Offset = 300f;
    private float cast3Radius = 300f;
    private float knockupTime = .25f;
    private float castTime = .6f;

    public int currentCast = 1;

    protected override void Start()
    {
        base.Start();
    }

    public override void OnPressKey()
    {
        base.OnPressKey();

        if (level < 1 || locked)
            return;

        // stop nav agent
        behavior.effects.ApplyEffect(EffectType.LockInCast, castTime);
        behavior.agent.destination = behavior.transform.position;

        // turn to cursor direction
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 1000f, LayerMask.GetMask("Terrain")))
        {
            Vector3 target = hit.point;
            target.y = 0;
            Quaternion targetRot = Quaternion.LookRotation(target - behavior.transform.position, Vector3.up);
            behavior.transform.rotation = targetRot;
        }

        ((AatroxBehavior)behavior)?.skill1Indicators[currentCast - 1]?.SetActive(true);
        StartCoroutine(DelayedHit());
    }

    public override void OnReleaseKey()
    {
        base.OnReleaseKey();
    }

    private IEnumerator DelayedHit()
    {
        locked = true;
        CDTimer = (currentCast == 3) ? CD[level - 1] : 1f;
        yield return new WaitForSeconds(castTime);

        List<ChampionBehavior> hit = new List<ChampionBehavior>();
        float baseDamage = damage[currentCast - 1, level - 1] + bonus[currentCast - 1, level - 1] * statistics.attack;
        foreach (ChampionBehavior champion in ActorManager.Get.champions)
        {
            if (champion.team != behavior.team)
            {
                bool isHit = false;
                Vector3 facing = behavior.transform.forward;
                if (currentCast == 1)
                {
                    if (Cast2D.CastCircleAgainstBox(champion.transform.position, 
                        champion.hitRadius * Measurements.UNIT_TO_UNITY,
                        behavior.transform.position + facing * cast1Range.x * Measurements.UNIT_TO_UNITY / 2, 
                        behavior.transform.rotation, 
                        cast1Range.x * Measurements.UNIT_TO_UNITY, cast1Range.y * Measurements.UNIT_TO_UNITY))
                    {
                        isHit = true;
                    }
                }
                else if (currentCast == 2)
                {
                    if (Cast2D.CastCircleAgainstBox(champion.transform.position, 
                        champion.hitRadius * Measurements.UNIT_TO_UNITY,
                        behavior.transform.position + facing * (cast2Offset + cast2Range.x / 2) * Measurements.UNIT_TO_UNITY, 
                        behavior.transform.rotation, 
                        cast2Range.x * Measurements.UNIT_TO_UNITY, cast2Range.y * Measurements.UNIT_TO_UNITY))
                    {
                        isHit = true;
                    }
                }
                else if (currentCast == 3)
                {
                    if (Cast2D.CastCircleAgainstCircle(champion.transform.position, 
                        champion.hitRadius * Measurements.UNIT_TO_UNITY,
                        behavior.transform.position + facing * cast3Offset * Measurements.UNIT_TO_UNITY, 
                        cast3Radius * Measurements.UNIT_TO_UNITY))
                    {
                        isHit = true;
                    }
                }

                if (isHit)
                {
                    champion.commonStats?.ApplyRawDamage(baseDamage, DamageType.Physical);
                    champion.effects?.ApplyEffect(EffectType.Knockup, knockupTime);
                }
            }
        }
        ((AatroxBehavior)behavior)?.skill1Indicators[currentCast - 1]?.SetActive(false);

        currentCast++;
        if (currentCast > 3)
            currentCast = 1;
    }
}
