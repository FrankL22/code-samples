using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utility;

public class AatroxSkill3 : BaseSkill
{
    private float[] CD = { 1f, 8f, 7f, 6f, 5f };

    private float minDist = 75f;
    private float maxDist = 300f;
    private float dashSpeed = 800f;

    private float remainingDashTime = 0f;
    private Vector3 dashTarget;

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
        behavior.agent.isStopped = true;
        behavior.agent.destination = behavior.transform.position;
        locked = true;

        // set dash target based on cursor position
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        Vector3 target = behavior.transform.position + behavior.transform.forward;
        float targetDist = 1f;
        if (Physics.Raycast(ray, out hit, 1000f, LayerMask.GetMask("Terrain")))
        {
            target = hit.point;
            target.y = 0;
            Vector3 direction = target - behavior.transform.position;
            targetDist = direction.magnitude;
            if (targetDist < minDist * Measurements.UNIT_TO_UNITY)
            {
                direction.Normalize();
                target = behavior.transform.position + direction * minDist * Measurements.UNIT_TO_UNITY;
                targetDist = minDist * Measurements.UNIT_TO_UNITY;
            }
            else if (targetDist > maxDist * Measurements.UNIT_TO_UNITY)
            {
                direction.Normalize();
                target = behavior.transform.position + direction * maxDist * Measurements.UNIT_TO_UNITY;
                targetDist = maxDist * Measurements.UNIT_TO_UNITY;
            }
        }
        // turn to face target and set dash target
        Quaternion targetRot = Quaternion.LookRotation(target - behavior.transform.position, Vector3.up);
        behavior.transform.rotation = targetRot;
        dashTarget = target;
        remainingDashTime = targetDist / dashSpeed;
        CDTimer = CD[level - 1];
    }

    public override void OnReleaseKey()
    {
        base.OnReleaseKey();
    }

    protected override void Update()
    {
        base.Update();

        if (remainingDashTime > 0f)
        {
            behavior.transform.position = Movement.LerpPosition(
                behavior.transform.position, dashTarget, remainingDashTime, Time.deltaTime);

            remainingDashTime -= Time.deltaTime;
            if (remainingDashTime <= 0f)
            {
                // resume nav agent
                behavior.agent.isStopped = false;
                behavior.agent.destination = dashTarget;
            }
        }
    }
}
