using Assets.UI.InfoPane;
using Assets.UI.SelectionManager;
using UnityEngine;

namespace Assets.UI.SelectionInfoPane
{
    public class DefaultMouseInput : MonoBehaviour, ISelectionInput
    {
        public InfoPaneBuilder paneBuilder;

        #region Selection Managing
        private GameObject currentlySelectedObject;
        private IFocusable currentlySelectedFocusable;

        private void Start()
        {
            SelectionTracker.globalTracker.PushSelectionInput(this);
        }

        public bool IsValidSelection(GameObject o)
        {
            return o.GetComponentInParent<IClickable>() != default;
        }

        public bool SelectedObject(GameObject o, RaycastHit hit)
        {
            var clickable = o.GetComponentInParent<IClickable>();

            if(clickable is IFocusable focusable)
            {
                this.Deselect();

                currentlySelectedObject = o;
                currentlySelectedFocusable = focusable;

                currentlySelectedFocusable.MeClicked(hit);
                currentlySelectedObject.GetComponentInChildren<IHighlightable>()?.SetHighlighted(HighlightState.Selected);


                paneBuilder.FocusableSelected(currentlySelectedFocusable);
            }else
            {
                Debug.Log("I've been clicked!!!!");
                //this.Deselect();
                clickable.MeClicked(hit);
            }

            return false;
        }

        private void Deselect()
        {
            // GameObject overloads this. Will equate to null when it has been destroyed, even if it's not actually null
            if (currentlySelectedObject != null)
            {
                currentlySelectedObject.GetComponentInChildren<IHighlightable>()?.SetHighlighted(HighlightState.None);
                currentlySelectedFocusable.OnMeDeselected();
            }
            currentlySelectedFocusable = null;
            currentlySelectedObject = null;
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
