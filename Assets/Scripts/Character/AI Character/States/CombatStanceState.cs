using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Moko
{
    [CreateAssetMenu(menuName = "A.I/States/Combat Stance")]
    public class CombatStanceState : AIState
    {
        // 1. select an attack for the attack state, depending on distance and angle of target in relation to character
        // 2. process any combat logic here whilst waiting to attack(blocking, strafing, dodging etc..)
        // 3. if target moves out of combat range, switch to pursue target
        // 4. if target is no longer present, switch to idle state

        [Header("Attacks")] 
        public List<AICharacterAttackAction> aiCharacterAttacks; // a list of all possible attacks this character can do
        protected List<AICharacterAttackAction> potentialAttacks; // all attakcs possible in this situation
        private AICharacterAttackAction chosenAttack;
        private AICharacterAttackAction previousAttack;
        protected bool hasAttack = false;

        [Header("Combo")] 
        [SerializeField] protected bool canPerformCombo = false; // if the character can perform a combo, after initial attack
        [SerializeField] protected int chanceToPerformCombo = 25; // the chance(%) of the character to perform a combo on the next attack
        protected bool hasRolledForComboChance = false; // if we have already rolled for the chance during this state

        [Header("Engagement Distance")]
        public float maximumEngagementDistance = 5f; // the distance we have to be away from the target before we enter the pursue target state

        public override AIState Tick(AICharacterManager aiCharacter)
        {
            if (aiCharacter.isPerformingAction) return this;
            
            if (!aiCharacter.navMeshAgent.enabled)
                aiCharacter.navMeshAgent.enabled = true;
            
            // if you want the ai character to face and turn towards its target when its outside it's fov include this
            if (!aiCharacter.aiCharacterNetworkManager.isMoving.Value)
            {
                if (aiCharacter.aiCharacterCombatManager.viewableAngle < -30 ||
                    aiCharacter.aiCharacterCombatManager.viewableAngle > 30)
                    aiCharacter.aiCharacterCombatManager.PivotTowardsTarget(aiCharacter);
            }
            
            // rotate to face our target
            aiCharacter.aiCharacterCombatManager.RotateTowardsAgent(aiCharacter);
            
            // if our target is no longer present, switch back to idle
            if (aiCharacter.aiCharacterCombatManager.currentTarget == null)
                return SwitchState(aiCharacter, aiCharacter.idle);

            // if we do not have an attack, get one
            if (!hasAttack)
            {
                GetNewAttack(aiCharacter);
            }
            else
            {
                aiCharacter.attack.currentAttack = chosenAttack;
                // roll for combo chance
                return SwitchState(aiCharacter, aiCharacter.attack);
            }

            // if we are outside of the combat engagement distance, switch to pursue target state
            if (aiCharacter.aiCharacterCombatManager.distanceFromTarget > maximumEngagementDistance)
                return SwitchState(aiCharacter, aiCharacter.pursueTarget);
            
            NavMeshPath path = new NavMeshPath();
            aiCharacter.navMeshAgent.CalculatePath(aiCharacter.aiCharacterCombatManager.currentTarget.transform.position, path);
            aiCharacter.navMeshAgent.SetPath(path);

            return this;
        }

        protected virtual void GetNewAttack(AICharacterManager aiCharacter)
        {
            potentialAttacks = new List<AICharacterAttackAction>();
            
            foreach (var potentialAttack in aiCharacterAttacks)
            {
                if (potentialAttack.minimumAttackDistance > aiCharacter.aiCharacterCombatManager.distanceFromTarget) 
                    continue; // too close

                if (potentialAttack.maximumAttackDistance < aiCharacter.aiCharacterCombatManager.distanceFromTarget)
                    continue; // too far

                // if the target is outside FOV
                if (potentialAttack.minimumAttackAngle > aiCharacter.aiCharacterCombatManager.viewableAngle)
                    continue;
                if (potentialAttack.maximumAttackAngle < aiCharacter.aiCharacterCombatManager.viewableAngle)
                    continue;

                potentialAttacks.Add(potentialAttack);
            }

            if (potentialAttacks.Count <= 0) return;

            var totalWeight = 0;

            foreach (var attack in potentialAttacks)
            {
                totalWeight += attack.attackWeight;
            }
            
            var randomWeightValue = Random.Range(1, totalWeight + 1);
            var processedWeight = 0;

            foreach (var attack in potentialAttacks)
            {
                processedWeight += attack.attackWeight;

                if (randomWeightValue <= processedWeight)
                {
                    chosenAttack = attack;
                    previousAttack = chosenAttack;
                    hasAttack = true;
                    return;
                }
            }
        }

        protected virtual bool RollForOutComeChance(int outcomeChance)
        {
            bool outcomeWillBePerformed = false;

            int randomPercentage = Random.Range(0, 100);

            if (randomPercentage < chanceToPerformCombo)
            {
                outcomeWillBePerformed = true;
            }
            
            return outcomeWillBePerformed;
        }

        protected override void ResetStateFlags(AICharacterManager aiCharacter)
        {
            base.ResetStateFlags(aiCharacter);

            hasAttack = false;
            hasRolledForComboChance = false;
        }
    }
}