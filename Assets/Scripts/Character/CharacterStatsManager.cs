using UnityEngine;

namespace Moko
{
    public class CharacterStatsManager : MonoBehaviour
    {
        private CharacterManager character;
        
        [Header("Stamina Regeneration")] 
        [SerializeField] private float staminaRegenerationAmount = 2f;
        private float staminaRegenerationTimer = 0;
        private float staminaTickTimer = 0;
        [SerializeField] private float staminaRegenerationDelay = 2f;

        protected virtual void Awake()
        {
            character = GetComponent<CharacterManager>();
        }

        protected virtual void Start()
        {
            
        }
        
        public int CalculateHealthBasedOnVitalityLevel(int vitality)
        {
            float health = 0;
            
            // create an equation for how you want your stamina to be calculated

            health = vitality * 15;
            return Mathf.RoundToInt(health);
        }
        
        public int CalculateStaminaBasedOnEnduranceLevel(int endurance)
        {
            float stamina = 0;
            
            // create an equation for how you want your stamina to be calculated

            stamina = endurance * 10;
            return Mathf.RoundToInt(stamina);
        }
        
        public virtual void RegenerateStamina()
        {
            if (!character.IsOwner) return;

            // dont regenerate stamina when using it
            if (character.characterNetworkManager.isSprinting.Value) return;
            if (character.isPerformingAction) return;

            staminaRegenerationTimer += Time.deltaTime;
            if (staminaRegenerationTimer >= staminaRegenerationDelay)
            {
                if (character.characterNetworkManager.currentStamina.Value < character.characterNetworkManager.maxStamina.Value)
                {
                    staminaTickTimer += Time.deltaTime;

                    if (staminaTickTimer >= 0.1f)
                    {
                        staminaTickTimer = 0;
                        character.characterNetworkManager.currentStamina.Value += staminaRegenerationAmount;
                    }
                }
            }
        }

        public virtual void ResetStaminaRegenTimer(float previousStaminaAmount, float currentStaminaAmount)
        {
            // only want to reset the regeneration if the action used stamina
            // dont want to reset the regeneration if we are already regenerating stamina
            if (currentStaminaAmount < previousStaminaAmount)
            {
                staminaRegenerationTimer = 0;
            }
        }
    }
}
