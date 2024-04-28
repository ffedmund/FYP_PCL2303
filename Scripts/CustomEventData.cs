using FYP;
using UnityEngine.EventSystems;

namespace FYP
{
    public class CustomEventData : BaseEventData{
        public PlayerManager playerManager;

        public CustomEventData(EventSystem eventSystem) : base(eventSystem){}
    }
}