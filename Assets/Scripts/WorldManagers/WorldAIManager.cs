using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Moko
{
    public class WorldAIManager : MonoBehaviour
    {
        public static WorldAIManager instance;
        


        [Header("Characters")] 
        [SerializeField] private List<AICharacterSpawner> aiCharacterSpawners;
        [SerializeField] private List<GameObject> spawnedInCharacters;

        private void Awake()
        {
            if (instance == null) instance = this;
            else Destroy(gameObject);
        }

        public void SpawnCharacter(AICharacterSpawner aiCharacterSpawner)
        {
            if (NetworkManager.Singleton.IsServer)
            {
                aiCharacterSpawners.Add(aiCharacterSpawner);
                aiCharacterSpawner.AttemptToSpawnCharacter();
            }
        }

        private void DespawnAllCharacters()
        {
            foreach (var character in spawnedInCharacters)
            {
                character.GetComponent<NetworkObject>().Despawn();
            }
        }

        private void DisableAllCharacters()
        {
            // TO DO
            // Disable CharacterGameObjects, syncdisabled status on network
            // Disable GameObjects for clients upon connecting, if the disabled status is true
            // can be used to disable characters that are far from players to save memory
            // characters can be split into areas (AREA_00, AREA_01, AREA_02)
        }
    }
}
