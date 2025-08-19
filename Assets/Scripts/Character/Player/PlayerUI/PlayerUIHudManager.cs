using UnityEngine;

namespace Moko
{
    public class PlayerUIHudManager : MonoBehaviour
    {
        [SerializeField] private UI_StatBar staminaBar;

        public void SetNewStaminaValue(float oldValue, float newValue)
        {
            staminaBar.SetStat(Mathf.RoundToInt(newValue));
        }

        public void SetMaxStaminaValue(int maxStamina)
        {
            staminaBar.SetMaxStat(maxStamina);
        }
    }
}
