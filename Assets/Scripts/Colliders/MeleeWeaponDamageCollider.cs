using UnityEngine;
using UnityEngine.Serialization;

namespace Moko
{
    public class MeleeWeaponDamageCollider : DamageCollider
    {
        [Header("Attacking Character")]
        public CharacterManager characterCausingDamage;

        [Header("Weapon Attack Modifiers")] 
        public float light_Attack_01_Modifier;
        public float light_Attack_02_Modifier;
        public float heavy_Attack_01_Modifier;
        public float heavy_Attack_02_Modifier;
        public float charge_Attack_01_Modifier;
        public float charge_Attack_02_Modifier;

        protected override void Awake()
        {
            base.Awake();

            if (damageCollider == null)
            {
                damageCollider = GetComponent<Collider>();
            }
            
            damageCollider.enabled = false;
        }

        protected override void OnTriggerEnter(Collider other)
        {
            CharacterManager damageTarget = other.GetComponentInParent<CharacterManager>();
            
            if (damageTarget != null)
            {
                if (damageTarget == characterCausingDamage) return; // dont damage ourselves
                
                contactPoint = other.gameObject.GetComponent<Collider>().ClosestPointOnBounds(contactPoint);
                
                // check if we can damage this target based on friendly fire
                
                // check if target is blocking
                
                // check if target is invulnerable

                DamageTarget(damageTarget); // damage appliance
            }
        }

        protected override void DamageTarget(CharacterManager damageTarget)
        {
            // we don't want to damage the same target more than once in a single attack
            // add them to a list that checks before applying damage

            if (charactersDamaged.Contains(damageTarget)) return; // already damaged
            
            charactersDamaged.Add(damageTarget);

            TakeDamageEffect damageEffect = Instantiate(WorldCharacterEffectsManager.instance.takeDamageEffect);
            damageEffect.physicalDamage = physicalDamage;
            damageEffect.magicDamage = magicDamage;
            damageEffect.fireDamage = fireDamage;
            damageEffect.lightningDamage = lightningDamage;
            damageEffect.holyDamage = holyDamage;
            damageEffect.contactPoint = contactPoint;
            damageEffect.angleHitFrom = Vector3.SignedAngle(
                characterCausingDamage.transform.position, damageTarget.transform.forward, Vector3.up);

            switch (characterCausingDamage.characterCombatManager.currentAttackType)
            {
                case AttackType.LightAttack01:
                    ApplyAttackDamageModifiers(light_Attack_01_Modifier, damageEffect);
                    break;
                case AttackType.LightAttack02:
                    ApplyAttackDamageModifiers(light_Attack_02_Modifier, damageEffect);
                    break;
                case AttackType.HeavyAttack01:
                    ApplyAttackDamageModifiers(heavy_Attack_01_Modifier, damageEffect);
                    break;
                case AttackType.HeavyAttack02:
                    ApplyAttackDamageModifiers(heavy_Attack_02_Modifier, damageEffect);
                    break;
                case AttackType.ChargedAttack01:
                    ApplyAttackDamageModifiers(charge_Attack_01_Modifier, damageEffect);
                    break;
                case AttackType.ChargedAttack02:
                    ApplyAttackDamageModifiers(charge_Attack_02_Modifier, damageEffect);
                    break;
                default:
                    break;
            }
            
            //damageTarget.characterEffectsManager.ProcessInstantEffect(damageEffect);

            if (characterCausingDamage.IsOwner)
            {
                // send a damage request across network
                damageTarget.characterNetworkManager.NotifyTheServerOfCharacterDamageServerRpc(
                    damageTarget.NetworkObjectId,
                    characterCausingDamage.NetworkObjectId,
                    damageEffect.physicalDamage,
                    damageEffect.magicDamage,
                    damageEffect.fireDamage,
                    damageEffect.lightningDamage,
                    damageEffect.holyDamage,
                    damageEffect.poiseDamage,
                    damageEffect.angleHitFrom,
                    damageEffect.contactPoint.x,
                    damageEffect.contactPoint.y,
                    damageEffect.contactPoint.z);
            }
        }

        private void ApplyAttackDamageModifiers(float modifier, TakeDamageEffect damage)
        {
            damage.physicalDamage *= modifier;
            damage.magicDamage *= modifier;
            damage.fireDamage *= modifier;
            damage.lightningDamage *= modifier;
            damage.holyDamage *= modifier;
            damage.poiseDamage *= modifier;
            
            // if attack is a fully charged heavy, multiply by full charge modifier after normal modifier have been calculated
        }
    }
}
