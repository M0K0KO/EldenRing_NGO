using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Moko
{
    public class WorldItemDatabase : MonoBehaviour
    {
        public static WorldItemDatabase Instance;
        
        [Header("Weapons")]
        public WeaponItem unarmedWeapon;
        [SerializeField] private List<WeaponItem> weapons = new List<WeaponItem>();
        
        [Header("Items")]
        private List<Item> items = new List<Item>();

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }

            // ADD ALL WEAPONS TO THE LIST OF ITEM
            foreach (var weapon in weapons)
            {
                items.Add(weapon);
            }

            // ASSIGN ALL ITEMS A UNIQUE ITEM ID
            for (int i = 0; i < items.Count; i++)
            {
                items[i].itemID = i;
            }
        }

        public WeaponItem GetWeaponByID(int ID)
        {
            return weapons.FirstOrDefault(weapon => weapon.itemID == ID);
        }
    }
}