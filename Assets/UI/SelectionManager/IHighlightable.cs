using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.UI.SelectionManager
{
    public enum HighlightState
    {
        Selected,
        None
    }
    public interface IHighlightable
    {
        void SetHighlighted(HighlightState highlighted);
    }
}
