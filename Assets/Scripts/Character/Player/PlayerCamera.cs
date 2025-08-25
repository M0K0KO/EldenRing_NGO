using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

namespace Moko
{
    public class PlayerCamera : MonoBehaviour
    {
        public static PlayerCamera instance;
        public PlayerManager player;
        public Camera cameraObject;
        [SerializeField] private Transform cameraPivotTransform;

        [Header("Camera Settings")]
        private float cameraSmoothSpeed = 1f; // bigger this number, longer for the camera to reach its position

        [SerializeField] private float leftAndRightRotationSpeed = 220;
        [SerializeField] private float upAndDownRotationSpeed = 220;
        [SerializeField] private float minimumPivot = -30;
        [SerializeField] private float maximumPivot = 60;
        [SerializeField] private float cameraCollisionRadius = 0.2f;
        [SerializeField] private LayerMask collideWithLayers;

        [Header("Camera Values")] 
        private Vector3 cameraVelocity;
        private Vector3 cameraObjectPosition; // used for camera collisions (moves the camera object to this pos)
        [SerializeField] private float cameraCollisionLerpSpeed = 0.2f;
        [SerializeField] private float leftAndRightLookAngle;
        [SerializeField] private float upAndDownLookAngle;
        private float cameraZPosition; // values used for camera collisions
        private float targetCameraZPosition; // values used for camera collision

        [Header("LockOn")]
        [SerializeField] private float lockOnRadius = 20f;
        [SerializeField] private float minimumViewableAngle = -50f;
        [SerializeField] private float maximumViewableAngle = 50f;
        [SerializeField] private float lockOnTargetFollowSpeed = 0.2f;
        [SerializeField] private float setCameraHeightSpeed = 1f;
        [SerializeField] private float unlockedCameraHeight = 1.65f;
        [SerializeField] private float lockedCameraHeight = 2.0f;
        private Coroutine cameraLockOnHeightCoroutine;
        private List<CharacterManager> availableTargets = new List<CharacterManager>();
        public CharacterManager nearestLockOnTarget;
        public CharacterManager leftLockOnTarget;
        public CharacterManager rightLockOnTarget;


        private void Awake()
        {
            if (instance == null) instance = this;
            else Destroy(gameObject);
        }

        private void Start()
        {
            DontDestroyOnLoad(gameObject);
            cameraZPosition = cameraObject.transform.localPosition.z;
        }

        public void HandleAllCameraActions()
        {
            if (player)
            {
                HandleFollowTarget();
                HandleRotations();
                HandleCollisions();
            }
        }

        private void HandleFollowTarget()
        {
            Vector3 targetCameraPosition = Vector3.SmoothDamp(
                transform.position,
                player.transform.position,
                ref cameraVelocity,
                cameraSmoothSpeed * Time.deltaTime);
            transform.position = targetCameraPosition;
        }

        private void HandleRotations()
        {
            // if locked on, force rotation towards target
            if (player.playerNetworkManager.isLockedOn.Value)
            {
                // ROTATES THIS GAMEOBJECT
                Vector3 rotationDirection =
                    player.playerCombatManager.currentTarget.characterCombatManager.lockOnTransform.position -
                    transform.position;
                rotationDirection.Normalize();
                rotationDirection.y = 0;
                
                Quaternion targetRotation = Quaternion.LookRotation(rotationDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, lockOnTargetFollowSpeed);
                
                // ROTATES THE PIVOT OBJECT
                rotationDirection =
                    player.playerCombatManager.currentTarget.characterCombatManager.lockOnTransform.position -
                    cameraPivotTransform.position;
                rotationDirection.Normalize();
                
                targetRotation = Quaternion.LookRotation(rotationDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, lockOnTargetFollowSpeed);
                
                // Save our rotations to our look angles, so when we unlock it doesnt snap too far away
                leftAndRightLookAngle = transform.eulerAngles.y;
                upAndDownLookAngle = transform.eulerAngles.x;
            }
            else // else rotate regularly
            {
                // Normal Rotations
                // rotate left and right based on horziontal movement on the mouse
                leftAndRightLookAngle +=
                    (PlayerInputManager.instance.cameraHorizontal_Input * leftAndRightRotationSpeed) * Time.deltaTime;
                // rotate up and down based on vertical movement on the mouse
                upAndDownLookAngle -=
                    (PlayerInputManager.instance.cameraVertical_Input * upAndDownRotationSpeed) * Time.deltaTime;
                // clamp the upAndDownLookAngle between a min/max value
                upAndDownLookAngle = Mathf.Clamp(upAndDownLookAngle, minimumPivot, maximumPivot);

                Vector3 cameraRotation = Vector3.zero;
                Quaternion targetRotation = Quaternion.identity;

                // rotate this gameObject left and right
                cameraRotation.y = leftAndRightLookAngle;
                targetRotation = Quaternion.Euler(cameraRotation);
                transform.rotation = targetRotation;

                // rotate the pivot gameObject up and down
                cameraRotation = Vector3.zero;
                cameraRotation.x = upAndDownLookAngle;
                targetRotation = Quaternion.Euler(cameraRotation);
                cameraPivotTransform.localRotation = targetRotation;
            }
        }
        
