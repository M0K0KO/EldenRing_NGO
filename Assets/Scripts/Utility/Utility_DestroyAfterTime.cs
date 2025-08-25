using System;
using UnityEngine;

namespace Moko
{
    public class Utility_DestroyAfterTime : MonoBehaviour
    {
        [SerializeField] private float timeUntilDestroyed = 5;

        private void Awake()
        {
            Destroy(gameObject, timeUntilDestroyed);
        }
    }
}
