using System.Collections.Generic;
using FYP;
using UnityEngine;

public abstract class PlayerSetupBehaviour : MonoBehaviour
{
    public abstract void Setup(PlayerManager playerManager);

    private static readonly HashSet<PlayerSetupBehaviour> instances = new HashSet<PlayerSetupBehaviour>();
    public static HashSet<PlayerSetupBehaviour> Instances => new HashSet<PlayerSetupBehaviour>(instances);

    protected virtual void Awake()
    {
        instances.Add(this);
    }

    protected virtual void OnDestroy()
    {
        instances.Remove(this);
    }
}