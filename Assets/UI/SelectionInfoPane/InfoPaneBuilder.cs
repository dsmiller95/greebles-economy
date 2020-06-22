using Assets.UI.Plotter;
using Assets.UI.SelectionManager;
using UnityEngine;

namespace Assets.UI.InfoPane
{
    public class InfoPaneBuilder : MonoBehaviour, ISelectionInput
    {
        public GameObject plottablePrefab;
        private UIElementSeriesBuilder panelBuilder;

        private void Start()
        {
            SelectionTracker.globalTracker.AddSelectionInput(this);
            panelBuilder = new UIElementSeriesBuilder(gameObject);
        }

        private void SetNewPaneConfig(InfoPaneConfiguration paneConfiguration)
        {
            panelBuilder.ClearContainer();
            foreach (var plottableConfig in paneConfiguration.plottables)
            {
                var newPlottable = Instantiate(plottablePrefab, transform);

                var plotter = newPlottable.GetComponentInChildren<GraphPlotter>();
                plotter.SetPlottablesPreStart(plottableConfig.plot.GetPlottableSeries());
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
        public void BeginSelectionInput()
        {
        }

        public bool IsValidSelection(GameObject o)
        {
            return o.GetComponent<IFocusable>() != default;
        }

        public bool SelectedObject(GameObject o)
        {

            currentlySelected?.GetComponent<IHighlightable>()?.SetHighlighted(HighlightState.None);
            currentlySelected?.GetComponent<IFocusable>().OnMeDeselected();

            currentlySelected = o;

            var currentFocusable = currentlySelected.GetComponent<IFocusable>();
            currentFocusable.OnMeSelected();
            currentlySelected.GetComponent<IHighlightable>()?.SetHighlighted(HighlightState.Selected);

            this.SetNewPaneConfig(currentFocusable.GetInfoPaneConfiguration());
            return false;
        }
        #endregion
    }
}