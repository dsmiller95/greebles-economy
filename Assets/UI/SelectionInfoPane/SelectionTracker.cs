using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.UI.InfoPane
{
    public class SelectionTracker : MonoBehaviour
    {
        public string cameraName;


        private ISelectable currentlySelected;
        public event EventHandler<ISelectable> SelectionChanged;
        private Camera cam;
        // Start is called before the first frame update
        void Start()
        {
            this.cam = GameObject.FindGameObjectsWithTag("MainCamera")
                .Where(gameObject => gameObject.name == this.cameraName).First()
                .GetComponent<Camera>();
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = cam.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, 1000))
                {
                    var hitGameObject = hit.transform.gameObject;
                    var selectable = hitGameObject.GetComponent<ISelectable>();
                    if (selectable != default)
                    {
                        this.currentlySelected?.OnMeDeselected();
                        selectable.OnMeSelected();
                        this.currentlySelected = selectable;
                        SelectionChanged?.Invoke(this, this.currentlySelected);
                    }
                }
            }
        }
    }
}