using UnityEngine;

namespace Moko
{
    [System.Serializable]
    // since we want to reference this data for every save file, this script is not a monobehvaiour and is instaed serializable
    public class CharacterSaveData
    {
        [Header("Scene Index")] 
        public int sceneIndex = 1;
        
        [Header("Character Name")]
        public string characterName = "character";

        [Header("Time Played")] 
        public float secondsPlayed;

        [Header("World Coordinates")] 
        public float xPosition;
        public float yPosition;
        public float zPosition;

        [Header("Resources")] 
        public int currentHealth;
        public float currentStamina;

        [Header("Stats")] 
        public int vitality;
        public int endurance;
    }
}
