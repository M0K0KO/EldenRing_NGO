using System;
using UnityEngine;

namespace Moko
{
    public class WorldUtilityManager : MonoBehaviour
    {
        public static WorldUtilityManager instance;

        [SerializeField] private LayerMask characterLayers;
        [SerializeField] private LayerMask environmentLayers;

        private void Awake()
        {
            if (instance == null) instance = this;
            else Destroy(gameObject);
        }

        public LayerMask GetCharacterLayers()
        {
            return characterLayers;
        }

        public LayerMask GetEnvironmentLayers()
        {
            return environmentLayers;
        }
    }
}
