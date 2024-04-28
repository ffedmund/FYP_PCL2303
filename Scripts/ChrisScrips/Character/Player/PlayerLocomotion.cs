using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;

// using System.Numerics;

using UnityEngine;
using UnityEngine.XR;

namespace FYP
{
    public class PlayerLocomotion : MonoBehaviour
    {
        CameraHandler cameraHandler;
        PlayerManager playerManager;
        PlayerStats playerStats;
        Transform cameraObject;
        InputHandler inputHandler;
        public Vector3 targetDirection;
        public Vector3 targetDir;
        public Vector3 rotationDirection;
        public Vector3 moveDirection;
        public Vector3 jumpDirection;

        [HideInInspector]
        public Transform myTransform;
        [HideInInspector]
        public AnimatorHandler animatorHandler;

        public new Rigidbody rigidbody;
        public GameObject normalCamera;

        [Header("Ground & Air Detection Stats")]
        [SerializeField]
        float groundDetectionRayStartPoint = 0.5f;
        [SerializeField]
        float minimumDistanceNeededToBeginFall = 1f;
        [SerializeField]
        float groundDirectionRayDistance = -0.3f;
        LayerMask ignoreForGroundCheck;
        public float inAirTimer;

        [Header("Movement Stats")]
        [SerializeField]
        public float movementSpeed = 5;
        [SerializeField]
        public float sprintSpeed = 7;
        [SerializeField]
        float rotationSpeed = 10;
        [SerializeField]
        float fallingSpeed = 80f;

        [Header("Stamina Costs")]
        [SerializeField]
        int rollStaminaCost = 15;
        int backstepStaminaCost = 10;
        public float sprintStaminaCost = 0.5f;

        public CapsuleCollider characterCollider;
        public CapsuleCollider characterCollisionBlockerCollider;

        #region newVariables

        public float verticalMovement;
        public float horizontalMovement;
        // public float moveAmount;

        [SerializeField] float walkingSpeed = 3f;
        [SerializeField] float runningSpeed = 5f;

        // Ground check and jumping
        [SerializeField] float jumpHeight = 3f;
        [SerializeField] protected float gravityForce = -5.55f;
        [SerializeField] LayerMask groundLayer;
        [SerializeField] public float groundCheckSphereRadius = 0.3f;
        [SerializeField] protected Vector3 yVelocity;
        [SerializeField] protected float groundedYVelocity = -20f;
        [SerializeField] protected float fallStartYVelocity = -5f;
        protected bool fallingVelocityHasBeenSet = false;

        [SerializeField] protected Vector3 rollDirection;

        #endregion

        private void Awake()
        {
            cameraHandler = FindObjectOfType<CameraHandler>();
            playerManager = GetComponent<PlayerManager>();
            playerStats = GetComponent<PlayerStats>();
            rigidbody = GetComponent<Rigidbody>();
            inputHandler = GetComponent<InputHandler>();
            animatorHandler = GetComponentInChildren<AnimatorHandler>();
        }

        void Start()
        {
            cameraObject = GameManager.instance? GameManager.instance.mainCamera.transform:Camera.main.transform;
            myTransform = transform;
            animatorHandler.Initialize();

            playerManager.anim.SetBool("isGrounded", true);
            ignoreForGroundCheck = ~(1 << 8 | 1 << 11 | 1 << 14);
            Physics.IgnoreCollision(characterCollider, characterCollisionBlockerCollider, true);
        }

        #region newMethods

        protected void HandleGroundCheck()
        {
            if (Physics.CheckSphere(playerManager.characterController.transform.position, groundCheckSphereRadius, groundLayer))
            {
                playerManager.anim.SetBool("isGrounded", true);
            }
            else
            {
                playerManager.anim.SetBool("isGrounded", false);
            }


            if (playerManager.isGrounded)
            {
                if (yVelocity.y < 0)
                {
                    inAirTimer = 0;
                    fallingVelocityHasBeenSet = false;
                    yVelocity.y = groundedYVelocity;
                }
            }
            else
            {
                if (!playerManager.isJumping && !fallingVelocityHasBeenSet)
                {
                    fallingVelocityHasBeenSet = true;
                    yVelocity.y = fallStartYVelocity;
                }

                inAirTimer += Time.deltaTime;

                yVelocity.y += gravityForce * Time.deltaTime;
                playerManager.anim.SetFloat("InAirTimer", inAirTimer);
            }

            playerManager.characterController.Move(yVelocity * Time.deltaTime);
        }

