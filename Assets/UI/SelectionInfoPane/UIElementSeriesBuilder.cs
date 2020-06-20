using System.Collections.Generic;
using UnityEngine;

namespace Assets.UI.InfoPane
{
    class UIElementSeriesBuilder
    {
        private GameObject container;

        private float uiElementVerticalOffset;

        public UIElementSeriesBuilder(GameObject container)
        {
            this.container = container;
            uiElementVerticalOffset = 0;
        }

        public void AddNextPanel(GameObject panel)
        {
            var positioning = panel.GetComponentInChildren<RectTransform>();
            positioning.position -= new Vector3(0, uiElementVerticalOffset);
            uiElementVerticalOffset += positioning.sizeDelta.y;
        }

        public void AddAllPanels(IEnumerable<GameObject> panels)
        {
            foreach (var panel in panels)
            {
                AddNextPanel(panel);
            }
        }

        public void ClearContainer()
        {
            // transform does not implement a generic IEnumerable,
            //   but it will return all transform children when iterated
            foreach (Transform child in container.transform)
            {
                GameObject.Destroy(child.gameObject);
            }
            uiElementVerticalOffset = 0;
        }
    }
}
