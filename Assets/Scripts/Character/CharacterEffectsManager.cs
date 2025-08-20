using System;
using UnityEngine;


namespace Moko
{
    public class CharacterEffectsManager : MonoBehaviour
    {
        private CharacterManager character;

        protected void Awake()
        {
            character = GetComponent<CharacterManager>();
        }
        

        // process isntant effects (take damage, healing)
        
        // process timed effects (poison, build ups)
        
        // process static effect (Adding/Removing buffs from talismans etc)

        public virtual void ProcessInstantEffect(InstantCharacterEffect effect)
        {
            // Take in an effect
            // process it
            effect.ProcessEffect(character);
        }
    }
}
