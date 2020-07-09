using Assets.Utilities.TreeEventSystem;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Utilities.MyEventSystem
{
    class TreeEventNode : MonoBehaviour
    {
        private TreeEventNode parentEventNode;

        private IDictionary<TreeEventType, ITreeEventListener> eventListeners;


        public void Awake()
        {
            parentEventNode = transform.parent.GetComponentInParent<TreeEventNode>();
            this.eventListeners = new Dictionary<TreeEventType, ITreeEventListener>();
        }

        public void RecievedEvent(ITreeEvent e)
        {
            var stopPropigation = eventListeners[e.EventType]?.OnEvent(e);
            if (stopPropigation.HasValue && stopPropigation.Value)
            {
                return;
            }
            this.parentEventNode?.RecievedEvent(e);
        }

        public void RegisterListener(ITreeEventListener listener)
        {
            eventListeners[listener.EventType] = listener;
        }

        public void DeRegisterListener(ITreeEventListener listener)
        {
            eventListeners.Remove(listener.EventType);
        }
    }
}
