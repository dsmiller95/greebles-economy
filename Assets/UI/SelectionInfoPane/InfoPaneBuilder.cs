using Assets.UI.Plotter;
using UnityEngine;

namespace Assets.UI.InfoPane
{
    [RequireComponent(typeof(SelectionTracker))]
    public class InfoPaneBuilder : MonoBehaviour
    {
        public GameObject plottablePrefab;

        private SelectionTracker selectionTracker;

        private UIElementSeriesBuilder panelBuilder;

        private void Awake()
        {
            selectionTracker = GetComponent<SelectionTracker>();
            selectionTracker.SelectionChanged += OnSelectionChanged;
        }

        private void Start()
        {
            panelBuilder = new UIElementSeriesBuilder(this.gameObject);
        }

        private void OnSelectionChanged(object sender, ISelectable e)
        {
            this.panelBuilder.ClearContainer();
            var paneConfig = e.GetInfoPaneConfiguration();
            foreach (var plottableConfig in paneConfig.plottables)
            {
                var newPlottable = Instantiate(plottablePrefab, transform);

                var plotter = newPlottable.GetComponentInChildren<GraphPlotter>();
                plotter.SetPlottablesPreStart(plottableConfig.plot.GetPlottableSeries());
                this.panelBuilder.AddNextPanel(newPlottable);
            }
            if (paneConfig.uiObjects != default)
            {
                foreach (var genericUIObject in paneConfig.uiObjects)
                {
                    var newUIObject = Instantiate(genericUIObject.prefabToInit, transform);
                    genericUIObject.postInitHook(newUIObject);
                    this.panelBuilder.AddNextPanel(newUIObject);
                }
            }
        }
    }
}