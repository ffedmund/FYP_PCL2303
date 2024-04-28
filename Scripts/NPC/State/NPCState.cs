using UnityEngine;

public abstract class NPCState{
    public abstract NPCState Update(NPCStateController stateController);
}