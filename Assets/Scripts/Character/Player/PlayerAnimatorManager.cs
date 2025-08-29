using System;
using UnityEngine;

namespace Moko
{
    public class PlayerAnimatorManager : CharacterAnimatorManager
    {
        private PlayerManager player;

        protected override void Awake()
        {
            base.Awake();
            
            player = GetComponent<PlayerManager>();
        }

        // ANIMTAION EVENT CALLS
        
        private void OnAnimatorMove()
        {
            if (applyRootMotion)
            {
                Vector3 velocity = player.animator.deltaPosition;
                player.characterController.Move(velocity);
                player.transform.rotation *= player.animator.deltaRotation;
            }
        }
        
        public override void EnableCanDoCombo()
        {
            if (player.playerNetworkManager.isUsingRightHand.Value)
            {
                player.playerCombatManager.canComboWithMainHandWeapon = true;
            }
            else
            {
                // Enable off hand combo
            }
        }

        public override void DisableCanDoCombo()
        {
            player.playerCombatManager.canComboWithMainHandWeapon = false;
            // Enable off hand combo
        }
    }
}
