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
            return o.GetComponent<IFocusable>() != default;
        }

        public bool SelectedObject(GameObject o)
        {

            currentlySelected?.GetComponentInChildren<IHighlightable>()?.SetHighlighted(HighlightState.None);
            currentlySelected?.GetComponent<IFocusable>().OnMeDeselected();

            currentlySelected = o;

            var currentFocusable = currentlySelected.GetComponent<IFocusable>();
            currentFocusable.OnMeSelected();
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