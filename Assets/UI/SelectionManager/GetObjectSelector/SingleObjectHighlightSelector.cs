using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.UI.SelectionManager.GetObjectSelector
{
    class SingleObjectHighlightSelector<T> : ISelectionInput
    {
        private Func<T, bool> filter;
        private Action<T> itemSelected;
        public SingleObjectHighlightSelector(Func<T, bool> filter, Action<T> itemSelected)
        {
            this.filter = filter;
            this.itemSelected = itemSelected;
        }

        private IList<IHighlightable> highlightedObjects;
        public void BeginSelectionInput()
        {
            highlightedObjects = GameObject.FindObjectsOfType<MonoBehaviour>()
                .OfType<T>()
                .Where(filter)
                .Select(obj => (obj as MonoBehaviour).GetComponentInChildren<IHighlightable>())
                .ToList();
            foreach (var obj in highlightedObjects)
            {
                obj.SetHighlighted(HighlightState.CanBeSelected);
            }
        }

        public void CloseSelectionInput()
        {
            foreach (var obj in highlightedObjects)
            {
                obj.SetHighlighted(HighlightState.None);
            }
        }

        public bool IsValidSelection(GameObject o)
        {
            var obj = o.GetComponent<T>();
            return obj != null && filter(obj);
        }

        public bool SelectedObject(GameObject o)
        {
            itemSelected(o.GetComponent<T>());
            return true;
        }

        public bool Supersceded(ISelectionInput other)
        {
            return true;
        }
    }
}
