using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;

namespace Moko
{
    public class PlayerManager : CharacterManager
    {
        [Header("DEBUG MENU")] 
        [SerializeField] private bool respawnCharacter = false;
        [SerializeField] private bool switchRightWeapon = false;
        [SerializeField] private bool switchLeftWeapon = false;
        
        [HideInInspector] public PlayerAnimatorManager playerAnimatorManager;
        [HideInInspector] public PlayerLocomotionManager playerLocomotionManager;
        [HideInInspector] public PlayerNetworkManager playerNetworkManager;
        [HideInInspector] public PlayerStatsManager playerStatsManager;
        [HideInInspector] public PlayerInventoryManager playerInventoryManager;
        [HideInInspector] public PlayerEquipmentManager playerEquipmentManager;
        [HideInInspector] public PlayerCombatManager playerCombatManager;
        
        protected override void Awake()
        {
            base.Awake();
            
            // Do more stuff only for player
            
            playerLocomotionManager = GetComponent<PlayerLocomotionManager>();
            playerAnimatorManager = GetComponent<PlayerAnimatorManager>();
            playerNetworkManager = GetComponent<PlayerNetworkManager>();
            playerStatsManager = GetComponent<PlayerStatsManager>();
            playerInventoryManager = GetComponent<PlayerInventoryManager>();
            playerEquipmentManager = GetComponent<PlayerEquipmentManager>();
            playerCombatManager = GetComponent<PlayerCombatManager>();
        }

        protected override void Update()
        {
            base.Update();

            // if we do not own this gameobject, we do not control or edit it
            if (!IsOwner) return;
            
            // Handle Movement
            playerLocomotionManager.HandleAllMovement();
            
            // Regen Stamina
            playerStatsManager.RegenerateStamina();

            DebugMenu();
        }

        protected override void LateUpdate()
        {
            if (!IsOwner) return;
            
            base.LateUpdate();
            
            PlayerCamera.instance.HandleAllCameraActions();
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnectedCallback;
            
            if (IsOwner)
            {
                PlayerCamera.instance.player = this;
                PlayerInputManager.instance.player = this;
                WorldSaveGameManager.instance.player = this;

                // Update the total amount of health or stamina when the stat linked to either changes
                playerNetworkManager.vitality.OnValueChanged +=
                    playerNetworkManager.SetNewMaxHealthValue;              
                playerNetworkManager.endurance.OnValueChanged +=
                    playerNetworkManager.SetNewMaxStaminaValue;
                
                // Updates UI stat bars when a stat changes
                playerNetworkManager.currentHealth.OnValueChanged +=
                    PlayerUIManager.instance.playerUIHudManager.SetNewHealthValue;
                
                playerNetworkManager.currentStamina.OnValueChanged +=
                    PlayerUIManager.instance.playerUIHudManager.SetNewStaminaValue;
                playerNetworkManager.currentStamina.OnValueChanged +=
                    playerStatsManager.ResetStaminaRegenTimer;
            }

            // STATS
            playerNetworkManager.currentHealth.OnValueChanged += playerNetworkManager.CheckHP;
            
            // LOCK ON
            playerNetworkManager.isLockedOn.OnValueChanged +=
                playerNetworkManager.OnIsLockedOnChanged;
            playerNetworkManager.currentTargetNetworkObjectID.OnValueChanged +=
                playerNetworkManager.OnLockOnTargetIDChange;
            
            // EQUIPMENT
            playerNetworkManager.currentRightHandWeaponID.OnValueChanged +=
                playerNetworkManager.OnCurrentRightHandWeaponIDChange;
            playerNetworkManager.currentLeftHandWeaponID.OnValueChanged +=
                playerNetworkManager.OnCurrentLeftHandWeaponIDChange;
            playerNetworkManager.currentWeaponBeingUsed.OnValueChanged +=
                playerNetworkManager.OnCurrentWeaponBeingUsedIDChange;
            
            // FLAGS
            playerNetworkManager.isChargingAttack.OnValueChanged +=
                playerNetworkManager.OnIsChargingAttackChanged;

            if (IsOwner && !IsServer)
            {
                LoadGameDataFromCurrentCharacterData(ref WorldSaveGameManager.instance.currentCharacterData);
            }
        }

        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();
            
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnectedCallback;
            
            if (IsOwner)
            {
                // Update the total amount of health or stamina when the stat linked to either changes
                playerNetworkManager.vitality.OnValueChanged -=
                    playerNetworkManager.SetNewMaxHealthValue;              
                playerNetworkManager.endurance.OnValueChanged -=
                    playerNetworkManager.SetNewMaxStaminaValue;
                
                // Updates UI stat bars when a stat changes
                playerNetworkManager.currentHealth.OnValueChanged -=
                    PlayerUIManager.instance.playerUIHudManager.SetNewHealthValue;
                
                playerNetworkManager.currentStamina.OnValueChanged -=
                    PlayerUIManager.instance.playerUIHudManager.SetNewStaminaValue;
                playerNetworkManager.currentStamina.OnValueChanged -=
                    playerStatsManager.ResetStaminaRegenTimer;
            }

            // STATS
            playerNetworkManager.currentHealth.OnValueChanged -= playerNetworkManager.CheckHP;
            
            // LOCK ON
            playerNetworkManager.isLockedOn.OnValueChanged -=
                playerNetworkManager.OnIsLockedOnChanged;
            playerNetworkManager.currentTargetNetworkObjectID.OnValueChanged -=
                playerNetworkManager.OnLockOnTargetIDChange;
            
