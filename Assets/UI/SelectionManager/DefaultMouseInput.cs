﻿using Assets.UI.ActionButtons;
using Assets.UI.InfoPane;
using UnityEngine;

namespace Assets.UI.SelectionManager
{
    public class DefaultMouseInput : MonoBehaviour, ISelectionInput
    {
        public InfoPaneBuilder paneBuilder;
        public ActionButtonPanelManager buttonPanel;

        #region Selection Managing
        private GameObject currentlySelectedObject;
        private IFocusable currentlySelectedFocusable;

        private void Start()
        {
            SelectionTracker.instance.PushSelectionInput(this);
        }

        public bool IsValidClick(GameObject o)
        {
            return o.GetComponentInParent<IClickable>() != default;
        }

        public bool ObjectClicked(GameObject o, RaycastHit hit)
        {
            var clickable = o.GetComponentInParent<IClickable>();

            if (clickable is IFocusable focusable)
            {
                Deselect();

                currentlySelectedObject = o;
                currentlySelectedFocusable = focusable;

                currentlySelectedFocusable.MeClicked(hit);
                currentlySelectedObject.GetComponentInChildren<IHighlightable>()?.SetHighlighted(HighlightState.Selected);

                paneBuilder.FocusableSelected(currentlySelectedFocusable);
                buttonPanel.SetButtonPanelFocus(currentlySelectedFocusable);
            }
            else
            {
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
