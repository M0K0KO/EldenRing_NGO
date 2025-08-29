using System;
using Unity.Netcode;
using UnityEngine;

namespace Moko
{
    public class AICharacterSpawner : MonoBehaviour
    {
        [Header("Character")] 
        [SerializeField] private GameObject characterGameObject;
        [SerializeField] private GameObject instantiatedGameObject;

        private void Awake()
        {
        }

        private void Start()
        {
            WorldAIManager.instance.SpawnCharacter(this);
            gameObject.SetActive(false);
        }

        public void AttemptToSpawnCharacter()
        {
            if (characterGameObject != null)
            {
                instantiatedGameObject = Instantiate(characterGameObject);
                instantiatedGameObject.transform.position = transform.position;
                instantiatedGameObject.transform.rotation = transform.rotation;
                instantiatedGameObject.GetComponent<NetworkObject>().Spawn();
            }
        }
    }
}
