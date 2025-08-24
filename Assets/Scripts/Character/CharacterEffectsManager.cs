using System;
using UnityEngine;


namespace Moko
{
    public class CharacterEffectsManager : MonoBehaviour
    {
        private CharacterManager character;

        [Header("VFX")] 
        [SerializeField] private GameObject bloodSplatterVFX;

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

        public void PlayBloodSplatterVFX(Vector3 contactPoint)
        {
            if (bloodSplatterVFX != null) // if we manually have placed a blood splatter vfx on this model, play its version
            {
                GameObject bloodSplatter = Instantiate(bloodSplatterVFX, contactPoint, Quaternion.identity);
            }
            else // use the generic
            {
                GameObject bloodSplatter = Instantiate(WorldCharacterEffectsManager.instance.bloodSplatterVFX, contactPoint, Quaternion.identity);
            }
        }
    }
}