        private void HandleCollisions()
        {
            targetCameraZPosition = cameraZPosition;
            
            RaycastHit hit;
            // direction for collision check
            Vector3 direction = cameraObject.transform.position - cameraPivotTransform.position;
            direction.Normalize();

            // check if ther is an object in front of desired direction
            if (Physics.SphereCast(
                    cameraPivotTransform.position, 
                    cameraCollisionRadius,
                    direction, 
                    out hit,
                    Mathf.Abs(targetCameraZPosition), 
                    collideWithLayers))
            {
                // if there is, get distance from it
                float distanceFromHitObject = Vector3.Distance(cameraPivotTransform.position, hit.point);
                // equate our target z position to the following
                targetCameraZPosition = -(distanceFromHitObject - cameraCollisionRadius);
            }

            // if target position is less than collision radius, subtract collision radius
            if (Mathf.Abs(targetCameraZPosition) < cameraCollisionRadius)
            {
                targetCameraZPosition = -cameraCollisionRadius;
            }

            // apply final position using LERP
            cameraObjectPosition.z = Mathf.Lerp(cameraObject.transform.localPosition.z, targetCameraZPosition, cameraCollisionLerpSpeed);
            cameraObject.transform.localPosition = cameraObjectPosition;
        }

        public void HandleLocatingLockOnTargets()
        {
            float shortestDistance = Mathf.Infinity;               //  WILL BE USED TO DETERMINE THE TARGET CLOSEST TO US
            float shortestDistanceOfRightTarget = Mathf.Infinity;  //  WILL BE USED TO DETERMINE SHORTEST DISTANCE ON ONE AXIS TO THE RIGHT OF CURRENT TARGET (+)
            float shortestDistanceOfLeftTarget = -Mathf.Infinity;  //  WILL BE USED TO DETERMINE SHORTEST DISTANCE ON ONE AXIS TO THE LEFT OF CURRENT TARGET (-)

            //  TO DO USE A LAYERMASK
            Collider[] colliders = Physics.OverlapSphere(player.transform.position, lockOnRadius, WorldUtilityManager.instance.GetCharacterLayers());

            for (int i = 0; i < colliders.Length; i++)
            {
                CharacterManager lockOnTarget = colliders[i].GetComponent<CharacterManager>();

                if (lockOnTarget != null)
                {
                    // CHECK IF THEY ARE WITHIN OUR FIELD OF VIEW
                    Vector3 lockOnTargetsDirection = lockOnTarget.transform.position - player.transform.position;
                    float distanceFromTarget = Vector3.Distance(player.transform.position, lockOnTarget.transform.position);
                    float viewableAngle = Vector3.Angle(lockOnTargetsDirection, cameraObject.transform.forward);

                    //  IF TARGET IS DEAD, CHECK THE NEXT POTENTIAL TARGET
                    if (lockOnTarget.isDead.Value)
                        continue;

                    //  IF TARGET IS US, CHECK THE NEXT POTENTIAL TARGET
                    if (lockOnTarget.transform.root == player.transform.root)
                        continue;

                    //  LASTLY IF THE TARGET IS OUTSIDE FIELD OF VIEW OR IS BLOCKED BY ENVIRO, CHECK NEXT POTENTIAL TARGET
                    if (viewableAngle > minimumViewableAngle && viewableAngle < maximumViewableAngle)
                    {
                        RaycastHit hit;

                        if (Physics.Linecast(player.playerCombatManager.lockOnTransform.position, 
                            lockOnTarget.characterCombatManager.lockOnTransform.position, 
                            out hit, WorldUtilityManager.instance.GetEnvironmentLayers()))
                        {
                            //  WE HIT SOMETHING, WE CANNOT SEE OUR LOCK ON TARGET
                            continue;
                        }
                        else
                        {
                            //  OTHERWISE, ADD THEM TO POTENTIAL TARGET LIST
                            availableTargets.Add(lockOnTarget);
                        }
                    }
                }
            }

            //  WE NOW SORT THROUGH OUR POTENTIAL TARGETS TO SEE WHICH ONE WE LOCK ONTO FIRST
            for (int k = 0; k < availableTargets.Count; k++)
            {
                if (availableTargets[k] != null)
                {
                    float distanceFromTarget = Vector3.Distance(player.transform.position, availableTargets[k].transform.position);

                    if (distanceFromTarget < shortestDistance)
                    {
                        shortestDistance = distanceFromTarget;
                        nearestLockOnTarget = availableTargets[k];
                    }

                    //  IF WE ARE ALREADY LOCKED ON WHEN SEARCHING FOR TARGETS, SEARCH FOR OUR NEAREST LEFT/RIGHT TARGETS
                    if (player.playerNetworkManager.isLockedOn.Value)
                    {
                        Vector3 relativeEnemyPosition = player.transform.InverseTransformPoint(availableTargets[k].transform.position);

                        var distanceFromLeftTarget = relativeEnemyPosition.x;
                        var distanceFromRightTarget = relativeEnemyPosition.x;

                        if (availableTargets[k] == player.playerCombatManager.currentTarget)
                            continue;

                        //  CHECK THE LEFT SIDE FOR TARGETS
                        if (relativeEnemyPosition.x <= 0.00 && distanceFromLeftTarget > shortestDistanceOfLeftTarget)
                        {
                            shortestDistanceOfLeftTarget = distanceFromLeftTarget;
                            leftLockOnTarget = availableTargets[k];
                        }
                        //  CHECK THE RIGHT SIDE FOR TARGETS
                        else if (relativeEnemyPosition.x >= 0.00 && distanceFromRightTarget < shortestDistanceOfRightTarget)
                        {
                            shortestDistanceOfRightTarget = distanceFromRightTarget;
                            rightLockOnTarget = availableTargets[k];
                        }
                    }
                }
                else
                {
                    ClearLockOnTargets();
                    player.playerNetworkManager.isLockedOn.Value = false;
                }
            }
        }

