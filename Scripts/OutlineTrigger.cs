using UnityEngine;

public class OutlineTrigger : MonoBehaviour {
    Outline _outline;
    [SerializeField]bool outlineLock;
    void Start(){
        _outline = transform.parent.GetComponentInChildren<Outline>(true);
        _outline.enabled = false;
    }

    public void LockOutline(){
        outlineLock = true;
        _outline.enabled = false;
    }

    public void UnLockOutline(){
        outlineLock = false;
        _outline.enabled = true;
    }

    private void OnTriggerEnter(Collider other) {
        _outline.enabled = true && !outlineLock;
    }

    private void OnTriggerExit(Collider other) {
        _outline.enabled = false;
    }
}