        protected void OnDrawGizmosSelected()
        {
            // Gizmos.DrawSphere(transform.position, groundCheckSphereRadius);

            Gizmos.DrawSphere(playerManager.characterController.transform.position, groundCheckSphereRadius);
        }

        public void HandleAllMovement()
        {
            HandleGroundCheck();
            HandleGroundedMovement();
            HandleSprinting();
            HandleRotation();
        }

        public void GetVerticalAndHorizontalInput()
        {
            verticalMovement = inputHandler.vertical;
            horizontalMovement = inputHandler.horizontal;
        }

        public void HandleGroundedMovement()
        {
            if (playerManager.isInteracting)
                return;

            GetVerticalAndHorizontalInput();

            moveDirection = cameraObject.forward * verticalMovement;
            moveDirection += cameraObject.right * horizontalMovement;
            moveDirection.Normalize();
            moveDirection.y = 0;
            

            if (playerManager.isSprinting)
            {
                playerStats.TakeStaminaDamage(sprintStaminaCost * Time.deltaTime * 100);
                playerManager.characterController.Move(moveDirection * sprintSpeed * Time.deltaTime);
            }
            else
            {
                if (inputHandler.moveAmount > 0.5f)
                {
                    playerManager.characterController.Move(moveDirection * runningSpeed * Time.deltaTime);
                }
                else if (inputHandler.moveAmount <= 0.5f)
                {
                    playerManager.characterController.Move(moveDirection * walkingSpeed * Time.deltaTime);
                }
            }



            if (inputHandler.lockOnFlag && inputHandler.sprintFlag == false)
            {
                animatorHandler.UpdateAnimatorValues(inputHandler.vertical, inputHandler.horizontal, playerManager.isSprinting);
            }
            else
            {
                animatorHandler.UpdateAnimatorValues(inputHandler.moveAmount, 0, playerManager.isSprinting);
            }

            if (animatorHandler.canRotate)
            {
                HandleRotation();
            }
        }

        public void HandleSprinting()
        {
            if (playerManager.isInteracting)
            {
                playerManager.isSprinting = false;
            }

            if (inputHandler.sprintFlag && inputHandler.moveAmount >= 0.5)
            {
                playerManager.isSprinting = true;
            }
            else
            {
                playerManager.isSprinting = false;
            }

        }

        public void ApplyJumpingVelocity()
        {
            yVelocity.y = Mathf.Sqrt(jumpHeight * -2f * gravityForce);

        }

        #endregion

        #region Movement
        Vector3 normalVector;
        Vector3 targetPosition;

        private void HandleRotation()
        {
            if (inputHandler.lockOnFlag && inputHandler.sprintFlag == false)
            {
                if (inputHandler.sprintFlag || inputHandler.rollFlag)
                {
                    // Vector3 targetDirection = Vector3.zero;
                    targetDirection = cameraHandler.cameraTransform.forward * inputHandler.vertical;
                    targetDirection += cameraHandler.cameraTransform.right * inputHandler.horizontal;
                    targetDirection.Normalize();
                    targetDirection.y = 0;

                    if (targetDirection == Vector3.zero)
                        targetDirection = myTransform.forward;

                    Quaternion tr = Quaternion.LookRotation(targetDirection);
                    Quaternion targetRotation = Quaternion.Slerp(transform.rotation, tr, rotationSpeed * Time.deltaTime);

                    transform.rotation = targetRotation;
                }
                else
                {
                    if (!playerManager.canRoll) return;

                    // Vector3 rotationDirection = moveDirection;
                    rotationDirection = cameraHandler.currentLockOnTarget.transform.position - transform.position;
                    rotationDirection.y = 0;
                    rotationDirection.Normalize();
                    Quaternion tr = Quaternion.LookRotation(rotationDirection);
                    Quaternion targetRotation = Quaternion.Slerp(transform.rotation, tr, rotationSpeed * Time.deltaTime);
                    transform.rotation = targetRotation;
                }
            }
            else
            {
                targetDir = Vector3.zero;
                float moveOverride = inputHandler.moveAmount;

                targetDir = cameraObject.forward * inputHandler.vertical;
                targetDir += cameraObject.right * inputHandler.horizontal;
                targetDir.Normalize();
                targetDir.y = 0;

                if (targetDir == Vector3.zero)
                    targetDir = myTransform.forward;

                float rs = rotationSpeed;

                Quaternion tr = Quaternion.LookRotation(targetDir);
                Quaternion targetRotation = Quaternion.Slerp(myTransform.rotation, tr, rs * Time.deltaTime);

                myTransform.rotation = targetRotation;
            }
        }

        #endregion

