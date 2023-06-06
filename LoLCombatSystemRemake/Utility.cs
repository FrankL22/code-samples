using UnityEngine;

namespace Utility
{
    public static class Measurements
    {
        public const float UNIT_TO_UNITY = 0.01f;
        public const float UNIT_SQ_TO_UNITY = 0.0001f;
    }

    public static class Cast2D
    {
        /// <summary>
        /// setting radius1 to 0, this will work as casting point against box
        /// </summary>
        public static bool CastCircleAgainstBox(Vector3 center1, float radius1, 
            Vector3 center2, Quaternion rot2, float height2, float width2)
        {
            Vector3 relativePoint = center1 - center2;
            Quaternion invQuat = Quaternion.Inverse(rot2);
            relativePoint = invQuat * relativePoint;
            relativePoint.x = Mathf.Abs(relativePoint.x);
            relativePoint.z = Mathf.Abs(relativePoint.z);

            float dx = Mathf.Max(relativePoint.x - height2 / 2, 0);
            float dz = Mathf.Max(relativePoint.z - width2 / 2, 0);
            float dSq = dx * dx + dz * dz;

            return dSq <= (radius1 * radius1);
        }

        /// <summary>
        /// setting radius1 to 0, this will work as casting point against circle
        /// </summary>
        public static bool CastCircleAgainstCircle(Vector3 center1, float radius1, Vector3 center2, float radius2)
        {
            float dSq = (center1 - center2).sqrMagnitude;
            return dSq <= (radius1 + radius2);
        }
    }

    public static class CombatCalculation
    {
        public static float DamageAfterResistance(float damage, float resistance)
        {
            if (resistance < 0)
                return damage * (2f - (100f / 100f - resistance));
            else
                return damage * (100f / 100f + resistance);
        }
    }

    public static class Movement
    {
        /// <summary>
        /// returns target location resolved for terrain overlaps
        /// </summary>
        public static Vector3 LerpPosition(Vector3 start, Vector3 target, float remainingTime, float deltaTime)
        {
            Vector3 pos = Vector3.Lerp(start, target, 
                Mathf.Min(deltaTime / remainingTime, 1f));
            if (deltaTime/remainingTime >= 1f)
            {
                // TODO: resolve overlapping with terrain
            }
            return pos;
        }
    }
}
