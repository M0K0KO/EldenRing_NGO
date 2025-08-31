using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace Moko
{
    public class WorldObjectManager : MonoBehaviour
    {
        public static WorldObjectManager instance;

        [Header("Network Objects")] 
        [SerializeField] private List<NetworkObjectSpawner> networkObjectSpawners;
        [SerializeField] private List<GameObject> spawnedInObjects;
        
        [Header("Fog Walls")]
        public List<FogWallInteractable> fogWalls;

        private void Awake()
        {
            if (instance == null) instance = this;
            else Destroy(gameObject);
        }

        public void SpawnObject(NetworkObjectSpawner networkObjectSpawner)
        {
            if (NetworkManager.Singleton.IsServer)
            {
                networkObjectSpawners.Add(networkObjectSpawner);
                networkObjectSpawner.AttemptToSpawnObject();
            }
        }

        public void AddFogWallToList(FogWallInteractable fogWall)
        {
            if (!fogWalls.Contains(fogWall))
            {
                fogWalls.Add(fogWall);
            }
        }

        public void RemoveFogWallFromList(FogWallInteractable fogWall)
        {
            if (fogWalls.Contains(fogWall))
            {
                fogWalls.Remove(fogWall);
            }
        }
        
        // 2.Spawn in those fogwalls as network objects during start of game (must have a spawner object)
        // 3.Create general object spawner script and prefab
        // 4.When the fog walls are spawned, add them to world fog wall list
        // 5.Grab the correct fogwall from the list on the boss manager when the boss is being initialized
    }
}
