using System;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.Serialization;
using UnityEngine.SceneManagement;

namespace Moko
{
    public class PlayerInputManager : MonoBehaviour
    {
        public static PlayerInputManager instance;
        public PlayerManager player;

        // 1. find a way to read the values
        // 2. move character based on those values

        private PlayerControls playerControls;

        [Header("CAMERA MOVEMENT INPUT")] [SerializeField]
        private Vector2 camera_Input;

        public float cameraVertical_Input;
        public float cameraHorizontal_Input;

        [Header("LOCK ON INPUT")] 
        [SerializeField] private bool lock_On_Input;
        [SerializeField] private bool lockOn_Left_Input;
        [SerializeField] private bool lockOn_Right_Input;
        private Coroutine lockOnCoroutine;

        [Header("PLAYER MOVEMENT INPUT")] [SerializeField]
        private Vector2 movementInput;

        public float vertical_Input;
        public float horizontal_Input;
        public float moveAmount;

        [Header("PLAYER ACTION INPUT")] [SerializeField]
        private bool dodge_Input = false;

        [SerializeField] private bool sprint_Input = false;
        [SerializeField] private bool jump_Input = false;
        [SerializeField] private bool LMB_Input = false;


        private void Awake()
        {
            if (instance == null) instance = this;
            else Destroy(gameObject);
        }

        private void Start()
        {
            DontDestroyOnLoad(gameObject);

            // when the scene changes, run this logic
            SceneManager.activeSceneChanged += OnSceneChange;

            instance.enabled = false;

            if (playerControls != null)
            {
                playerControls.Disable();
            }
        }

        private void OnSceneChange(Scene oldScene, Scene newScene)
        {
            // if we are loading into our world scene, enable our players controls
            if (newScene.buildIndex == WorldSaveGameManager.instance.GetWorldSceneIndex())
            {
                instance.enabled = true;

                if (playerControls != null)
                {
                    playerControls.Enable();
                }
            }
            // otherwise we must be at the main menu, disable our players controls
            // this is so our player cant move around if we enter things like a character creation menu etc..
            else
            {
                instance.enabled = false;

                if (playerControls != null)
                {
                    playerControls.Disable();
                }
            }
        }

        private void OnEnable()
        {
            if (playerControls == null)
            {
                playerControls = new PlayerControls();

                playerControls.PlayerMovement.Movement.performed += i => movementInput = i.ReadValue<Vector2>();
                playerControls.PlayerCamera.Movement.performed += i => camera_Input = i.ReadValue<Vector2>();
                playerControls.PlayerActions.Dodge.performed += i => dodge_Input = true;
                playerControls.PlayerActions.Jump.performed += i => jump_Input = true;
                playerControls.PlayerActions.LMB.performed += i => LMB_Input = true;
                
                // LOCK ON
                playerControls.PlayerActions.LockOn.performed += i => lock_On_Input = true;
                playerControls.PlayerActions.SeekLeftLockOnTarget.performed += i => lockOn_Left_Input = true;
                playerControls.PlayerActions.SeekRightLockOnTarget.performed += i => lockOn_Right_Input = true;

                playerControls.PlayerActions.Sprint.performed += i => sprint_Input = true;
                playerControls.PlayerActions.Sprint.canceled += i => sprint_Input = false;
            }

            playerControls.Enable();
        }

        private void OnDestroy()
        {
            // if we destroy this object, unsubscribe from this event
            SceneManager.activeSceneChanged -= OnSceneChange;
        }

        // if we minimize or lower the window, stop adjusting inputs
        private void OnApplicationFocus(bool focus)
        {
            if (enabled)
            {
                if (focus)
                {
                    playerControls.Enable();
                }
                else
                {
                    playerControls.Disable();
                }
            }
        }

        private void Update()
        {
            HandleAllInputs();
        }

        private void HandleAllInputs()
        {
            HandleLockOnInput();
            HandleLockOnSwitchTargetInput();
            HandlePlayerMovementInput();
            HandleCameraMovementInput();
            HandleDodgeInput();
            HandleSprintInput();
            HandleJumpInput();
            HandleLMBInput();
        }

        // LOCK ON

