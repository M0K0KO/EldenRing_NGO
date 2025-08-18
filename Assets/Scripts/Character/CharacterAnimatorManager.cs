using Unity.Netcode;
using UnityEngine;

namespace Moko
{
    public class CharacterAnimatorManager : MonoBehaviour
    {
        private CharacterManager character;

        private readonly int vertical = Animator.StringToHash("Vertical");
        private readonly int horizontal = Animator.StringToHash("Horizontal");
        
        protected virtual void Awake()
        {
            character = GetComponent<CharacterManager>();
        }
        
        public void UpdateAnimatorMovementParameters(float horizontalMovement, float verticalMovement, bool isSprinting)
        {
            float horizontalAmount = horizontalMovement;
            float verticalAmount = verticalMovement;

            if (isSprinting)
            {
                verticalAmount = 2;
            }
            
            character.animator.SetFloat(horizontal, horizontalAmount, 0.1f, Time.deltaTime);
            character.animator.SetFloat(vertical, verticalAmount, 0.1f, Time.deltaTime);
        }

        public virtual void PlayeTargetActionAnimation
            (string targetAnimation,
                bool isPerformingAction,
                bool applyRootMotion = true,
                bool canRotate = false, 
                bool canMove = false)
        {
            character.applyRootMotion = applyRootMotion;
            character.animator.CrossFade(targetAnimation, 0.2f);
            
            // can be used to stop character from ateempting new actions
            // [EXAMPLE] if you get damaged, and begin performing a damage animation
            // this flag will turn true if you are stunned
            // then we can check for this before attempting new actions
            character.isPerformingAction = isPerformingAction;
            character.canRotate = canRotate;
            character.canMove = canMove;
            
            // Tell the server we played an animation, and to play that animation for everybody else present
            character.characterNetworkManager.NotifyTheServerofActionAnimationServerRpc(
                NetworkManager.Singleton.LocalClientId,
                targetAnimation, 
                applyRootMotion);
        }
    }
}
