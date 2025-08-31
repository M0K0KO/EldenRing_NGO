using Unity.Netcode;
using UnityEngine;

namespace Moko
{
    public class NetworkObjectSpawner : MonoBehaviour
    {
        [Header("Object")] 
        [SerializeField] private GameObject networkGameObject;
        [SerializeField] private GameObject instantiatedGameObject;

        private void Awake()
        {
        }

        private void Start()
        {
            WorldObjectManager.instance.SpawnObject(this);
            gameObject.SetActive(false);
        }

        public void AttemptToSpawnObject()
        {
            if (networkGameObject != null)
            {
                instantiatedGameObject = Instantiate(networkGameObject);
                instantiatedGameObject.transform.position = transform.position;
                instantiatedGameObject.transform.rotation = transform.rotation;
                instantiatedGameObject.GetComponent<NetworkObject>().Spawn();
            }
        }
    }
}
