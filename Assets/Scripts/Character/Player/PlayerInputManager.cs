using System;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.Serialization;
using UnityEngine.SceneManagement;

namespace Moko
{
    public class PlayerInputManager : MonoBehaviour
    {
        public static PlayerInputManager instance;
        
        // 1. find a way to read the values
        // 2. move character based on those values

        private PlayerControls playerControls;

        [SerializeField] private Vector2 movementInput;
        public float verticalInput;
        public float horizontalInput;
        public float moveAmount;

        private void Awake()
        {
            if (instance == null) instance = this;
            else Destroy(gameObject);
        }

        private void Start()
        {
            DontDestroyOnLoad(gameObject);

            // when the scene changes, run this logic
            SceneManager.activeSceneChanged += OnSceneChange;
            
            instance.enabled = false;
        }

        private void OnSceneChange(Scene oldScene, Scene newScene)
        {
            // if we are loading into our world scene, enable our players controls
            if (newScene.buildIndex == WorldSaveGameManager.instance.GetWorldScneneIndex())
            {
                instance.enabled = true;
            }
            // otherwise we must be at the main menu, disable our players controls
            // this is so our player cant move around if we enter things like a character creation menu etc..
            else
            {
                instance.enabled = false;
            }
        }

        private void OnEnable()
        {
            if (playerControls == null)
            {
                playerControls = new PlayerControls();

                playerControls.PlayerMovement.Movement.performed += i => movementInput = i.ReadValue<Vector2>();
            }

            playerControls.Enable();
        }

        private void OnDestroy()
        {
            // if we destroy this object, unsubscribe from this event
            SceneManager.activeSceneChanged -= OnSceneChange;
        }

        // if we minimize or lower the window, stop adjusting inputs
        private void OnApplicationFocus(bool focus)
        {
            if (enabled)
            {
                if (focus)
                {
                    playerControls.Enable();
                }
                else
                {
                    playerControls.Disable();
                }
            }
        }

        private void Update()
        {
            HandleMovementInput();
        }

        private void HandleMovementInput()
        {
            verticalInput = movementInput.y;
            horizontalInput = movementInput.x;
            
            // return the absolute number
            moveAmount = Mathf.Clamp01(Mathf.Abs(verticalInput) + Mathf.Abs(horizontalInput));
            
            // we clamp the values, so they are 0, 0.5 or 1
            if (moveAmount <= 0.5 && moveAmount > 0)
            {
                moveAmount = 0.5f;
            }
            else if (moveAmount > 0.5f && moveAmount <= 1)
            {
                moveAmount = 1;
            }
        }
    }
}
