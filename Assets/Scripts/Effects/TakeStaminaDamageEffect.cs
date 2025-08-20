using UnityEngine;

namespace Moko
{
    [CreateAssetMenu(menuName = "Character Effects/Instant Effects/Take Stamina Damage")]
    public class TakeStaminaDamageEffect : InstantCharacterEffect
    {
        public float staminaDamage;
        
        public override void ProcessEffect(CharacterManager character)
        {
            // why do this?
            CalculateStaminaDamage(character);
        }

        private void CalculateStaminaDamage(CharacterManager character)
        {
             // compare the base stamina damage against other player effects/modifiers
             // change the value befor subtracting/adding it
             // play sound fx or vfx during effect

             if (character.IsOwner)
             {
                 Debug.Log("CHARACTER IS TAKING " + staminaDamage + " STAMINA DAMAGE");
                 character.characterNetworkManager.currentStamina.Value -= staminaDamage;
             }
        }
    }
}
