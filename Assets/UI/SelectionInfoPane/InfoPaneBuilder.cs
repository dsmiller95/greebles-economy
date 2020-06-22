using Assets.UI.Plotter;
using Assets.UI.SelectionManager;
using Assets.UI.SelectionMananger;
using UnityEngine;

namespace Assets.UI.InfoPane
{
    [RequireComponent(typeof(SelectionTracker))]
    public class InfoPaneBuilder : MonoBehaviour, ISelectionInput
    {
        public GameObject plottablePrefab;
        private SelectionTracker selectionTracker;
        private UIElementSeriesBuilder panelBuilder;

        private void Awake()
        {
            selectionTracker = GetComponent<SelectionTracker>();
            selectionTracker.AddSelectionInput(this);
        }

        private void Start()
        {
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
        private IFocusable currentlySelected;
        public void BeginSelection()
        {
        }

        public bool IsValidSelection(GameObject o)
        {
            return o.GetComponent<IFocusable>() != default;
        }

        public bool SelectedObject(GameObject o)
        {
            this.currentlySelected?.OnMeDeselected();

            var focusable = o.GetComponent<IFocusable>();
            focusable.OnMeSelected();
            this.currentlySelected = focusable;

            this.SetNewPaneConfig(focusable.GetInfoPaneConfiguration());
            return false;
        }
        #endregion
    }
}