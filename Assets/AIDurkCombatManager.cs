using System.Collections.Generic;
using UnityEngine;

namespace Moko
{
    public class AIDurkCombatManager : AICharacterCombatManager
    {
        [Header("Damage Collider")]
        [SerializeField] DurkClubDamageCollider clubDamageCollider;
        [SerializeField] private Transform durksStompingFoot;
        [SerializeField] private float stompAttackAOERadius = 1.5f;
        
        [Header("Damage")] 
        [SerializeField] private int baseDamage = 25;
        [SerializeField] private float attack01DamageModifier = 1.0f;
        [SerializeField] private float attack02DamageModifier = 1.4f;
        [SerializeField] private float attack03DamageModifier = 1.6f;
        [SerializeField] private float stompDamage = 25f;

        public void SetAttack01Damage()
        {
            clubDamageCollider.physicalDamage = baseDamage * attack01DamageModifier;
        }
        
        public void SetAttack02Damage()
        {
            clubDamageCollider.physicalDamage = baseDamage * attack02DamageModifier;
        }
        
        public void SetAttack03Damage()
        {
            clubDamageCollider.physicalDamage = baseDamage * attack03DamageModifier;
        }

        public void OpenClubDamageCollider()
        {
            aiCharacter.characterSoundFXManager.PlayAttackGrunt();
            clubDamageCollider.EnableDamageCollider();
        }

        public void CloseClubDamageCollider()
        {
            clubDamageCollider.DisableDamageCollider();
        }

        public void ActivateDurkStomp()
        {
            Collider[] colliders = Physics.OverlapSphere(
                durksStompingFoot.position, 
                stompAttackAOERadius,
                WorldUtilityManager.instance.GetCharacterLayers());
            
            List<CharacterManager> charactersDamaged = new List<CharacterManager>();

            foreach (var collider in colliders)
            {
                CharacterManager character = collider.GetComponentInParent<CharacterManager>();

                if (character != null)
                {
                    if (charactersDamaged.Contains(character)) continue;
                    charactersDamaged.Add(character);
                    
                    if (character.IsOwner)
                    {
                        // check for block
                    
                        TakeDamageEffect damageEffect = Instantiate(WorldCharacterEffectsManager.instance.takeDamageEffect);
                        damageEffect.physicalDamage = stompDamage;
                        damageEffect.poiseDamage = stompDamage;
                    
                        character.characterEffectsManager.ProcessInstantEffect(damageEffect);
                    
                    }
                }
            }
        }

        public override void PivotTowardsTarget(AICharacterManager aiCharacter)
        {
            // play a pivot animation depending on viewable angle of target
            if (aiCharacter.isPerformingAction) return;


            else if (viewableAngle >= 61 && viewableAngle <= 110)
            {
                aiCharacter.characterAnimatorManager.PlayTargetActionAnimation("Turn_Right_90", true);
            }
            else if (viewableAngle <= -61 && viewableAngle >= -110)
            {
                aiCharacter.characterAnimatorManager.PlayTargetActionAnimation("Turn_Left_90", true);
            }
            else if (viewableAngle >= 146 && viewableAngle <= 180)
            {
                aiCharacter.characterAnimatorManager.PlayTargetActionAnimation("Turn_Right_180", true);
            }
            else if (viewableAngle <= -146 && viewableAngle >= -180)
            {
                aiCharacter.characterAnimatorManager.PlayTargetActionAnimation("Turn_Left_180", true);
            }
        }
    }
}
