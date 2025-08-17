using UnityEngine;

namespace Moko
{
    public class PlayerManager : CharacterManager
    {
        private PlayerLocomotionManager playerLocomotionManager;
        
        protected override void Awake()
        {
            base.Awake();
            
            // Do more stuff only for player
            
            playerLocomotionManager = GetComponent<PlayerLocomotionManager>();
        }

        protected override void Update()
        {
            base.Update();

            // if we do not own this gameobject, we do not control or edit it
            if (!IsOwner) return;
            
            // Handle Movement
            playerLocomotionManager.HandleAllMovement();
        }
    }
}
