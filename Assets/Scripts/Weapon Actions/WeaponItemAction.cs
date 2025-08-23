using UnityEngine;

namespace Moko
{
    [CreateAssetMenu(menuName = "Character Actions/Weapon Actions/Test Action")]
    public class WeaponItemAction : ScriptableObject
    {
        public int actionID;
        public virtual void AttemptToPerformAction(PlayerManager playerPerformingAction,
            WeaponItem weaponPerformingAction)
        {
            // what does every waepon action have in common
            // 1. we should always keep track of which weapon is currently being used

            if (playerPerformingAction.IsOwner)
            {
                playerPerformingAction.playerNetworkManager.currentWeaponBeingUsed.Value =
                    weaponPerformingAction.itemID;
            }

            Debug.Log("THE ACTION HAS FIRED");
        }
    }
}
