using System;
using Unity.Netcode;
using UnityEngine;

namespace Moko
{
    public class PlayerUIManager : MonoBehaviour
    {
        public static PlayerUIManager instance;
        
        [Header("NETWORK JOIN")] 
        [SerializeField] private bool startGameAsClient = false;

        [HideInInspector] public PlayerUIHudManager playerUIHudManager;

        private void Awake()
        {
            if (instance == null) instance = this;
            else Destroy(gameObject);
            
            playerUIHudManager = GetComponentInChildren<PlayerUIHudManager>();
        }

        private void Start()
        {
            DontDestroyOnLoad(this);
        }

        private void Update()
        {
            if (startGameAsClient)
            {
                startGameAsClient = false;
                
                // We must first shut down, because we have started as a host during the title screen
                NetworkManager.Singleton.Shutdown();
                
                // we then restart as a client
                NetworkManager.Singleton.StartClient();
            }
        }
    }
}
