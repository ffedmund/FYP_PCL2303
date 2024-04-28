using UnityEngine;

public class NavigationTarget : MonoBehaviour {
    [SerializeField] string id;

    void Start(){
        if(TryGetComponent(out NPCController npcController)){
            id = npcController.npc.npcName;
        }
        NavigationSystem.instance?.AddNavigationTarget(id,transform.position);
    }

    void OnDisable() {
        NavigationSystem.instance?.RemoveNavigationTarget(id,transform.position);
    }
}