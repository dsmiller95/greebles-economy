using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.UI.SelectionManager
{
    public class SelectionTracker : MonoBehaviour
    {
        public static SelectionTracker globalTracker;

        public string cameraName;

        public event EventHandler<GameObject> SelectionChanged;
        private Camera cam;

        private Stack<ISelectionInput> inputRequests = new Stack<ISelectionInput>();

        private void Awake()
        {
            if (SelectionTracker.globalTracker != default)
            {
                Debug.LogError("Cannot register more than one selection tracker");
            }
            SelectionTracker.globalTracker = this;
        }

        // Start is called before the first frame update
        void Start()
        {
            cam = GameObject.FindGameObjectsWithTag("MainCamera")
                .Where(gameObject => gameObject.name == cameraName).First()
                .GetComponent<Camera>();
            //inputRequests.Push(new FocusableSelectionInput());
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                var inputCommand = inputRequests.Peek();
                Ray ray = cam.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, 1000))
                {
                    var hitGameObject = hit.transform.gameObject;
                    if (inputCommand.IsValidSelection(hitGameObject))
                    {
                        if (inputCommand.SelectedObject(hitGameObject))
                        {
                            inputRequests.Pop();
                            if (inputRequests.Count <= 0)
                            {
                                Debug.LogWarning("Selection input stack is empty");
                            }
                        }
                        SelectionChanged?.Invoke(this, hitGameObject);
                    }
                }
            }
        }
        public void AddSelectionInput(ISelectionInput input)
        {
            inputRequests.Push(input);
        }
    }
}