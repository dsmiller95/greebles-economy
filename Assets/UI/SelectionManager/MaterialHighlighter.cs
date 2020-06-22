using UnityEngine;


namespace Assets.UI.SelectionManager
{
    [RequireComponent(typeof(MeshRenderer))]
    public class MaterialHighlighter : MonoBehaviour, IHighlightable
    {
        public Material baseMaterial;
        public Material selectedMaterial;
        public Material outlineMaterial;

        private Renderer outlineRenderer;

        // Start is called before the first frame update
        void Start()
        {
            if (outlineMaterial)
            {
                var outlineObject = GameObject.Instantiate(gameObject, transform);
                Destroy(outlineObject.GetComponent<MaterialHighlighter>()); //.enabled = false;

                outlineRenderer = outlineObject.GetComponent<Renderer>();
                outlineRenderer.material = outlineMaterial;
                // TODO: paramaterize
                //rend.material.SetColor("_OutlineColor", color);
                //rend.material.SetFloat("_ScaleFactor", scaleFactor);
                outlineRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                outlineRenderer.enabled = false;
            }
        }

        // Update is called once per frame
        void Update()
        {

        }
        public void SetHighlighted(HighlightState highlighted)
        {
            switch (highlighted)
            {
                case HighlightState.None:
                    SetOutline(false);
                    GetComponent<MeshRenderer>().material = baseMaterial;
                    break;
                case HighlightState.Selected:
                    SetOutline(false);
                    GetComponent<MeshRenderer>().material = selectedMaterial;
                    break;
                case HighlightState.CanBeSelected:
                    SetOutline(true);
                    //if (outlineMaterial)
                    //{
                    //    GetComponent<MeshRenderer>().material = outlineMaterial;
                    //}
                    break;
            }
        }

        private void SetOutline(bool hasOutline)
        {
            if (outlineRenderer != default)
            {
                outlineRenderer.enabled = hasOutline;
            }
        }
    }
}