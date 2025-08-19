using UnityEngine;

namespace Moko
{
    public class PlayerLocomotionManager : CharacterLocomotionManager
    {
        private PlayerManager player;
        
        [HideInInspector] public float verticalMovement;
        [HideInInspector] public float horizontalMovement;
        [HideInInspector] public float moveAmount;

        private Vector3 moveDirection;
        private Vector3 targetRotationDirection;
        [SerializeField] private float walkingSpeed = 2f;
        [SerializeField] private float runningSpeed = 5f;
        [SerializeField] private float sprintingSpeed = 6.5f;
        [SerializeField] private float rotationSpeed = 15f;
        [SerializeField] private int sprintingStaminaCost = 2;

        [Header("Dodge")] 
        private Vector3 rollDirection;
        [SerializeField] private float dodgeStaminaCost = 25f;

        protected override void Awake()
        {
            base.Awake();
            
            player = GetComponent<PlayerManager>();
        }

        protected override void Update()
        {
            base.Update();

            if (player.IsOwner)
            {
                player.characterNetworkManager.verticalMovement.Value = verticalMovement;
                player.characterNetworkManager.horizontalMovement.Value = horizontalMovement;
                player.characterNetworkManager.moveAmount.Value = moveAmount;
            }
            else
            {
                verticalMovement = player.characterNetworkManager.verticalMovement.Value;
                horizontalMovement = player.characterNetworkManager.horizontalMovement.Value;
                moveAmount = player.characterNetworkManager.moveAmount.Value;
                
                // if not locked on, pass move amount
                player.playerAnimatorManager.UpdateAnimatorMovementParameters(
                    0, moveAmount, player.playerNetworkManager.isSprinting.Value);
            }
        }

        public void HandleAllMovement()
        {
            HandleGroundedMovement();
            HandleRotation();

            // Aerial Movement
            // Falling
        }

        private void GetMovementValues()
        {
            verticalMovement = PlayerInputManager.instance.verticalInput;
            horizontalMovement = PlayerInputManager.instance.horizontalInput;
            moveAmount = PlayerInputManager.instance.moveAmount;
            
            // clamp the movements
        }

        private void HandleGroundedMovement()
        {
            if (!player.canMove) return;
            
            GetMovementValues();
            
            // movedirection is based on our cameras facing perspective and our movement input
            moveDirection = PlayerCamera.instance.transform.forward * verticalMovement;
            moveDirection += PlayerCamera.instance.transform.right * horizontalMovement;
            moveDirection.Normalize();
            moveDirection.y = 0;

            if (player.playerNetworkManager.isSprinting.Value)
            {
                player.characterController.Move(moveDirection * (sprintingSpeed * Time.deltaTime));
            }
            else
            {
                if (PlayerInputManager.instance.moveAmount > 0.5f)
                {
                    // Move at a running speed
                    player.characterController.Move(moveDirection * (runningSpeed * Time.deltaTime));
                }
                else if (PlayerInputManager.instance.moveAmount <= 0.5f)
                {
                    // Move at a walking speed;
                    player.characterController.Move(moveDirection * (walkingSpeed * Time.deltaTime));
                }
            }
        }

        private void HandleRotation()
        {
            if (!player.canRotate) return;
            
            targetRotationDirection = Vector3.zero;
            targetRotationDirection = PlayerCamera.instance.cameraObject.transform.forward * verticalMovement;
            targetRotationDirection += PlayerCamera.instance.cameraObject.transform.right * horizontalMovement;
            targetRotationDirection.Normalize();
            targetRotationDirection.y = 0;

            if (targetRotationDirection == Vector3.zero)
            {
                targetRotationDirection = transform.forward;
            }

            Quaternion newRotation = Quaternion.LookRotation(targetRotationDirection);
            Quaternion targetRotation = Quaternion.Slerp(transform.rotation, newRotation, rotationSpeed * Time.deltaTime);
            transform.rotation = targetRotation;
        }

        public void HandleSprinting()
        {
            if (player.isPerformingAction)
            {
                // set sprinting to false
                player.playerNetworkManager.isSprinting.Value = false;
            }

            if (player.playerNetworkManager.currentStamina.Value <= 0)
            {
                player.playerNetworkManager.isSprinting.Value = false;
                return;
            }

            if (moveAmount >= 0.5f)  // if we are moving set sprinting to true
            {
                player.playerNetworkManager.isSprinting.Value = true;
            }
            else // if we are stationary set sprinting to false
            {
                player.playerNetworkManager.isSprinting.Value = false;
            }

            if (player.playerNetworkManager.isSprinting.Value)
            {
                player.playerNetworkManager.currentStamina.Value -= sprintingStaminaCost * Time.deltaTime;
            }
        }

        public void AttemptToPerformDodge()
        {
            if (player.isPerformingAction) return;
            if (player.playerNetworkManager.currentStamina.Value <= 0) return;
            
            // if we are moving when we attempt to dodge, we perform a roll
            if (PlayerInputManager.instance.moveAmount > 0)
            {
                rollDirection = PlayerCamera.instance.cameraObject.transform.forward *
                                PlayerInputManager.instance.verticalInput;
                rollDirection += PlayerCamera.instance.cameraObject.transform.right * 
                                 PlayerInputManager.instance.horizontalInput;
                rollDirection.y = 0;
                rollDirection.Normalize();
            
                Quaternion playerRotation = Quaternion.LookRotation(rollDirection);
                player.transform.rotation = playerRotation;

                // perform a roll animation
                player.playerAnimatorManager.PlayeTargetActionAnimation("Roll_Forward_01", true, true);
            }
            // if we are stationary, perform a backstep
            else
            {
                // perform a backstep animation
                player.playerAnimatorManager.PlayeTargetActionAnimation("Back_Step_01", true, true);
            }

            player.playerNetworkManager.currentStamina.Value -= dodgeStaminaCost;
        }
    }
}
