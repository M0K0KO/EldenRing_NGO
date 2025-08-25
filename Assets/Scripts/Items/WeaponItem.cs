using UnityEngine;

namespace Moko
{
    public class WeaponItem : Item
    {
        // Animator controller override (Change Attack Animations based on weapon currently using

        [Header("Weapon Model")] 
        public GameObject weaponModel;

        [Header("Weapon Requirements")] 
        public int strengthREQ = 0;
        public int dexREQ = 0;
        public int intREQ = 0;
        public int faithREQ = 0;

        [Header("Weapon Base Damage")] 
        public int physicalDamage = 0;
        public int magicDamage = 0;
        public int fireDamage = 0;
        public int holyDamage = 0;
        public int ligthningDamage = 0;
        
        // weapon gaurd absorptions (blocking power)

        [Header("Weapon Base Poise Damage")] 
        public float poiseDamage = 10;
        // offensive poise Bonus when attacking
        
        [Header("Attack Modifiers")]
        public float light_Attack_01_Modifier = 1.1f;
        public float heavy_Attack_01_Modifier = 1.4f;
        public float charge_Attack_01_Modifier = 2.0f;
        // CRITICAL DAMAGE MODIFIER ETC

        [Header("Stamina Costs")] 
        public int baseStaminaCost = 20;
        public float lightAttackStaminaCostMultiplier = 0.9f;
        // RUNNING ATTACK STAMINA COST MODIFIER
        // LIGHT ATTACK STAMINA COST MODIFIER
        // HEAVY ATTACK STAMINA COST MODIFIER ETC
        
        // item based actions
        [Header("Actions")] 
        public WeaponItemAction oh_LMB_Action; // one handed, left mouse button, action
        public WeaponItemAction oh_BMB_Action; // one handed, back mouse button, action

        // ASH OF WAR

        // blocking sounds
    }
}
