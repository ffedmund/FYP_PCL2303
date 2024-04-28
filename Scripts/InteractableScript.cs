using UnityEngine.EventSystems;
using UnityEngine;
using DG.Tweening;
using Unity.Netcode;

namespace FYP
{
    public class InteractableScript : MonoBehaviour
    {
        public float radius = 0.6f;
        public string interactableText;

        [SerializeField] protected GameObject targetUIWindow;
        [SerializeField] protected bool isUITrigger;
        [SerializeField] protected UIController canvasUIController;
        public EventTrigger.TriggerEvent customCallback;

        private void OnDrawGizmosSelected() {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, radius);
        }

        private void OnDestroy() {
            if(DOTween.IsTweening(transform.gameObject)){
                DOTween.Kill(transform.gameObject);
            }
        }

        public virtual void Interact(PlayerManager playerManager) {
            if(customCallback != null){
                BaseEventData eventData = new BaseEventData(EventSystem.current)
                {
                    selectedObject = playerManager?playerManager.gameObject:null
                };
                customCallback.Invoke(eventData);
            }
            if(playerManager){
                if(isUITrigger && targetUIWindow.GetComponent<RectTransform>()){
                    targetUIWindow.SetActive(true);
                }
                if(TryGetComponent(out NetworkInteraction networkInteraction) && NetworkManager.Singleton){
                    networkInteraction.InteractServerRPC(NetworkManager.Singleton.LocalClientId);
                }
            }
        }
    }
}