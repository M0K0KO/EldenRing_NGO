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
        
        // weapon garud absorptions (blocking power)

        [Header("Weapon Base Poise Damage")] 
        public float poiseDamage = 10;
        // offensive poise Bonus when attacking
        
        // Weapon Modifiers
        // LIGTH ATTACK MODIFIERS
        // HEAVY ATTACK MODIFIERS
        // CRITICAL DAMAGE MODIFIER ETC

        [Header("Stamina Costs")] 
        public int baseStaminaCost = 20;
        // RUNNING ATTACK STAMINA COST MODIFIER
        // LIGHT ATTACK STAMINA COST MODIFIER
        // HEAVY ATTACK STAMINA COST MODIFIER ETC
        
        // item based actions
        
        // ASH OF WAR
        
        // blocking sounds
    }
}
