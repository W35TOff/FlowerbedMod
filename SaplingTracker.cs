using UnityEngine;

namespace TrowelMod
{

    public class SaplingTracker : MonoBehaviour
    {
        public Vector3 originalPosition;

        private void Awake()
        {
            // Save the sapling’s starting position
            originalPosition = transform.position;
        }
    }

}