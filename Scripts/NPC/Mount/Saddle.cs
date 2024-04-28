using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using FYP;
using UnityEngine;
using UnityEngine.AI;

public class Saddle : MonoBehaviour
{
    public CharacterController mountController;
    public NavMeshAgent _agent;
    public Animator _animator;
    [SerializeField] Transform characterBindPoint;
    [SerializeField] LayerMask layerMask;
    [Header("Mount Stats")]
    [SerializeField] int dexterity = 50;
    [SerializeField] float extraWalkSpeed = 5f;

    PlayerManager riddingPlayer;
    Transform riderCamera;
    bool isJumping;
    bool isSprintingInPast;
    float originalFoV;

    public void SetRider(PlayerManager player)
    {
        Transform rider = player.transform;
        rider.SetParent(characterBindPoint);
        rider.localPosition = new Vector3(0,-player.playerLocomotion.characterCollider.height/2,0);
        rider.rotation = Quaternion.identity;
        riddingPlayer = player;
        riderCamera = player.cameraHandler.transform;
        originalFoV = player.cameraHandler.cameraTransform.GetComponent<Camera>().fieldOfView;

        player.characterController.enabled = false;
        mountController.enabled = true;
        player.characterController = mountController;

        _agent.enabled = false;

        UpdatePlayerMovementSpeed(player);
    }

    public void RemoveRider(PlayerManager player)
    {
        riddingPlayer.canAttack = true;
        Transform rider = player.transform;
        rider.SetParent(null);
        riddingPlayer = null;
        riderCamera = null;
        originalFoV = player.cameraHandler.cameraTransform.GetComponent<Camera>().fieldOfView;

        player.characterController = player.GetComponent<CharacterController>();
        player.characterController.enabled = true;
        player.anim.Play("Falling");
        player.anim.Play("Both Arms Empty");
        mountController.enabled = false;

        _agent.enabled = true;
        _animator.SetFloat("speed",0);

        RaycastHit hit;
        if (Physics.Raycast(mountController.transform.position, -Vector3.up, out hit, 200f)) 
        {
            mountController.transform.position = new Vector3(transform.position.x,hit.point.y,transform.position.z);
        }

        ResetPlayerMovementSpeed(player);
    }

    void Update()
    {
        if(riddingPlayer)
        {
            riddingPlayer.canAttack = false;
            riddingPlayer.anim.Play("Ride");
            HandleJump();
            isJumping = _animator.GetBool("isJumping");
            HandleMovement();
            SetCameraEffect();
        }
    }

    void UpdatePlayerMovementSpeed(PlayerManager player)
    {
        player.playerLocomotion.movementSpeed += extraWalkSpeed;
        player.playerStats.extraDexterity += Mathf.Max(dexterity - player.playerStats.dexterityLevel,0);
        player.playerLocomotion.sprintStaminaCost = 0.1f;
        player.playerStats.UpdateSprintSpeed();
    }

    void ResetPlayerMovementSpeed(PlayerManager player)
    {
        player.playerLocomotion.movementSpeed -= extraWalkSpeed;
        player.playerStats.extraDexterity -= Mathf.Max(dexterity - player.playerStats.dexterityLevel,0);;
        player.playerLocomotion.sprintStaminaCost = 0.5f;
        player.playerStats.UpdateSprintSpeed();
    }


    private void HandleJump()
    {
        if(riddingPlayer.inputHandler.jump_Input && riddingPlayer.isGrounded && !riddingPlayer.isJumping)
        {
            _animator.Play("Jump");
            _animator.SetBool("isJumping",true);
            riddingPlayer.anim.SetBool("isJumping",true);
        }
        else if(!isJumping && riddingPlayer.isGrounded && riddingPlayer.isJumping)
        {
            riddingPlayer.anim.SetBool("isJumping",false);
        }
    }

    private void HandleMovement()
    {
        float verticalMovement = riddingPlayer.inputHandler.vertical;
        float horizontalMovement = riddingPlayer.inputHandler.horizontal;
        float speed = _agent.speed * (riddingPlayer.isSprinting?3:1);
        Vector3 moveDirection;
        moveDirection = riderCamera.forward * verticalMovement;
        moveDirection += riderCamera.right * horizontalMovement;
        moveDirection.Normalize();
        moveDirection.y = 0;
        _animator.SetFloat("speed",riddingPlayer.inputHandler.moveAmount * speed);
        _animator.SetFloat("direction",horizontalMovement == 0?0.5f:horizontalMovement > 0?0:1);
        if (moveDirection != Vector3.zero)
        {
            Quaternion toRotation = Quaternion.LookRotation(moveDirection, Vector3.up);
            mountController.transform.rotation = Quaternion.Lerp(mountController.transform.rotation, toRotation, speed * Time.deltaTime);
        }

        riddingPlayer.transform.forward = mountController.transform.forward;
    }

    void SetCameraEffect()
    {
        if(!isSprintingInPast && riddingPlayer.isSprinting)
        {
            // riderCamera.GetComponentInChildren<Camera>().fieldOfView;
            DOTween.Kill("fov");
            DOTween.To(()=>Camera.main.fieldOfView,x=>Camera.main.fieldOfView = x,65,0.5f).SetId("fov");
            isSprintingInPast = true;
        }
        else if(isSprintingInPast && !riddingPlayer.isSprinting)
        {
            DOTween.Kill("fov");
            DOTween.To(()=>Camera.main.fieldOfView,x=>Camera.main.fieldOfView = x,originalFoV,0.5f).SetId("fov");
            isSprintingInPast = false;
        }
    }
}
