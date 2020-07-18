using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.UI.SelectionManager.GetObjectSelector
{
    public class ObjectSelectionCancelledException : Exception
    {
        public ObjectSelectionCancelledException(string description) : base(description)
        {

        }
    }

    public static class SingleObjectSelectionTrackerExtension
    {
        public static Task<T> GetInputAsync<T>(this SelectionTracker tracker, Func<T, bool> filter)
        {
            var taskCompletor = new TaskCompletionSource<T>();
            var highlightSelector = new SingleObjectHighlightSelector<T>(filter, (item) =>
            {
                taskCompletor.SetResult(item);
            }, () =>
            {
                taskCompletor.SetException(new ObjectSelectionCancelledException("Object selection overwritten by another input option"));
            });
            tracker.PushSelectionInput(highlightSelector);

            return taskCompletor.Task;
        }
    }

    public class SingleObjectHighlightSelector<T> : ISelectionInput
    {
        private Func<T, bool> filter;
        private Action<T> itemSelected;
        private Action itemSupersceded;
        public SingleObjectHighlightSelector(Func<T, bool> filter, Action<T> itemSelected, Action itemSupersceded = null)
        {
            this.filter = filter;
            this.itemSelected = itemSelected;
            this.itemSupersceded = itemSupersceded;
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

        public bool IsValidClick(GameObject o)
        {
            var obj = o.GetComponent<T>();
            return obj != null && filter(obj);
        }

        public bool ObjectClicked(GameObject o, RaycastHit rayHit)
        {
            itemSelected(o.GetComponent<T>());
            return true;
        }

        public bool Supersceded(ISelectionInput other)
        {
            itemSupersceded?.Invoke();
            return true;
        }
    }
}
