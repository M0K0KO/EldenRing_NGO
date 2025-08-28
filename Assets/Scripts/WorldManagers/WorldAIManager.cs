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

        [Header("DEBUG")] 
        [SerializeField] private bool despawnCharacters = false;
        [SerializeField] private bool respawnCharacters = false;
        

        [Header("Characters")] 
        [SerializeField] private GameObject[] aiCharacters;
        [SerializeField] private List<GameObject> spawnedInCharacters;

        private void Awake()
        {
            if (instance == null) instance = this;
            else Destroy(gameObject);
        }

        private void Start()
        {
            if (NetworkManager.Singleton.IsServer)
            {
                // Spawn All ai in scene
                StartCoroutine(WaitForSceneToLoadThenSpawnCharacters());
            }
        }

        private void Update()
        {
            if (respawnCharacters)
            {
                respawnCharacters = false;
                SpawnAllCharacters();
            }
            
            if (despawnCharacters)
            {
                despawnCharacters = false;
                DespawnAllCharacters();
            }
        }

        private IEnumerator WaitForSceneToLoadThenSpawnCharacters()
        {
            while (!SceneManager.GetActiveScene().isLoaded)
            {
                yield return null;
            }

            SpawnAllCharacters();
        }

        private void SpawnAllCharacters()
        {
            foreach (var character in aiCharacters)
            {
                GameObject instantiatedCharacter = Instantiate(character);
                instantiatedCharacter.GetComponent<NetworkObject>().Spawn();
                spawnedInCharacters.Add(instantiatedCharacter);
            }
        }

        private void DespawnAllCharacters()
        {
            foreach (var character in spawnedInCharacters)
            {
                Debug.Log($"Character isSpawned : {character.GetComponent<NetworkObject>().IsSpawned}");
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
