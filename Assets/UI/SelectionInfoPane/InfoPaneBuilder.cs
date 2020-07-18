using Assets.UI.Plotter;
using Assets.UI.SelectionManager;
using UnityEngine;

namespace Assets.UI.InfoPane
{
    public class InfoPaneBuilder : MonoBehaviour
    {
        public GameObject plottablePrefab;
        private UIElementSeriesBuilder panelBuilder;

        private void Start()
        {
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

        public void FocusableSelected(IFocusable focusable)
        {
            SetNewPaneConfig(focusable.GetInfoPaneConfiguration());
        }
    }
}