using Assets.UI.SelectionManager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.UI.ActionButtons
{
    public class ActionButtonPanelManager : MonoBehaviour
    {
        private GameObject lastButtonPanel;

        public void SetButtonPanelFocus(IFocusable newFocus)
        {
            if(lastButtonPanel != null)
            {
                Destroy(lastButtonPanel);
            }
            lastButtonPanel = newFocus.InstantiateButtonPanel(this.gameObject);
        }
    }
}