using UnityEngine;
using UnityEngine.AI;

namespace Moko
{
    [CreateAssetMenu(menuName = "A.I/States/Pursue Target")]
    public class PursueTargetState : AIState
    {
        public override AIState Tick(AICharacterManager aiCharacter)
        {
            // check if we are preforming an action (if so, do nothing unitl action is complete)
            if (aiCharacter.isPerformingAction) return this;

            // check if our target is null, if we dont have a target, return to idle state
            if (aiCharacter.aiCharacterCombatManager.currentTarget == null)
                return SwitchState(aiCharacter, aiCharacter.idle);

            // make sure navmesh agent is active, if its not, enable it
            if (aiCharacter.navMeshAgent.enabled)
                aiCharacter.navMeshAgent.enabled = true;

            // if our target goes outside of the charactesr FOV, pivot to face them
            if (aiCharacter.aiCharacterCombatManager.viewableAngle < aiCharacter.aiCharacterCombatManager.minimumFOV ||
                aiCharacter.aiCharacterCombatManager.viewableAngle > aiCharacter.aiCharacterCombatManager.maximumFOV)
            {
                aiCharacter.aiCharacterCombatManager.PivotTowardsTarget(aiCharacter);
            }
            
            aiCharacter.aiCharacterLocomotionManager.RotateTowardsAgent(aiCharacter);

            // if we are within combat range of a target, switch state to combat stance state
            if (aiCharacter.aiCharacterCombatManager.distanceFromTarget <= aiCharacter.navMeshAgent.stoppingDistance)
                return SwitchState(aiCharacter, aiCharacter.combatStance);

            // if the target is not reachable, and they are far away, return home

            // PURSUE TARGET
            NavMeshPath path = new NavMeshPath();
            aiCharacter.navMeshAgent.CalculatePath(aiCharacter.aiCharacterCombatManager.currentTarget.transform.position, path);
            aiCharacter.navMeshAgent.SetPath(path);

            return this;
        }
    }
}