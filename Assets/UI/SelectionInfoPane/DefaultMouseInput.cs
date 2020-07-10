using Assets.UI.InfoPane;
using Assets.UI.SelectionManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.UI.SelectionInfoPane
{
    public class DefaultMouseInput : MonoBehaviour, ISelectionInput
    {
        public InfoPaneBuilder paneBuilder;

        #region Selection Managing
        private GameObject currentlySelected;

        public bool IsValidSelection(GameObject o)
        {
            return o.GetComponentInParent<IFocusable>() != default;
        }

        public bool SelectedObject(GameObject o, RaycastHit hit)
        {
            // GameObject overloads this. Will equate to null when it has been destroyed, even if it's not actually null
            if (currentlySelected != null)
            {
                currentlySelected.GetComponentInChildren<IHighlightable>()?.SetHighlighted(HighlightState.None);
                currentlySelected.GetComponentInParent<IFocusable>().OnMeDeselected();
            }

            currentlySelected = o;

            var currentFocusable = currentlySelected.GetComponentInParent<IFocusable>();
            currentFocusable.OnMeSelected(hit.point);
            currentlySelected.GetComponentInChildren<IHighlightable>()?.SetHighlighted(HighlightState.Selected);

            paneBuilder.FocusableSelected(currentFocusable);
            return false;
        }

        public void BeginSelectionInput() { }
        public void CloseSelectionInput() { }

        public bool Supersceded(ISelectionInput other)
        {
            return false;
        }
        #endregion
    }
}
