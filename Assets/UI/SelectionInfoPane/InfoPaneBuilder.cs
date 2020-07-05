using Assets.UI.Plotter;
using Assets.UI.SelectionManager;
using System.Linq;
using UnityEngine;

namespace Assets.UI.InfoPane
{
    public class InfoPaneBuilder : MonoBehaviour, ISelectionInput
    {
        public GameObject plottablePrefab;
        private UIElementSeriesBuilder panelBuilder;

        private void Start()
        {
            SelectionTracker.globalTracker.PushSelectionInput(this);
            panelBuilder = new UIElementSeriesBuilder(gameObject);
        }

        private void SetNewPaneConfig(InfoPaneConfiguration paneConfiguration)
        {
            panelBuilder.ClearContainer();
            if(paneConfiguration == null)
            {
                return;
            }
            foreach (var plottableConfig in paneConfiguration.plottables)
            {
                var newPlottable = Instantiate(plottablePrefab, transform);

                var plotter = newPlottable.GetComponentInChildren<GraphPlotter>();
                var series = plottableConfig.plot.GetPlottableSeries();
                plotter.SetPlottablesPreStart(series);
                panelBuilder.AddNextPanel(newPlottable);
            }
            if (paneConfiguration.uiObjects != default)
            {
                foreach (var genericUIObject in paneConfiguration.uiObjects)
                {
                    var newUIObject = Instantiate(genericUIObject.prefabToInit, transform);
                    genericUIObject.postInitHook(newUIObject);
                    panelBuilder.AddNextPanel(newUIObject);
                }
            }
        }

        #region Selection Managing
        private GameObject currentlySelected;

        public bool IsValidSelection(GameObject o)
        {
            return o.GetComponentInParent<IFocusable>() != default;
        }

        public bool SelectedObject(GameObject o, RaycastHit hit)
        {
            // GameObject overloads this. Will equate to null when it has been destroyed, even if it's not actually null
            if(currentlySelected != null)
            {
                currentlySelected.GetComponentInChildren<IHighlightable>()?.SetHighlighted(HighlightState.None);
                currentlySelected.GetComponentInParent<IFocusable>().OnMeDeselected();
            }

            currentlySelected = o;

            var currentFocusable = currentlySelected.GetComponentInParent<IFocusable>();
            currentFocusable.OnMeSelected(hit.point);
            currentlySelected.GetComponentInChildren<IHighlightable>()?.SetHighlighted(HighlightState.Selected);

            SetNewPaneConfig(currentFocusable.GetInfoPaneConfiguration());
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