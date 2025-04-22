using System.Collections.Generic;
using UnityEngine;

namespace TrowelMod
{
    public class GrowthAura : MonoBehaviour
    {
        [Tooltip("The range around this object in which plants grow faster.")]
        public float range = 5.5f;

        private static readonly List<GrowthAura> allAuras = new();

        private void OnEnable()
        {
            if (!allAuras.Contains(this))
                allAuras.Add(this);
        }

        private void OnDisable()
        {
            allAuras.Remove(this);
        }

        public static bool IsInGrowthAura(Vector3 position)
        {
            foreach (var aura in allAuras)
            {
                if (Vector3.Distance(aura.transform.position, position) <= aura.range)
                {
                    return true;
                }
            }

            return false;
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, range);
        }
#endif
    }
}