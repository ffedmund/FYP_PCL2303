using System.Collections;
using System.Collections.Generic;
using FYP;
using UnityEngine;

public class ResetAnimatorBool : StateMachineBehaviour
{
    // public string targetBool;
    // public bool status;

    public string isInteracting = "isInteracting";
    public bool isInteractingStatus = false;

    public string canDoCombo = "canDoCombo";
    public bool canDoComboStatus = false;

    public string isUsingRightHand = "isUsingRightHand";
    public bool isUsingRightHandStatus = false;

    public string isUsingLeftHand = "isUsingLeftHand";
    public bool isUsingLeftHandStatus = false;

    public string isInvulnerable = "isInvulnerable";
    public bool isInvulnerableStatus = false;

    public string isJumping = "isJumping";
    public bool isJumpingStatus = false;

    public string isFiringSpell = "isFiringSpell";
    public bool isFiringSpellStatus = false;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // animator.SetBool(targetBool, status);

        CharacterManager character = animator.GetComponentInParent<CharacterManager>();
        
        if(character == null)
        {
            return;
        }

        character.isUsingLeftHand = false;
        character.isUsingRightHand = false;
        character.canRoll = true;

        animator.SetBool(isInteracting, isInteractingStatus);
        animator.SetBool(canDoCombo, canDoComboStatus);
        animator.SetBool(isUsingRightHand, isUsingRightHandStatus);
        animator.SetBool(isUsingLeftHand, isUsingLeftHandStatus);
        animator.SetBool(isInvulnerable, isInvulnerableStatus);
        animator.SetBool(isJumping, isJumpingStatus);
        animator.SetBool(isFiringSpell, isFiringSpellStatus);

    }
}