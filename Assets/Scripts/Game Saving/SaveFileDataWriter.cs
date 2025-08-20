using UnityEngine;
using System;
using System.IO;

namespace Moko
{
    public class SaveFileDataWriter
    {
        public string saveDataDirectoryPath = "";
        public string saveFileName = "";

        
        // before we create a new save file, we must check to see if one of this character slot already exists
        public bool CheckToSeeIfFileExists()
        {
            return File.Exists(Path.Combine(saveDataDirectoryPath, saveFileName)) ? true : false;
        }

        public void DeleteSaveFile()
        {
            File.SetAttributes(Path.Combine(saveDataDirectoryPath, saveFileName), FileAttributes.Normal);
            File.Delete(Path.Combine(saveDataDirectoryPath, saveFileName));
        }

        public void CreateNewCharacterSaveFile(CharacterSaveData characterData)
        {
            // amke a path to save the file
            string savePath = Path.Combine(saveDataDirectoryPath, saveFileName);

            try
            {
                // create the directory the file will be writtent o, if it does not exist
                Directory.CreateDirectory(Path.GetDirectoryName(savePath));
                Debug.Log("CREATING SAVE FILE, AT SAVE PATH : " + savePath);

                // Serialize the C# Game Data Object into JSON
                string dataToStore = JsonUtility.ToJson(characterData, true);

                // write the file to our system
                using (FileStream stream = new FileStream(savePath, FileMode.Create))
                {
                    using (StreamWriter fileWriter = new StreamWriter(stream))
                    {
                        fileWriter.Write(dataToStore);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("ERROR WHILE TRYING TO SAVE CHARCTER DATA, GAME NOT SAVED" + savePath + "\n" + ex);
            }
        }

        public CharacterSaveData LoadSaveFile()
        {
            CharacterSaveData characterData = null;
            
            string loadPath = Path.Combine(saveDataDirectoryPath, saveFileName);

            if (File.Exists(loadPath))
            {
                try
                {
                    string dataToLoad = "";

                    using (FileStream stream = new FileStream(loadPath, FileMode.Open))
                    {
                        using (StreamReader reader = new StreamReader(stream))
                        {
                            dataToLoad = reader.ReadToEnd();
                        }
                    }

                    // Deserialize the data from json back to unity
                    characterData = JsonUtility.FromJson<CharacterSaveData>(dataToLoad);
                }
                catch (Exception ex)
                {
                    Debug.Log("FAILED TO LOAD SAVE FILE");
                }
            }
            return characterData;
        }
    }
}
