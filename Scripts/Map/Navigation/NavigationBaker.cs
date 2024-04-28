using Unity.AI.Navigation;
using UnityEngine;

[RequireComponent(typeof(NavMeshSurface))]
public class NavigationBaker : MonoBehaviour{
    public static NavigationBaker instance;
    NavMeshSurface surface;
    int previousChildCount = 0;

    public void Awake(){
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    void Start()
    {
        surface = GetComponent<NavMeshSurface>();
    }

    private void LateUpdate() {
        if(transform.childCount != previousChildCount)
        {
            surface.BuildNavMesh();
            previousChildCount = transform.childCount;
        }
    }

    void OnDestroy(){
        instance = null;
    }
}