        private void HandleLockOnInput()
        {
            if (player.playerNetworkManager.isLockedOn.Value)
            {
                if (player.playerCombatManager.currentTarget == null) return;
                
                if (player.playerCombatManager.currentTarget.isDead.Value)
                {
                    player.playerNetworkManager.isLockedOn.Value = false;
                    
                    // Attempt to find new target
                    if (lockOnCoroutine != null) StopCoroutine(lockOnCoroutine);
                    lockOnCoroutine = StartCoroutine(PlayerCamera.instance.WaitThenFindNewTarget());
                }
            }

            if (lock_On_Input && player.playerNetworkManager.isLockedOn.Value)
            {
                lock_On_Input = false;
                PlayerCamera.instance.ClearLockOnTargets();
                player.playerNetworkManager.isLockedOn.Value = false;
                // Disable lock on
                return;
            }

            if (lock_On_Input && !player.playerNetworkManager.isLockedOn.Value)
            {
                lock_On_Input = false;
                
                // if we are aiming using ranged weapons, return (dont allow lock on)

                PlayerCamera.instance.HandleLocatingLockOnTargets();

                if (PlayerCamera.instance.nearestLockOnTarget != null)
                {
                    // set the target as our current target
                    player.playerCombatManager.SetTarget(PlayerCamera.instance.nearestLockOnTarget);
                    player.playerNetworkManager.isLockedOn.Value = true;
                }
            }
        }

        private void HandleLockOnSwitchTargetInput()
        {
            if (lockOn_Left_Input)
            {
                lockOn_Left_Input = false;

                if (player.playerNetworkManager.isLockedOn.Value)
                {
                    PlayerCamera.instance.HandleLocatingLockOnTargets();

                    if (PlayerCamera.instance.leftLockOnTarget != null)
                    {
                        player.playerCombatManager.SetTarget(PlayerCamera.instance.leftLockOnTarget);
                    }
                }
            }
            
            if (lockOn_Right_Input)
            {
                lockOn_Right_Input = false;

                if (player.playerNetworkManager.isLockedOn.Value)
                {
                    PlayerCamera.instance.HandleLocatingLockOnTargets();

                    if (PlayerCamera.instance.rightLockOnTarget != null)
                    {
                        player.playerCombatManager.SetTarget(PlayerCamera.instance.rightLockOnTarget);
                    }
                }
            }
        }

        // MOVEMENT

        private void HandlePlayerMovementInput()
        {
            vertical_Input = movementInput.y;
            horizontal_Input = movementInput.x;

            // return the absolute number
            moveAmount = Mathf.Clamp01(Mathf.Abs(vertical_Input) + Mathf.Abs(horizontal_Input));

            // we clamp the values, so they are 0, 0.5 or 1
            if (moveAmount <= 0.5 && moveAmount > 0)
            {
                moveAmount = 0.5f;
            }
            else if (moveAmount > 0.5f && moveAmount <= 1)
            {
                moveAmount = 1;
            }

            if (!player) return;


            if (!player.playerNetworkManager.isLockedOn.Value || player.playerNetworkManager.isSprinting.Value)
            {
                // not strafing
                player.playerAnimatorManager.UpdateAnimatorMovementParameters(
                    0, moveAmount, player.playerNetworkManager.isSprinting.Value);
            }
            else
            {
                player.playerAnimatorManager.UpdateAnimatorMovementParameters(
                    horizontal_Input, vertical_Input, player.playerNetworkManager.isSprinting.Value);
            }
        }

        private void HandleCameraMovementInput()
        {
            cameraVertical_Input = camera_Input.y;
            cameraHorizontal_Input = camera_Input.x;
        }

        // ACTION

        private void HandleDodgeInput()
        {
            if (dodge_Input)
            {
                dodge_Input = false;

                player.playerLocomotionManager.AttemptToPerformDodge();
                // return if ui or menu is open
                // perform a dodge
            }
        }

        private void HandleSprintInput()
        {
            if (sprint_Input)
            {
                // Handle Sprinting
                player.playerLocomotionManager.HandleSprinting();
            }
            else
            {
                player.playerNetworkManager.isSprinting.Value = false;
            }
        }

        private void HandleJumpInput()
        {
            if (jump_Input)
            {
                jump_Input = false;

                player.playerLocomotionManager.AttemptToPerformJump();
            }
        }

        private void HandleLMBInput()
        {
            if (LMB_Input)
            {
                LMB_Input = false;

                // if we have a ui window open, return and do nothing

                player.playerNetworkManager.SetCharacterActionHand(true);

                // TODO : if two handing, use the two handed action

                player.playerCombatManager.PerformWeaponBasedAction(
                    player.playerInventoryManager.currentRightHandWeapon.oh_LMB_Action,
                    player.playerInventoryManager.currentRightHandWeapon);
            }
        }
    }
}