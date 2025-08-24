using Unity.Netcode;
using UnityEngine;
using System.Collections.Generic;

namespace Moko
{
    public class CharacterAnimatorManager : MonoBehaviour
    {
        private CharacterManager character;

        private readonly int vertical = Animator.StringToHash("Vertical");
        private readonly int horizontal = Animator.StringToHash("Horizontal");

        [Header("Damage Animations")] 
        public string lastDamageAnimationPlayed;
        
        [SerializeField] private string hit_Forward_Medium_01 = "hit_Forward_Medium_01";
        [SerializeField] private string hit_Forward_Medium_02 = "hit_Forward_Medium_02";
        [SerializeField] private string hit_Backward_Medium_01 = "hit_Backward_Medium_01";
        [SerializeField] private string hit_Backward_Medium_02 = "hit_Backward_Medium_02";
        [SerializeField] private string hit_Left_Medium_01 = "hit_Left_Medium_01";
        [SerializeField] private string hit_Left_Medium_02 = "hit_Left_Medium_02";
        [SerializeField] private string hit_Right_Medium_01 = "hit_Right_Medium_01";
        [SerializeField] private string hit_Right_Medium_02 = "hit_Right_Medium_02";

        public List<string> forward_Medium_Damage = new List<string>();
        public List<string> backward_Medium_Damage = new List<string>();
        public List<string> left_Medium_Damage = new List<string>();
        public List<string> right_Medium_Damage = new List<string>();

        protected virtual void Awake()
        {
            character = GetComponent<CharacterManager>();
        }

        protected virtual void Start()
        {
            forward_Medium_Damage.Add(hit_Forward_Medium_01);
            forward_Medium_Damage.Add(hit_Forward_Medium_02);
            
            backward_Medium_Damage.Add(hit_Backward_Medium_01);
            backward_Medium_Damage.Add(hit_Backward_Medium_02);
            
            left_Medium_Damage.Add(hit_Left_Medium_01);
            left_Medium_Damage.Add(hit_Left_Medium_02);
            
            right_Medium_Damage.Add(hit_Right_Medium_01);
            right_Medium_Damage.Add(hit_Right_Medium_02);
        }

        public string GetRandomAnimationFromList(List<string> animationList)
        {
            List<string> finalList = new List<string>();

            foreach (var animation in animationList)
            {
                finalList.Add(animation);
            }
            
            // check if we have already played this damage animation so it doesnt repeat
            finalList.Remove(lastDamageAnimationPlayed);

            for (int i = finalList.Count - 1; i >= 0; i--)
            {
                if (finalList[i] == null)
                {
                    finalList.RemoveAt(i);
                }
            }

            int index = Random.Range(0, finalList.Count);

            return finalList[index];
        }

        public void UpdateAnimatorMovementParameters(float horizontalMovement, float verticalMovement, bool isSprinting)
        {
            float horizontalAmount = horizontalMovement;
            float verticalAmount = verticalMovement;

            if (isSprinting)
            {
                verticalAmount = 2;
            }

            character.animator.SetFloat(horizontal, horizontalAmount, 0.1f, Time.deltaTime);
            character.animator.SetFloat(vertical, verticalAmount, 0.1f, Time.deltaTime);
        }

        public virtual void PlayTargetActionAnimation(
            string targetAnimation,
            bool isPerformingAction,
            bool applyRootMotion = true,
            bool canRotate = false,
            bool canMove = false)
        {
            Debug.Log("PLAYING ANIMATION : " + targetAnimation);
            character.applyRootMotion = applyRootMotion;
            character.animator.CrossFade(targetAnimation, 0.2f);

            // can be used to stop character from ateempting new actions
            // [EXAMPLE] if you get damaged, and begin performing a damage animation
            // this flag will turn true if you are stunned
            // then we can check for this before attempting new actions
            character.isPerformingAction = isPerformingAction;
            character.canRotate = canRotate;
            character.canMove = canMove;

            // Tell the server we played an animation, and to play that animation for everybody else present
            character.characterNetworkManager.NotifyTheServerOfActionAnimationServerRpc(
                NetworkManager.Singleton.LocalClientId,
                targetAnimation,
                applyRootMotion);
        }

        public virtual void PlayTargetAttackActionAnimation(
            AttackType attackType,
            string targetAnimation,
            bool isPerformingAction,
            bool applyRootMotion = true,
            bool canRotate = false,
            bool canMove = false)
        {
            // keep track of the last attack performed (combo attacks)
            // keep track of current attack type (light, heavy, etc)
            // update animation set to current weapons animations
            // decide if our attack can be parried
            // tell the network our "ISATTACKING" flag (for counter damage etc)
            character.characterCombatManager.currentAttackType = attackType;
            character.applyRootMotion = applyRootMotion;
            character.animator.CrossFade(targetAnimation, 0.2f);
            character.isPerformingAction = isPerformingAction;
            character.canRotate = canRotate;
            character.canMove = canMove;

            // Tell the server we played an animation, and to play that animation for everybody else present
            character.characterNetworkManager.NotifyTheServerOfAttackActionAnimationServerRpc(
                NetworkManager.Singleton.LocalClientId,
                targetAnimation,
                applyRootMotion);
        }
    }
}