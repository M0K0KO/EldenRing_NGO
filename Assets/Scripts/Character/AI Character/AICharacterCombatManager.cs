using Unity.VisualScripting;
using UnityEngine;

namespace Moko
{
    public class AICharacterCombatManager : CharacterCombatManager
    {
        protected AICharacterManager aiCharacter;
        
        [Header("Recovery Timer")] 
        public float actionRecoveryTimer = 0;

        [Header("Target Information")] 
        public float distanceFromTarget;
        public float viewableAngle;
        public Vector3 targetsDirection;
        
        [Header("Detection")] 
        [SerializeField] private float detectionRadius = 15f;
        public float minimumFOV = -35f;
        public float maximumFOV = 35f;

        [Header("Attack Rotation Speed")]
        public float attackRotationSpeed = 25f;


        protected override void Awake()
        {
            base.Awake();

            aiCharacter = GetComponent<AICharacterManager>();
            lockOnTransform = GetComponentInChildren<LockOnTransform>().transform;
        }

        public void FindATargetViaLineOfSight(AICharacterManager aiCharacter)
        {
            if (currentTarget != null) return;

            Collider[] colliders = Physics.OverlapSphere(
                aiCharacter.transform.position,
                detectionRadius,
                WorldUtilityManager.instance.GetCharacterLayers());

            for (int i = 0; i < colliders.Length; i++)
            {
                CharacterManager targetCharacter = colliders[i].transform.GetComponent<CharacterManager>();

                if (targetCharacter == null) continue;
                
                if (targetCharacter == aiCharacter) continue;

                if (targetCharacter.isDead.Value) continue;
                
                // Can i attack this character, if so, make them my target
                if (WorldUtilityManager.instance.CanIDamageThisTarget(character.characterGroup, targetCharacter.characterGroup))
                {
                    // if a potential target is found, it has to be in front of us
                    Vector3 targetsDirection = targetCharacter.transform.position - aiCharacter.transform.position;
                    float angleOfPotentialTarget = Vector3.Angle(targetsDirection, aiCharacter.transform.forward);

                    if (angleOfPotentialTarget > minimumFOV && angleOfPotentialTarget < maximumFOV)
                    {
                        // Lastly, we check for enviro blocks
                        if (Physics.Linecast(
                                aiCharacter.characterCombatManager.lockOnTransform.position,
                                targetCharacter.characterCombatManager.lockOnTransform.position, 
                                WorldUtilityManager.instance.GetEnvironmentLayers()))
                        {
                            Debug.DrawLine(aiCharacter.characterCombatManager.lockOnTransform.position, targetCharacter.characterCombatManager.lockOnTransform.position);
                        }
                        else
                        {
                            targetsDirection = targetCharacter.transform.position - aiCharacter.transform.position;
                            viewableAngle = WorldUtilityManager.instance.GetAngleOfTarget(transform, targetsDirection);
                            aiCharacter.characterCombatManager.SetTarget(targetCharacter);
                            PivotTowardsTarget(aiCharacter);
                        }
                    }
                }
            }
        }

        public void PivotTowardsTarget(AICharacterManager aiCharacter)
        {
            // play a pivot animation depending on viewable angle of target
            if (aiCharacter.isPerformingAction) return;

            if (viewableAngle >= 20 && viewableAngle <= 60)
            {
                aiCharacter.characterAnimatorManager.PlayTargetActionAnimation("Turn_Right_45", true);
            }
            else if (viewableAngle >= -20 && viewableAngle >= -60)
            {
                aiCharacter.characterAnimatorManager.PlayTargetActionAnimation("Turn_Left_45", true);
            }
            else if (viewableAngle >= 61 && viewableAngle <= 110)
            {
                aiCharacter.characterAnimatorManager.PlayTargetActionAnimation("Turn_Right_90", true);
            }
            else if (viewableAngle <= -61 && viewableAngle >= -110)
            {
                aiCharacter.characterAnimatorManager.PlayTargetActionAnimation("Turn_Left_90", true);
            }

            if (viewableAngle >= 110 && viewableAngle <= 145)
            {
                aiCharacter.characterAnimatorManager.PlayTargetActionAnimation("Turn_Right_135", true);
            }
            else if (viewableAngle <= -110 && viewableAngle >= -145)
            {
                aiCharacter.characterAnimatorManager.PlayTargetActionAnimation("Turn_Left_135", true);
            }

            if (viewableAngle >= 146 && viewableAngle <= 180)
            {
                aiCharacter.characterAnimatorManager.PlayTargetActionAnimation("Turn_Right_180", true);
            }
            else if (viewableAngle <= -146 && viewableAngle >= -180)
            {
                aiCharacter.characterAnimatorManager.PlayTargetActionAnimation("Turn_Left_180", true);
            }
        }

        public void RotateTowardsAgent(AICharacterManager aiCharacter)
        {
            if (aiCharacter.aiCharacterNetworkManager.isMoving.Value)
            {
                aiCharacter.transform.rotation = aiCharacter.navMeshAgent.transform.rotation;
            }
        }
        
        public void RotateTowardsTargetWhilstAttacking(AICharacterManager aiCharacter)
        {
            if (currentTarget == null) return;
            
            // 1. check if we can rotate 
            if (!aiCharacter.characterLocomotionManager.canRotate) return;
            
            if (!aiCharacter.isPerformingAction) return;
            
            // 2. rotate towards the target & a specifed roation speed during specified frames
            Vector3 targetDirection = currentTarget.transform.position - aiCharacter.transform.position;
            targetDirection.y = 0;
            targetDirection.Normalize();
            
            if (targetDirection == Vector3.zero)
                targetDirection = aiCharacter.transform.forward;
            
            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
            aiCharacter.transform.rotation = Quaternion.Slerp(aiCharacter.transform.rotation, targetRotation,
                Time.deltaTime * attackRotationSpeed);
        }
        
        public void HandleActionRecovery(AICharacterManager aiCharacter)
        {
            if (actionRecoveryTimer > 0)
            {
                if (!aiCharacter.isPerformingAction)
                {
                    actionRecoveryTimer -= Time.deltaTime;
                }
            }
        }
    }
}
