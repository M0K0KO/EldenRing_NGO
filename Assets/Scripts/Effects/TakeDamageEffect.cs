using UnityEngine;

namespace Moko
{
    [CreateAssetMenu(menuName = "Character Effects/Instant Effects/Take Damage")]
    public class TakeDamageEffect : InstantCharacterEffect
    {
        [Header("Character Causing Damage")] 
        public CharacterManager characterCausingDamage; // if the damage is caused by anothoer characters attack it will be stored here

        [Header("Damage")]
        public float physicalDamage = 0; // in the future will be split into "Standarad", "Strike", "Slash" and "Pierce"
        public float magicDamage = 0;
        public float fireDamage = 0;
        public float lightningDamage = 0;
        public float holyDamage = 0;
        
        [Header("Final Damage")]
        private int finalDamageDealt = 0; // the damage character takes after ALL CALCULATIONS have been made

        [Header("Poise")] 
        public float poiseDamage = 0;
        public bool poiseIsBroken = false; // if a character's poise is broken, they will be stunned and play a damage animation
        
        // (TO DO) Build ups
        // build up effect amounts

        [Header("Animation")] 
        public bool playDamageAnimation = true;
        public bool manuallySelectDamageAniamtion = false;
        public string damageAnimation;

        [Header("Sound FX")] 
        public bool willPlayDamageSFX = true;
        public AudioClip elementalDamageSoundFX; // used on top of regular sfx if there is elemental damage present (Magic/Fire/Lightning/Holy)

        [Header("Direction Damage Taken From")]
        public float angleHitFrom; // Used to determine what damage animation to play (Directional Animations)
        public Vector3 contactPoint; // used to determine where the blood FX instantiates
        
        public override void ProcessEffect(CharacterManager character)
        {
            base.ProcessEffect(character);

            if (character.isDead.Value) return; // check if the character is dead
            
            // check for "invulnerability"

            CalculateDamage(character); // calculate damage

            // check which direction damage came from

            // play a damage animation

            // check for build ups (POISON, BLEED ETC)

            // play damage SFX, VFX

            // if character is A.I, check for new target if character causing damage is present
        }

        private void CalculateDamage(CharacterManager character)
        {
            if (!character.IsOwner) return;
            
            if (characterCausingDamage != null)
            {
                // check for damage modifiers and modify base damage (Physicsal damage buff, elemental damage buff etc)
                // [EX] physical *= physicalModifier 
            }
            
            // Check character for flat defenses and subtract them from the damage
            
            // check character for armor abosrptions, and subtract the percentage from the damage
            
            // add all damage types together, and apply final damage
            finalDamageDealt = Mathf.RoundToInt(physicalDamage + magicDamage + fireDamage + lightningDamage + holyDamage);

            if (finalDamageDealt <= 0)
            {
                finalDamageDealt = 1;
            }
            
            Debug.Log($"FINAL DAMAGE DEALT : {finalDamageDealt}");
            character.characterNetworkManager.currentHealth.Value -= finalDamageDealt;
            
            // Calculate poise damage to determine if the character will be stunned
        }
    }
}
