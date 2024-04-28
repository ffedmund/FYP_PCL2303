using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

namespace FYP
{
    public class AnimatorManager : MonoBehaviour
    {
        CharacterManager characterManager;
        public Animator anim;
        
        protected void Awake()
        {
            anim = GetComponent<Animator>();
            characterManager = GetComponentInParent<CharacterManager>();

            
        }

        public void PlayTargetAnimation(string targetAnim, bool isInteracting, bool canRoll = false, bool applyRootMotion = true)
        {
            anim.applyRootMotion = applyRootMotion;
            anim.SetBool("isInteracting", isInteracting);
            anim.CrossFade(targetAnim, 0.2f);

            if (characterManager != null)
                characterManager.canRoll = canRoll;
        }
    }
}

