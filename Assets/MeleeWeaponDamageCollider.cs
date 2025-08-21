using UnityEngine;

namespace Moko
{
    public class MeleeWeaponDamageCollider : DamageCollider
    {
        [Header("Attacking Character")]
        public CharacterManager characterCasuingDamage;
    }
}
