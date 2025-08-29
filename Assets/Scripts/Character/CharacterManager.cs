using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Multiplayer.Tools.NetStatsMonitor;
using UnityEngine;
using Unity.Netcode;
using UnityEditor;

namespace Moko
{
    public class CharacterManager : NetworkBehaviour
    {
        [Header("Status")]
        public NetworkVariable<bool> isDead = 
            new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        
        [HideInInspector] public CharacterController characterController;
        [HideInInspector] public Animator animator;
        
        [HideInInspector] public CharacterNetworkManager characterNetworkManager;
        [HideInInspector] public CharacterEffectsManager characterEffectsManager;
        [HideInInspector] public CharacterAnimatorManager characterAnimatorManager;
        [HideInInspector] public CharacterCombatManager characterCombatManager;
        [HideInInspector] public CharacterSoundFXManager characterSoundFXManager;
        [HideInInspector] public CharacterLocomotionManager characterLocomotionManager;
        
        [Header("Character Group")]
        public CharacterGroup characterGroup;

        [Header("Flags")]
        public bool isPerformingAction = false;

        
        protected virtual void Awake()
        {
            DontDestroyOnLoad(gameObject);
            
            characterController = GetComponent<CharacterController>();
            animator = GetComponent<Animator>();
            
            characterNetworkManager = GetComponent<CharacterNetworkManager>();
            characterEffectsManager = GetComponent<CharacterEffectsManager>();
            characterAnimatorManager = GetComponent<CharacterAnimatorManager>();
            characterCombatManager = GetComponent<CharacterCombatManager>();
            characterSoundFXManager = GetComponent<CharacterSoundFXManager>();
            characterLocomotionManager = GetComponent<CharacterLocomotionManager>();
        }

        protected virtual void Start()
        {
            IgnoreMyOwnColliders();
        }

        protected virtual void Update()
        {
            animator.SetBool("isGrounded", characterLocomotionManager.isGrounded);
            
            if (IsOwner) // has an authority to change position
            {
                characterNetworkManager.networkPosition.Value = transform.position;
                characterNetworkManager.networkRotation.Value = transform.rotation;
            }
            else // make other player's object follow their networkPosition
            {
                // POSITION
                transform.position = Vector3.SmoothDamp(
                    transform.position,
                    characterNetworkManager.networkPosition.Value, 
                    ref characterNetworkManager.networkPositionVelocity,
                    characterNetworkManager.networkPositionSmoothTime);
                
                // ROTATION
                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    characterNetworkManager.networkRotation.Value,
                    characterNetworkManager.networkRotationSmoothTime);
            }
        }

        protected virtual void FixedUpdate()
        {
            
        }

        protected virtual void LateUpdate()
        {
            
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            characterNetworkManager.OnIsMovingChanged(false, characterNetworkManager.isMoving.Value);
            characterNetworkManager.isMoving.OnValueChanged += characterNetworkManager.OnIsMovingChanged;
        }
        
        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();

            characterNetworkManager.isMoving.OnValueChanged -= characterNetworkManager.OnIsMovingChanged;
        }

        public virtual IEnumerator ProcessDeathEvent(bool manuallySelectDeathAnimation = false)
        {
            if (IsOwner)
            {
                characterNetworkManager.currentHealth.Value = 0;
                isDead.Value = true;
                
                //reset any flags here that need to be reset (NOTHING YET)
                
                // if we are not grounded, play an aerial death animation (NOT YET)

                if (!manuallySelectDeathAnimation)
                {
                    characterAnimatorManager.PlayTargetActionAnimation("Dead_01", true);
                }
            }
            
            // Play Death SFX

            yield return new WaitForSeconds(5f);
            
            // Award players with runes
            
            // disable character
        }

        public virtual void ReviveCharacter()
        {
            
        }

        protected virtual void IgnoreMyOwnColliders()
        {
            Collider characterControllerCollider = GetComponent<Collider>();
            Collider[] damageableCharacerColliders = GetComponentsInChildren<Collider>();
            
            List<Collider> ignoreColliders = new List<Collider>();

            foreach (var collider in damageableCharacerColliders)
            {
                ignoreColliders.Add(collider);
            }

            ignoreColliders.Add(characterControllerCollider);

            foreach (var collider in ignoreColliders)
            {
                foreach (var otherCollider in ignoreColliders)
                {
                    Physics.IgnoreCollision(collider, otherCollider, true);
                }
            }
        }
    }
}
