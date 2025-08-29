using UnityEngine;

namespace Moko
{
    public class AIBossCharacterManager : AICharacterManager
    {
        // Give this A.I unique id
        // when this A.I is spawned, check our save file (DICTIONARY)
        // if the save file does not contain a boss monster with this ID add it
        // if it is present, check if the boss has been defeated
        // if the boss has been defeated, disable this gameObject
        // if the boss has not been defeated, allow this obejct to continue to be active
    }
}