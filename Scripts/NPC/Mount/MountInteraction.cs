using FYP;
using UnityEngine;
using UnityEngine.AI;

public class MountInteraction : NPCInteraction {
    [SerializeField] Saddle _saddle;
    Tilt _tilt;
    bool notInManagerControl;

    public override void Awake()
    {
        base.Awake();
        _tilt = GetComponent<Tilt>();
    }

    public override void Setup(NPC npc, Animator animator, Firendship firendship)
    {
        base.Setup(npc, animator, firendship);
        if(_saddle == null)
        {
            _saddle = GetComponentInChildren<Saddle>();
            _saddle._animator = animator;
            _saddle._agent = GetComponent<NavMeshAgent>();
            _saddle.mountController = GetComponent<CharacterController>();
        }
    }

    public override void Interact(PlayerManager playerManager)
    {
        if(isInteracting)
        {
            Leave(playerManager);
        }
        else
        {
            Ride(playerManager);
        }
    }

    void Ride(PlayerManager playerManager)
    {
        isInteracting = true;
        _saddle.SetRider(playerManager);
        interactableText = "Leave";
        _tilt.enabled = false;
        if(TerrainGenerationManager.creatureLairGenerator != null && !notInManagerControl)
        {
            TerrainGenerationManager.creatureLairGenerator.RemoveCreatureFromManager(gameObject);
            notInManagerControl = true;
        }
    }

    void Leave(PlayerManager playerManager)
    {
        isInteracting = false;
        _saddle.RemoveRider(playerManager);
        interactableText = "Ride";
        _tilt.enabled = true;
    }
}