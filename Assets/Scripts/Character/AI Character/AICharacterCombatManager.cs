using Unity.VisualScripting;
using UnityEngine;

namespace Moko
{
    public class AICharacterCombatManager : CharacterCombatManager
    {
        [Header("Detection")] 
        [SerializeField] private float detectionRadius = 15f;
        [SerializeField] private float minimumDetectionAngle = -35f;
        [SerializeField] private float maximumDetectionAngle = 35f;
        
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
                    float viewableAngle = Vector3.Angle(targetsDirection, aiCharacter.transform.forward);

                    if (viewableAngle > minimumDetectionAngle && viewableAngle < maximumDetectionAngle)
                    {
                        // Lastly, we check for enviro blocks
                        if (Physics.Linecast(
                                aiCharacter.characterCombatManager.lockOnTransform.position,
                                targetCharacter.characterCombatManager.lockOnTransform.position, 
                                WorldUtilityManager.instance.GetEnvironmentLayers()))
                        {
                            Debug.DrawLine(aiCharacter.characterCombatManager.lockOnTransform.position, targetCharacter.characterCombatManager.lockOnTransform.position);
                            Debug.Log("BLOCKED");
                        }
                        else
                        {
                            aiCharacter.characterCombatManager.SetTarget(targetCharacter);
                        }
                    }
                }
            }
        }
    }
}
