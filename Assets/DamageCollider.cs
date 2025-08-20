using System;
using System.Collections.Generic;
using UnityEngine;

namespace Moko
{
    public class DamageCollider : MonoBehaviour
    {
        [Header("Damage")]
        public float physicalDamage = 0; // in the future will be split into "Standarad", "Strike", "Slash" and "Pierce"
        public float magicDamage = 0;
        public float fireDamage = 0;
        public float lightningDamage = 0;
        public float holyDamage = 0;

        [Header("Contact Point")]
        protected Vector3 contactPoint;

        [Header("Characters Damaged")]
        protected List<CharacterManager> charactersDamaged = new List<CharacterManager>();

        private void OnTriggerEnter(Collider other)
        {
            CharacterManager damageTarget = other.GetComponent<CharacterManager>();
            if (damageTarget != null)
            {
                contactPoint = other.gameObject.GetComponent<Collider>().ClosestPointOnBounds(contactPoint);
                
                // check if we can damage this target based on friendly fire
                
                // check if target is blocking
                
                // check if target is invulnerable

                DamageTarget(damageTarget); // damage appliance
            }
        }

        protected virtual void DamageTarget(CharacterManager damageTarget)
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
            
            damageTarget.characterEffectsManager.ProcessInstantEffect(damageEffect);
        }
    }
}
