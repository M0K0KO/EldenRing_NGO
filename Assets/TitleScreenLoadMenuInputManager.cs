using System;
using UnityEngine;

namespace Moko
{
    public class TitleScreenLoadMenuInputManager : MonoBehaviour
    {
        private PlayerControls playerControls;
        
        [Header("Title Screen Inputs")] 
        [SerializeField] private bool deleteCharacterSlot = false;

        private void Update()
        {
            if (deleteCharacterSlot)
            {
                deleteCharacterSlot = false;
                TitleScreenManager.Instance.AttemptToDeleteCharacterSlot();
            }
        }

        private void OnEnable()
        {
            if (playerControls == null)
            {
                playerControls = new PlayerControls();
                
                playerControls.UI.Delete.performed += i => deleteCharacterSlot = true;
            }
            
            playerControls.Enable();
        }

        private void OnDisable()
        {
            playerControls.Disable();
        }
    }
}
