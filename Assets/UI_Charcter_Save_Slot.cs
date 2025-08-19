using System;
using TMPro;
using UnityEngine;

namespace Moko
{
    public class UI_Charcter_Save_Slot : MonoBehaviour
    {
        private SaveFileDataWriter saveFileDataWriter;

        [Header("Game Slot")] 
        public CharacterSlot characterSlot;

        [Header("Character Info")]
        public TextMeshProUGUI characterName;
        public TextMeshProUGUI timePlayed;

        private void OnEnable()
        {
            LoadSaveSlots();
        }

        private void LoadSaveSlots()
        {
            saveFileDataWriter = new SaveFileDataWriter();
            saveFileDataWriter.saveDataDirectoryPath = Application.persistentDataPath;

            if (characterSlot == CharacterSlot.CharacterSlot_01)
            {
                saveFileDataWriter.saveFileName =
                    WorldSaveGameManager.instance.DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(characterSlot);

                // if the file exists, get information from it
                if (saveFileDataWriter.CheckToSeeIfFileExists())
                {
                    characterName.text = WorldSaveGameManager.instance.characterSlot01.characterName;
                }
                else
                {
                    gameObject.SetActive(false);
                }
            }
            else if (characterSlot == CharacterSlot.CharacterSlot_02)
            {
                saveFileDataWriter.saveFileName =
                    WorldSaveGameManager.instance.DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(characterSlot);

                if (saveFileDataWriter.CheckToSeeIfFileExists())
                {
                    characterName.text = WorldSaveGameManager.instance.characterSlot01.characterName;
                }
                else
                {
                    gameObject.SetActive(false);
                }
            }
            else if (characterSlot == CharacterSlot.CharacterSlot_03)
            {
                saveFileDataWriter.saveFileName =
                    WorldSaveGameManager.instance.DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(characterSlot);

                if (saveFileDataWriter.CheckToSeeIfFileExists())
                {
                    characterName.text = WorldSaveGameManager.instance.characterSlot01.characterName;
                }
                else
                {
                    gameObject.SetActive(false);
                }
            }
            else if (characterSlot == CharacterSlot.CharacterSlot_04)
            {
                saveFileDataWriter.saveFileName =
                    WorldSaveGameManager.instance.DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(characterSlot);

                if (saveFileDataWriter.CheckToSeeIfFileExists())
                {
                    characterName.text = WorldSaveGameManager.instance.characterSlot01.characterName;
                }
                else
                {
                    gameObject.SetActive(false);
                }
            }
            else if (characterSlot == CharacterSlot.CharacterSlot_05)
            {
                saveFileDataWriter.saveFileName =
                    WorldSaveGameManager.instance.DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(characterSlot);

                if (saveFileDataWriter.CheckToSeeIfFileExists())
                {
                    characterName.text = WorldSaveGameManager.instance.characterSlot01.characterName;
                }
                else
                {
                    gameObject.SetActive(false);
                }
            }
            else if (characterSlot == CharacterSlot.CharacterSlot_06)
            {
                saveFileDataWriter.saveFileName =
                    WorldSaveGameManager.instance.DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(characterSlot);

                if (saveFileDataWriter.CheckToSeeIfFileExists())
                {
                    characterName.text = WorldSaveGameManager.instance.characterSlot01.characterName;
                }
                else
                {
                    gameObject.SetActive(false);
                }
            }
            else if (characterSlot == CharacterSlot.CharacterSlot_07)
            {
                saveFileDataWriter.saveFileName =
                    WorldSaveGameManager.instance.DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(characterSlot);

                if (saveFileDataWriter.CheckToSeeIfFileExists())
                {
                    characterName.text = WorldSaveGameManager.instance.characterSlot01.characterName;
                }
                else
                {
                    gameObject.SetActive(false);
                }
            }
            else if (characterSlot == CharacterSlot.CharacterSlot_08)
            {
                saveFileDataWriter.saveFileName =
                    WorldSaveGameManager.instance.DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(characterSlot);

                if (saveFileDataWriter.CheckToSeeIfFileExists())
                {
                    characterName.text = WorldSaveGameManager.instance.characterSlot01.characterName;
                }
                else
                {
                    gameObject.SetActive(false);
                }
            }
            else if (characterSlot == CharacterSlot.CharacterSlot_09)
            {
                saveFileDataWriter.saveFileName =
                    WorldSaveGameManager.instance.DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(characterSlot);

                if (saveFileDataWriter.CheckToSeeIfFileExists())
                {
                    characterName.text = WorldSaveGameManager.instance.characterSlot01.characterName;
                }
                else
                {
                    gameObject.SetActive(false);
                }
            }
            else if (characterSlot == CharacterSlot.CharacterSlot_10)
            {
                saveFileDataWriter.saveFileName =
                    WorldSaveGameManager.instance.DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(characterSlot);

                if (saveFileDataWriter.CheckToSeeIfFileExists())
                {
                    characterName.text = WorldSaveGameManager.instance.characterSlot01.characterName;
                }
                else
                {
                    gameObject.SetActive(false);
                }
            }
        }
        
        public void LoadGameFromCharacterSlot()
        {
            WorldSaveGameManager.instance.currentCharacterSlotBeingUsed = characterSlot;
            WorldSaveGameManager.instance.LoadGame();
        }

        public void SelectCurrentSlot()
        {
            TitleScreenManager.Instance.SelecteCharacterSlot(characterSlot);
        }
    }
}