        public void SetLockCameraHeight()
        {
            if (cameraLockOnHeightCoroutine != null) StopCoroutine(cameraLockOnHeightCoroutine);
            cameraLockOnHeightCoroutine = StartCoroutine(SetCameraHeight());
        }
        
        public void ClearLockOnTargets()
        {
            nearestLockOnTarget = null;
            leftLockOnTarget = null;
            rightLockOnTarget = null;
            availableTargets.Clear();
        }

        public IEnumerator WaitThenFindNewTarget()
        {
            while (player.isPerformingAction)
            {
                yield return null;
            }
            
            ClearLockOnTargets();
            HandleLocatingLockOnTargets();

            if (nearestLockOnTarget != null)
            {
                player.playerCombatManager.SetTarget(nearestLockOnTarget);
                player.playerNetworkManager.isLockedOn.Value = true;
            }

            yield return null;
        }

        private IEnumerator SetCameraHeight()
        {
            float duration = 1;
            float timer = 0;

            Vector3 velocity = Vector3.zero;
            Vector3 newLockedCameraHeight = new Vector3(cameraPivotTransform.transform.localPosition.x, lockedCameraHeight);
            Vector3 newUnlockedCameraHeight = new Vector3(cameraPivotTransform.transform.localPosition.x, unlockedCameraHeight);
            
            while (timer < duration)
            {
                timer += Time.deltaTime;

                if (player != null)
                {
                    if (player.playerCombatManager.currentTarget != null)
                    {
                        cameraPivotTransform.transform.localPosition = 
                            Vector3.SmoothDamp(cameraPivotTransform.transform.localPosition, newLockedCameraHeight, ref velocity, setCameraHeightSpeed);

                        cameraPivotTransform.transform.localRotation = 
                            Quaternion.Slerp(cameraPivotTransform.transform.localRotation, Quaternion.Euler(0, 0, 0), lockOnTargetFollowSpeed);
                    }
                    else
                    {
                        cameraPivotTransform.transform.localPosition = 
                            Vector3.SmoothDamp(cameraPivotTransform.transform.localPosition, newUnlockedCameraHeight, ref velocity, setCameraHeightSpeed);
                    }
                }

                yield return null;
            }

            if (player != null)
            {
                if (player.playerCombatManager.currentTarget != null)
                {
                    cameraPivotTransform.transform.localPosition = newLockedCameraHeight;
                    cameraPivotTransform.transform.localRotation = Quaternion.Euler(0, 0, 0);
                }
                else
                {
                    cameraPivotTransform.transform.localPosition = newUnlockedCameraHeight;
                }
            }

            yield return null;
        }
    }
}
