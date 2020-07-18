using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.UI.SelectionManager
{
    public class SelectionTracker : MonoBehaviour
    {
        public static SelectionTracker instance;

        public string cameraName;

        private Camera cam;

        private Stack<ISelectionInput> inputRequests = new Stack<ISelectionInput>();

        private void Awake()
        {
            if (SelectionTracker.instance != default)
            {
                Debug.LogError("Cannot register more than one selection tracker");
            }
            SelectionTracker.instance = this;
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
                if (EventSystem.current.IsPointerOverGameObject())    // is the touch on the GUI
                {
                    // GUI Action
                    return;
                }
                var inputCommand = inputRequests.Peek();
                Ray ray = cam.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, 1000))
                {
                    var hitGameObject = hit.transform.gameObject;
                    if (inputCommand.IsValidClick(hitGameObject))
                    {
                        if (inputCommand.ObjectClicked(hitGameObject, hit))
                        {
                            PopSelectionInput();
                        }
                    }
                }
            }
        }

        private void PopSelectionInput()
        {
            inputRequests.Pop().CloseSelectionInput();
            if (inputRequests.Count <= 0)
            {
                Debug.LogWarning("Selection input stack is empty");
            }
            inputRequests.Peek().BeginSelectionInput();
        }

        public void RemoveSelectionInput(ISelectionInput input)
        {
            inputRequests = new Stack<ISelectionInput>(inputRequests.Where(x => x != input));
        }

        public void PushSelectionInputs(IList<ISelectionInput> inputs)
        {
            if (inputRequests.Count > 0)
            {
                var top = inputRequests.Peek();
                top.CloseSelectionInput();
                if (top.Supersceded(inputs[0]))
                {
                    inputRequests.Pop();
                }
            }
            foreach (var input in inputs)
            {
                inputRequests.Push(input);
            }
            inputRequests.Peek()?.BeginSelectionInput();
        }
        public void PushSelectionInput(ISelectionInput input)
        {
            if (inputRequests.Count > 0)
            {
                var top = inputRequests.Peek();
                top.CloseSelectionInput();
                if (top.Supersceded(input))
                {
                    inputRequests.Pop();
                }
            }
            inputRequests.Push(input);
            input.BeginSelectionInput();
        }
    }
}