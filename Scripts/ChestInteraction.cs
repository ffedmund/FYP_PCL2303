using System.Collections;
using UnityEngine;
using FYP;
using DG.Tweening;

public class ChestInteraction : InteractableScript
{
    public const string aniamtionClipName = "Fantasy_Polygon_Chest_Animation_Open";
    public const string initClipName = "Fantasy_Polygon_Chest_Animation_Close";

    public int awardNumber;
    public LootDropHandler lootDropHandler;
    OutlineTrigger outlineTrigger;
    Animator m_animator;
    Collider m_collider;

    public virtual void Awake(){
        m_collider = GetComponent<Collider>();
        outlineTrigger = GetComponentInChildren<OutlineTrigger>();
        m_animator = GetComponent<Animator>();
    }

    void OnEnable(){
        m_animator.Play(initClipName);
    }

    void OnDisable() {
        m_collider.enabled = true;
    }

    public void SetUp(LootList lootList) {
        lootDropHandler = GetComponent<LootDropHandler>();
        lootDropHandler.SetLootList(lootList);
    }

    public override void Interact(PlayerManager playerManager)
    {
        base.Interact(playerManager);
        m_collider.enabled = false;
        StartCoroutine(OpenChest(lootDropHandler));
    }

    IEnumerator OpenChest(LootDropHandler lootDropHandler)
    {
        if(m_animator)
        {
            AnimationClip clip = null;
            foreach(AnimationClip animationClip in m_animator.runtimeAnimatorController.animationClips){
                if(animationClip.name == aniamtionClipName){
                    clip = animationClip;
                    break;
                }
            }
            m_animator.Play(aniamtionClipName);
            yield return new WaitForSeconds(clip.length-1f);
        }
        outlineTrigger.LockOutline();
        lootDropHandler.DropLoot();
    }
}
