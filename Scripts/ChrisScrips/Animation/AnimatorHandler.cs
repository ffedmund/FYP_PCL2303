using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FYP
{
    public class AnimatorHandler : AnimatorManager
    {
        PlayerManager playerManager;
        InputHandler inputHandler;
        PlayerLocomotion playerLocomotion;
        int vertical;
        int horizontal;
        public bool canRotate;

        public void Initialize()
        {
            playerManager = GetComponentInParent<PlayerManager>();
            anim = GetComponent<Animator>();
            inputHandler = GetComponentInParent<InputHandler>();
            playerLocomotion = GetComponentInParent<PlayerLocomotion>();
            vertical = Animator.StringToHash("Vertical");
            horizontal = Animator.StringToHash("Horizontal");
        }

        public void UpdateAnimatorValues(float verticalMovement, float horizontalMovement, bool isSprinting)
        {
            #region Vertical
            float v = 0;

            if (verticalMovement > 0 && verticalMovement < 0.55f)
                v = 0.5f;
            else if (verticalMovement > 0.55f)
                v = 1;
            else if (verticalMovement < 0 && verticalMovement > -0.55f)
                v = -0.5f;
            else if (verticalMovement < -0.55f)
                v = -1;
            else
                v = 0;
            #endregion

            #region Horizontal
            float h = 0;

            if (horizontalMovement > 0 && horizontalMovement < 0.55f)
                h = 0.5f;
            else if (horizontalMovement > 0.55f)
                h = 1;
            else if (horizontalMovement < 0 && horizontalMovement > -0.55f)
                h = -0.5f;
            else if (horizontalMovement < -0.55f)
                h = -1;
            else
                h = 0;
            #endregion

            if (isSprinting)
            {
                v = 2;
                h = horizontalMovement;
            }

            anim.SetFloat(vertical, v, 0.1f, Time.deltaTime);
            anim.SetFloat(horizontal, h, 0.1f, Time.deltaTime);
        }

        public void CanRotate()
        {
            canRotate = true;
        }

        public void StopRotation()
        {
            canRotate = false;
        }

        public void EnableCombo()
        {
            anim.SetBool("canDoCombo", true);
        }

        public void DisableCombo()
        {
            anim.SetBool("canDoCombo", false);
        }

        public void EnableIsInvulnerable()
        {
            anim.SetBool("isInvulnerable", true);
        }

        public void DisableIsInvulnerable()
        {
            if (playerManager.isUsingSkill)
                return;
            
            anim.SetBool("isInvulnerable", false);
        }

        public void EnableCanRoll()
        {
            if (playerManager == null)
                return;
                
            playerManager.canRoll = true;
        }

        public void DisableIsJumping()
        {
            if (playerManager == null)
                return;

            anim.SetBool("isJumping", false);
        }

        private void OnAnimatorMove()
        {
            if (playerManager.isInteracting == false)
                return;

            if (!playerManager.anim.applyRootMotion)
                return;
            
            float delta = Time.deltaTime;
            playerLocomotion.rigidbody.drag = 0;
            Vector3 deltaPosition = anim.deltaPosition;
            deltaPosition.y = 0;
            Vector3 velocity = deltaPosition / delta;
            // playerLocomotion.rigidbody.velocity = velocity;

            playerManager.characterController.Move(velocity * Time.deltaTime);

        }

        public void SuccessfullyCastSpell()
        {
            playerManager.playerCombatManager.SuccessfullyCastSpell();
        }

        public IEnumerator HitStop(float time){
            anim.speed = 0.1f;
            yield return new WaitForSeconds(time);
            anim.speed = 1;
            yield return null;
        }
    }
}