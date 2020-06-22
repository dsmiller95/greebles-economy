using Assets.UI.SelectionManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.UI.SelectionMananger
{
    class FocusableSelectionInput : ISelectionInput
    {
        private IFocusable currentlySelected;
        public void BeginSelection()
        {
        }

        public bool IsValidSelection(GameObject o)
        {
            return o.GetComponent<IFocusable>() != default;
        }

        public bool SelectedObject(GameObject o)
        {
            var focusable = o.GetComponent<IFocusable>();
            this.currentlySelected?.OnMeDeselected();
            focusable.OnMeSelected();
            this.currentlySelected = focusable;
            return false;
        }
    }
}
