using Unity.Netcode;
using UnityEngine;
using UnityEngine.Serialization;

namespace Moko
{
    public class CharacterNetworkManager : NetworkBehaviour
    {
        private CharacterManager character;
        
        [Header("Position")] 
        public NetworkVariable<Vector3> networkPosition =
            new NetworkVariable<Vector3>(
                Vector3.zero,
                NetworkVariableReadPermission.Everyone,
                NetworkVariableWritePermission.Owner);
        public NetworkVariable<Quaternion> networkRotation =
            new NetworkVariable<Quaternion>(
                Quaternion.identity,
                NetworkVariableReadPermission.Everyone,
                NetworkVariableWritePermission.Owner);
        public Vector3 networkPositionVelocity;
        public float networkPositionSmoothTime = 0.001f;
        public float networkRotationSmoothTime = 0.01f;
        
        [Header("Animator")]
        public NetworkVariable<float> horizontalMovement =
            new NetworkVariable<float>(
                0,
                NetworkVariableReadPermission.Everyone, 
                NetworkVariableWritePermission.Owner);       
        public NetworkVariable<float> verticalMovement =
            new NetworkVariable<float>(
                0,
                NetworkVariableReadPermission.Everyone, 
                NetworkVariableWritePermission.Owner);
        public NetworkVariable<float> moveAmount =
            new NetworkVariable<float>(
                0,
                NetworkVariableReadPermission.Everyone, 
                NetworkVariableWritePermission.Owner);

        [Header("Flags")]
        public NetworkVariable<bool> isSprinting =
            new NetworkVariable<bool>(
                false,
                NetworkVariableReadPermission.Everyone,
                NetworkVariableWritePermission.Owner);
        
        [Header("Stats")]
        public NetworkVariable<int> endurance =
            new NetworkVariable<int>(
                10,
                NetworkVariableReadPermission.Everyone, 
                NetworkVariableWritePermission.Owner);
        public NetworkVariable<float> currentStamina =
            new NetworkVariable<float>(
                0,
                NetworkVariableReadPermission.Everyone, 
                NetworkVariableWritePermission.Owner);
        public NetworkVariable<int> maxStamina =
            new NetworkVariable<int>(
                0,
                NetworkVariableReadPermission.Everyone, 
                NetworkVariableWritePermission.Owner);

        protected virtual void Awake()
        {
            character = GetComponent<CharacterManager>();
        }

        [ServerRpc]
        public void NotifyTheServerofActionAnimationServerRpc(ulong clientId, string animationID, bool applyRootMotion)
        {
            if (IsServer)
            {
                PlayActionAnimationForAllClientsClientRpc(clientId, animationID, applyRootMotion);
            }
        }

        [ClientRpc]
        public void PlayActionAnimationForAllClientsClientRpc(ulong clientID, string animationID, bool applyRootMotion)
        {
            // make sure to not run the function on the character who sent it
            if (clientID != NetworkManager.Singleton.LocalClientId)
            {
                PerformActionAnimationFromServer(animationID, applyRootMotion);
            }
        }

        private void PerformActionAnimationFromServer(string animationID, bool applyRootMotion)
        {
            character.applyRootMotion = applyRootMotion;
            character.animator.CrossFade(animationID, 0.2f);
        }
    }
}