        public void HandleMovement(float delta)
        {
            if (inputHandler.rollFlag)
                return;

            if (playerManager.isInteracting)
                return;

            moveDirection = cameraObject.forward * inputHandler.vertical;
            moveDirection += cameraObject.right * inputHandler.horizontal;
            moveDirection.Normalize();
            moveDirection.y = 0;


            float avoidFloorDistance = .1f;
            float ladderGrabDistance = .4f;
            if (!playerManager.isClimbing)
            {
                if (Physics.Raycast(transform.position + Vector3.up * avoidFloorDistance, targetDir, out RaycastHit raycastHit, ladderGrabDistance))
                {
                    if (raycastHit.transform.TryGetComponent(out Ladder ladder))
                    {
                        playerManager.isClimbing = true;
                    }
                }
            }
            else
            {
                if (Physics.Raycast(transform.position + Vector3.up * avoidFloorDistance, targetDir, out RaycastHit raycastHit, ladderGrabDistance))
                {
                    if (!raycastHit.transform.TryGetComponent(out Ladder ladder))
                    {
                        playerManager.isClimbing = false;
                        moveDirection.y = 5f;
                    }
                }
                else
                {
                    playerManager.isClimbing = false;
                    moveDirection.y = 5f;
                }
            }

            if (playerManager.isClimbing)
            {
                moveDirection.x = 0f;
                // moveDirection.y = Mathf.Abs(moveDirection.z);
                moveDirection.y = 2f * inputHandler.vertical;
                moveDirection.z = 0f;

                // playerManager.isGrounded = true;
                playerManager.anim.SetBool("isGrounded", true);
                // playerManager.isInAir = false;
                playerManager.anim.SetBool("isInAir", false);
            }

            float speed = movementSpeed;

            if (inputHandler.sprintFlag && inputHandler.moveAmount > 0.5f)
            {
                speed = sprintSpeed;
                playerManager.isSprinting = true;
                moveDirection *= speed;
                playerStats.TakeStaminaDamage(sprintStaminaCost * Time.deltaTime * 100);
            }
            else
            {
                if (inputHandler.moveAmount < 0.5f)
                {
                    moveDirection *= speed / 2;
                    playerManager.isSprinting = false;
                }
                else
                {
                    moveDirection *= speed;
                    playerManager.isSprinting = false;
                }
            }

            Vector3 projectedVelocity = Vector3.ProjectOnPlane(moveDirection, normalVector);
            // rigidbody.velocity = projectedVelocity;
            // rigidbody.velocity = moveDirection;
            playerManager.characterController.Move(moveDirection * Time.deltaTime);

            if (inputHandler.lockOnFlag && inputHandler.sprintFlag == false)
            {
                animatorHandler.UpdateAnimatorValues(inputHandler.vertical, inputHandler.horizontal, playerManager.isSprinting);
            }
            else
            {
                animatorHandler.UpdateAnimatorValues(inputHandler.moveAmount, 0, playerManager.isSprinting);
            }

            if (animatorHandler.canRotate)
            {
                HandleRotation();
            }
        }

        public void HandleRollingAndSprinting(float delta)
        {
            // if (animatorHandler.anim.GetBool("isInteracting"))
            //     return;

            if (playerManager.isJumping || playerManager.isInAir)
                return;

            if (playerStats.currentStamina <= 0)
                return;

            if (inputHandler.rollFlag)
            {
                if (!playerManager.canRoll)
                    return;

                if (inputHandler.moveAmount > 0)
                {
                    rollDirection = cameraObject.forward * inputHandler.vertical;
                    rollDirection += cameraObject.right * inputHandler.horizontal;
                    rollDirection.y = 0;
                    rollDirection.Normalize();

                    Quaternion rollRotation = Quaternion.LookRotation(rollDirection);
                    myTransform.rotation = rollRotation;
                    animatorHandler.PlayTargetAnimation("Roll", true);
                    playerStats.TakeStaminaDamage(rollStaminaCost);
                }
                else
                {
                    animatorHandler.PlayTargetAnimation("Backstep", true);
                    playerStats.TakeStaminaDamage(backstepStaminaCost);
                }
            }
        }

