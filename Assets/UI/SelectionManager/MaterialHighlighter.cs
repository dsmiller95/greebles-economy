using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Assets.UI.SelectionManager
{
    [RequireComponent(typeof(MeshRenderer))]
    public class MaterialHighlighter : MonoBehaviour, IHighlightable
    {
        public Material baseMaterial;
        public Material selectedMaterial;

        // Start is called before the first frame update
        void Start()
        {

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
                    GetComponent<MeshRenderer>().material = baseMaterial;
                    break;
                case HighlightState.Selected:
                    GetComponent<MeshRenderer>().material = selectedMaterial;
                    break;
            }
        }
    }
}