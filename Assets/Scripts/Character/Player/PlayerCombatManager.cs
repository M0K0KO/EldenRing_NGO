using Unity.Netcode;
using UnityEngine;

namespace Moko
{
    public class PlayerCombatManager : CharacterCombatManager
    {
        private PlayerManager player;
        
        public WeaponItem currentWeaponBeingUsed;

        protected override void Awake()
        {
            base.Awake();
            
            player = GetComponent<PlayerManager>();
        }
        
        public void PerformWeaponBasedAction(WeaponItemAction weaponAction, WeaponItem weaponPerformingAction)
        {
            if (player.IsOwner)
            {
                // perform the action
                weaponAction.AttemptToPerformAction(player, weaponPerformingAction);
            
                // notify the server we have performed the action, to perform it from there perspectives also
                player.playerNetworkManager.NotifyTheServerOfWeaponActionServerRpc(
                    NetworkManager.Singleton.LocalClientId, weaponAction.actionID, weaponPerformingAction.itemID);
            }
        }

        public virtual void DrainStaminaBasedOnAttack()
        {
            if (!player.IsOwner) return;

            if (currentWeaponBeingUsed == null) return;

            float staminaDeducted = 0;

            switch (currentAttackType)
            {
                case AttackType.LightAttack01:
                    staminaDeducted = currentWeaponBeingUsed.baseStaminaCost *
                                      currentWeaponBeingUsed.lightAttackStaminaCostMultiplier;
                    break;
                default:
                    break;
            }
            
            player.playerNetworkManager.currentStamina.Value -= Mathf.RoundToInt(staminaDeducted);
        }

        public override void SetTarget(CharacterManager newTarget)
        {
            base.SetTarget(newTarget);

            if (player.IsOwner)
            {
                PlayerCamera.instance.SetLockCameraHeight();
            }
        }
    }
}
