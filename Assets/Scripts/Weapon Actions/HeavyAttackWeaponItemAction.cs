using UnityEngine;

namespace Moko
{
    [CreateAssetMenu(menuName = "Character Actions/Weapon Actions/Heavy Attack Action")]
    public class HeavyAttackWeaponItemAction : WeaponItemAction
    {
        [SerializeField] private string heavy_Attack_01 = "Main_Heavy_Attack_01"; 
        [SerializeField] private string heavy_Attack_02 = "Main_Heavy_Attack_02"; 
        
        public override void AttemptToPerformAction(PlayerManager playerPerformingAction,
            WeaponItem weaponPerformingAction)
        {
            base.AttemptToPerformAction(playerPerformingAction, weaponPerformingAction);
            
            if (!playerPerformingAction.IsOwner) return;

            if (playerPerformingAction.playerNetworkManager.currentStamina.Value <= 0) return;

            if (!playerPerformingAction.isGrounded) return;

            PerformHeavyAttack(playerPerformingAction, weaponPerformingAction);
        }

        private void PerformHeavyAttack(PlayerManager playerPerformingAction, WeaponItem weaponPerformingAction)
        {
            // if we are attacking currently, and we can combo, perform the combo attack
            if (playerPerformingAction.playerCombatManager.canComboWithMainHandWeapon &&
                playerPerformingAction.isPerformingAction)
            {
                playerPerformingAction.playerCombatManager.canComboWithMainHandWeapon = false;
                
                // Perform an attack based on the previous attack we just played
                if (playerPerformingAction.playerCombatManager.lastAttackAnimationPerformed == heavy_Attack_01)
                {
                    playerPerformingAction.playerAnimatorManager.PlayTargetAttackActionAnimation(
                        AttackType.HeavyAttack02, heavy_Attack_02, true);
                }
                else
                {
                    playerPerformingAction.playerAnimatorManager.PlayTargetAttackActionAnimation(
                        AttackType.HeavyAttack01, heavy_Attack_01, true);
                }
            }
            // otherwise, if we are not already attacking, just perform a regular attack
            else if (!playerPerformingAction.isPerformingAction)
            {            
                playerPerformingAction.playerAnimatorManager.PlayTargetAttackActionAnimation(
                    AttackType.HeavyAttack01, heavy_Attack_01, true);
            }
        }
    }
}