        public void HandleFalling(float delta, Vector3 moveDirection)
        {

            RaycastHit hit;
            Vector3 origin = myTransform.position;
            origin.y += groundDetectionRayStartPoint;

            if (Physics.Raycast(origin, myTransform.forward, out hit, 0.4f))
            {
                // moveDirection = Vector3.zero;
            }

            if (playerManager.isInAir)
            {
                rigidbody.AddForce(-Vector3.up * fallingSpeed * 5f);
                rigidbody.AddForce(moveDirection * fallingSpeed / 10f);
            }

            Vector3 dir = moveDirection;
            dir.Normalize();
            origin = origin + dir * groundDirectionRayDistance;

            targetPosition = myTransform.position;

            Debug.DrawRay(origin, -Vector3.up * minimumDistanceNeededToBeginFall, Color.red, 0.1f, false);
            if (Physics.Raycast(origin, -Vector3.up, out hit, minimumDistanceNeededToBeginFall, ignoreForGroundCheck))
            {
                normalVector = hit.normal;
                Vector3 tp = hit.point;
                playerManager.anim.SetBool("isGrounded", true);
                targetPosition.y = tp.y;

                if (playerManager.isInAir)
                {
                    if (inAirTimer > 0.5f)
                    {
                        Debug.Log("You were in the air for " + inAirTimer);

                        if (inAirTimer > 2)
                        {
                            playerStats.TakeDamage((int)(20 * inAirTimer), null);
                        }

                        animatorHandler.PlayTargetAnimation("Landing", true);
                        inAirTimer = 0;
                    }
                    else
                    {
                        animatorHandler.PlayTargetAnimation("Empty", false);
                        inAirTimer = 0;
                    }

                    // playerManager.isInAir = false;
                    playerManager.anim.SetBool("isInAir", false);
                }
            }
            else
            {
                if (playerManager.isClimbing)
                    return;

                if (playerManager.isGrounded)
                {
                    playerManager.anim.SetBool("isGrounded", false);
                }

                if (playerManager.isInAir == false)
                {
                    if (playerManager.isInteracting == false)
                    {
                        animatorHandler.PlayTargetAnimation("Falling", true, false, true);
                    }

                    Vector3 vel = rigidbody.velocity;
                    vel.Normalize();
                    rigidbody.velocity = vel * (movementSpeed / 2);
                    // playerManager.isInAir = true;
                    playerManager.anim.SetBool("isInAir", true);
                }
            }

            if (playerManager.isInteracting || inputHandler.moveAmount > 0)
            {
                myTransform.position = Vector3.Lerp(myTransform.position, targetPosition, Time.deltaTime / 0.1f);
            }
            else
            {
                myTransform.position = targetPosition;
            }
        }

        public void HandleJumping()
        {
            // if (playerManager.isGrounded)
            //     playerManager.isJumping = false;

            if (playerManager.isInteracting)
                return;

            if (playerStats.currentStamina <= 0)
                return;

            if (playerManager.isJumping)
                return;

            if (!playerManager.isGrounded)
                return;

            if (inputHandler.jump_Input)
            {
                animatorHandler.PlayTargetAnimation("Main_Jump_01", false);

                playerManager.anim.SetBool("isJumping", true);
                ApplyJumpingVelocity();
                // playerManager.isInteracting = true;



                // if (inputHandler.moveAmount > 0)
                // {
                //     moveDirection = cameraObject.forward * inputHandler.vertical;
                //     moveDirection += cameraObject.right * inputHandler.horizontal;
                //     // animatorHandler.PlayTargetAnimation("Jump", true);
                //     animatorHandler.PlayTargetAnimation("Main_Jump_01", true);
                //     moveDirection.y = 0;
                //     // Quaternion jumpRotation = Quaternion.LookRotation(moveDirection);
                //     // myTransform.rotation = jumpRotation;

                //     if (moveDirection != Vector3.zero)
                //     {
                //         Quaternion jumpRotation = Quaternion.LookRotation(moveDirection);
                //         transform.rotation = jumpRotation;
                //     }
                //     // playerManager.isJumping = true;
                //     playerManager.anim.SetBool("isJumping", true);
                // }
            }
        }

        public void ApplyForwardJumpForceOverTime()
        {
            if (playerManager.isJumping)
            {
                rigidbody.AddForce(2.5f * Vector3.up, ForceMode.Impulse);
                if (playerManager.isSprinting)
                {
                    // rigidbody.AddForce(Vector3.up, ForceMode.Impulse);
                    rigidbody.AddForce(moveDirection * 1.5f, ForceMode.Impulse);
                }
                else if (inputHandler.moveAmount > 0.5)
                {
                    // rigidbody.AddForce(Vector3.up, ForceMode.Impulse);
                    rigidbody.AddForce(moveDirection * 0.8f, ForceMode.Impulse);
                }
                else if (inputHandler.moveAmount <= 0.5)
                {
                    // rigidbody.AddForce(Vector3.up, ForceMode.Impulse);
                    rigidbody.AddForce(moveDirection * 0.5f, ForceMode.Impulse);
                }
            }

        }
    }
}