            // EQUIPMENT
            playerNetworkManager.currentRightHandWeaponID.OnValueChanged -=
                playerNetworkManager.OnCurrentRightHandWeaponIDChange;
            playerNetworkManager.currentLeftHandWeaponID.OnValueChanged -=
                playerNetworkManager.OnCurrentLeftHandWeaponIDChange;
            playerNetworkManager.currentWeaponBeingUsed.OnValueChanged -=
                playerNetworkManager.OnCurrentWeaponBeingUsedIDChange;
            
            // FLAGS
            playerNetworkManager.isChargingAttack.OnValueChanged -=
                playerNetworkManager.OnIsChargingAttackChanged;
        }

        private void OnClientConnectedCallback(ulong clientID)
        {
            // keep a list of active players in the game
            WorldGameSessionManager.instance.AddPlayerToActivePlayersList(this);

            if (!IsServer && IsOwner)
            {
                foreach (var player in WorldGameSessionManager.instance.players)
                {
                    if (player != this)
                    {
                        player.LoadOtherPlayerCharacterWhenJoiningServer();
                    }
                }
            }
        }

        public override void ReviveCharacter()
        {
            base.ReviveCharacter();

            if (IsOwner)
            {
                isDead.Value = false;
                playerNetworkManager.currentHealth.Value = playerNetworkManager.maxHealth.Value;
                playerNetworkManager.currentStamina.Value = playerNetworkManager.maxStamina.Value;
                // restore focus points
                
                // play rebirth effects
                playerAnimatorManager.PlayTargetActionAnimation("Empty", false);
            }
        }

        public override IEnumerator ProcessDeathEvent(bool manuallySelectDeathAnimation = false)
        {
            if (IsOwner)
            {
                PlayerUIManager.instance.playerUIPopUpManager.SendYouDiedPopUp();
            }
            
            return base.ProcessDeathEvent(manuallySelectDeathAnimation);
            
            // Check player that are alive, if 0 respawn characters

        }

        public void SaveGameDataToCurrentCharacterData(ref CharacterSaveData currentCharacterData)
        {
            currentCharacterData.sceneIndex = SceneManager.GetActiveScene().buildIndex;
            
            currentCharacterData.characterName = playerNetworkManager.characterName.Value.ToString();
            currentCharacterData.xPosition = transform.position.x;
            currentCharacterData.yPosition = transform.position.y;
            currentCharacterData.zPosition = transform.position.z;
            
            currentCharacterData.currentHealth = playerNetworkManager.currentHealth.Value;
            currentCharacterData.currentStamina = playerNetworkManager.currentStamina.Value;

            currentCharacterData.vitality = playerNetworkManager.vitality.Value;
            currentCharacterData.endurance = playerNetworkManager.endurance.Value;
        }

        public void LoadGameDataFromCurrentCharacterData(ref CharacterSaveData currentCharacterData)
        {
            playerNetworkManager.characterName.Value = currentCharacterData.characterName;
            Vector3 myPosition = new Vector3(
                currentCharacterData.xPosition, 
                currentCharacterData.yPosition, 
                currentCharacterData.zPosition);
            transform.position = myPosition;
            
            playerNetworkManager.vitality.Value = currentCharacterData.vitality;
            playerNetworkManager.endurance.Value = currentCharacterData.endurance;
            
            // this will be moved when saving and loading is added
            playerNetworkManager.maxHealth.Value =
                playerStatsManager.CalculateHealthBasedOnVitalityLevel(playerNetworkManager.vitality.Value);
            playerNetworkManager.maxStamina.Value = 
                playerStatsManager.CalculateStaminaBasedOnEnduranceLevel(playerNetworkManager.endurance.Value);
            
            playerNetworkManager.currentHealth.Value = currentCharacterData.currentHealth;
            playerNetworkManager.currentStamina.Value = currentCharacterData.currentStamina;
            
            PlayerUIManager.instance.playerUIHudManager.SetMaxStaminaValue(playerNetworkManager.maxStamina.Value);
            PlayerUIManager.instance.playerUIHudManager.SetMaxHealthValue(playerNetworkManager.maxHealth.Value);
        }

        public void LoadOtherPlayerCharacterWhenJoiningServer()
        {
            // sync weapons
            playerNetworkManager.OnCurrentRightHandWeaponIDChange(0, playerNetworkManager.currentRightHandWeaponID.Value);
            playerNetworkManager.OnCurrentLeftHandWeaponIDChange(0, playerNetworkManager.currentLeftHandWeaponID.Value);
            
            // sync armors
            
            // lock on

            if (playerNetworkManager.isLockedOn.Value)
            {
                playerNetworkManager.OnLockOnTargetIDChange(0, playerNetworkManager.currentTargetNetworkObjectID.Value);
            }
        }

        // DEBUG DELTE LATER
        private void DebugMenu()
        {
            if (respawnCharacter)
            {
                respawnCharacter = false;
                ReviveCharacter();
            }

            if (switchRightWeapon)
            {
                switchRightWeapon = false;
                playerEquipmentManager.SwitchRightWeapon();
            }

            if (switchLeftWeapon)
            {
                switchLeftWeapon = false;
                playerEquipmentManager.SwitchLeftWeapon();
            }
        }
    }
}
