using UnityEngine;
using UnityEngine.UI;

namespace Moko
{
    public class PlayerUIHudManager : MonoBehaviour
    {
        [Header("STAT BARS")]
        [SerializeField] private UI_StatBar healthBar;
        [SerializeField] private UI_StatBar staminaBar;

        [Header("QUICK SLOTS")]
        [SerializeField] private Image rightWeaponQuickSlotIcon;
        [SerializeField] private Image leftWeaponQuickSlotIcon;

        public void RefreshHUD()
        {
            healthBar.gameObject.SetActive(false);
            healthBar.gameObject.SetActive(true);
            
            staminaBar.gameObject.SetActive(false);
            staminaBar.gameObject.SetActive(true);
        }
        
        public void SetNewHealthValue(int oldValue, int newValue)
        {
            healthBar.SetStat(newValue);
        }

        public void SetMaxHealthValue(int maxHealth)
        {
            healthBar.SetMaxStat(maxHealth);
        }
        
        public void SetNewStaminaValue(float oldValue, float newValue)
        {
            staminaBar.SetStat(Mathf.RoundToInt(newValue));
        }

        public void SetMaxStaminaValue(int maxStamina)
        {
            staminaBar.SetMaxStat(maxStamina);
        }

        public void SetRightWeaponQuickSlotIcon(int weaponID)
        {
            // 1. dierctly referencing the right weapon in the hand of the player -> should remember order of operation
            // if i forgot to call this AFTER i loaded weapons -> ERROR
            
            // 2. require an itemID of the weapon, fetch the weapon from database
            // dont need to remember the order of operation

            WeaponItem weapon = WorldItemDatabase.Instance.GetWeaponByID(weaponID);
            
            if (weapon == null)
            {
                Debug.Log("ITEM IS NULL");
                rightWeaponQuickSlotIcon.enabled = false;
                rightWeaponQuickSlotIcon.sprite = null;
                return;
            }

            if (weapon.itemIcon == null)
            {
                Debug.Log("ITEM HAS NO ICON");
                rightWeaponQuickSlotIcon.enabled = false;
                rightWeaponQuickSlotIcon.sprite = null;
            }
            
            rightWeaponQuickSlotIcon.sprite = weapon.itemIcon;       
            rightWeaponQuickSlotIcon.enabled = true;
        }
        
        public void SetLeftWeaponQuickSlotIcon(int weaponID)
        {
            // 1. dierctly referencing the right weapon in the hand of the player -> should remember order of operation
            // if i forgot to call this AFTER i loaded weapons -> ERROR
            
            // 2. require an itemID of the weapon, fetch the weapon from database
            // dont need to remember the order of operation

            WeaponItem weapon = WorldItemDatabase.Instance.GetWeaponByID(weaponID);
            
            if (weapon == null)
            {
                Debug.Log("ITEM IS NULL");
                leftWeaponQuickSlotIcon.enabled = false;
                leftWeaponQuickSlotIcon.sprite = null;
                return;
            }

            if (weapon.itemIcon == null)
            {
                Debug.Log("ITEM HAS NO ICON");
                leftWeaponQuickSlotIcon.enabled = false;
                leftWeaponQuickSlotIcon.sprite = null;
            }
            
            leftWeaponQuickSlotIcon.sprite = weapon.itemIcon;       
            leftWeaponQuickSlotIcon.enabled = true;
        }
    }
}
