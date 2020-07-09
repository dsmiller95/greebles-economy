using Boo.Lang;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Utilities.TreeEventSystem
{
    public interface ITreeEventListener
    {
        TreeEventType EventType { get; }
        bool OnEvent(ITreeEvent e);
    }

    public class TreeEventListener<T> : ITreeEventListener where T : ITreeEvent
    {
        private Func<T, bool> onEvent;

        public TreeEventListener(Func<T, bool> onEvent, TreeEventType type)
        {
            this.onEvent = onEvent;
            this.EventType = type;
        }

        public TreeEventType EventType { get; }

        public bool OnEvent(ITreeEvent e)
        {
            if(e is T typedEvent)
            {
                return onEvent(typedEvent);
            }
            return false;
        }
    }
}
