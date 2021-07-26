using Assets.MapGen.TileManagement;
using Assets.Scripts.MovementExtensions;
using Assets.UI.SelectionManager;
using Boo.Lang;
using Simulation.Tiling.HexCoords;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Castle
{
    public class ObjectPlacer : MonoBehaviour, ISelectionInput
    {
        public static ObjectPlacer instance;

        public GameObject placeIndicator;
        public string cameraName = "Scene Camera";
        public HexTileMapManager mapManager;

        private Camera cam;
        private Plane targetPlane;

        public void Awake()
        {
            if(instance != null)
            {
                throw new Exception("Error: ObjectPlacer is a singleton");
            }
            instance = this;
            cam = GameObject.FindGameObjectsWithTag("MainCamera")
                .Where(gameObject => gameObject.name == cameraName).First()
                .GetComponent<Camera>();

            targetPlane = new Plane(Vector3.up, new Vector3(0, 0, 0));
        }

        private bool trackObject = false;
        public void Update()
        {
            if (!this.trackObject || placeIndicator == null || !placeIndicator.activeSelf)
            {
                return;
            }
            placeIndicator.GetComponentInChildren<HexMember>().PositionInTileMap = GetCoordFromCurrentMousePos();
        }

        private Action<AxialCoordinate> onPlaced;
        private Action onExit;
        public void PlaceObject(GameObject placeObject, Action<AxialCoordinate> onPlaced, Action onExit = null)
        {
            this.placeIndicator = placeObject;
            this.onPlaced = onPlaced;
            this.onExit = onExit;
            SelectionTracker.instance.PushSelectionInput(this);
        }

        private AxialCoordinate GetCoordFromCurrentMousePos()
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);

            float planeHit;
            if (!targetPlane.Raycast(ray, out planeHit))
            {
                Debug.Log("ray from camera did not hit the floating plane");
                throw new Exception("Camera raycast did not hit infinite plane");
            }

            var pointHitOnPlane = ray.GetPoint(planeHit);
            return GetCoordinateInHexPlane(new Vector2(pointHitOnPlane.x, pointHitOnPlane.z));
        }

        private AxialCoordinate GetCoordinateInHexPlane(Vector2 pointHit)
        {
            var tilePostion = mapManager.PositionInPlaneToTilemapPosition(pointHit);
            return tilePostion;
        }

        private void BeginTracking()
        {
            this.trackObject = true;
            this.placeIndicator.SetActive(true);
        }
        private void EndTracking()
        {
            this.trackObject = false;
            this.placeIndicator.SetActive(false);
        }


        public void BeginSelectionInput()
        {
            this.BeginTracking();
        }

        public void CloseSelectionInput()
        {
            this.EndTracking();
        }

        public bool IsValidClick(GameObject o)
        {
            //todo: attempt to place?
            return true;
        }

        public bool ObjectClicked(GameObject o, RaycastHit rayHit)
        {
            var placePostion = GetCoordFromCurrentMousePos();
            this.onPlaced(placePostion);
            this.onExit?.Invoke();
            return true;
        }

        public bool Supersceded(ISelectionInput other)
        {
            // todo: stop placing the thingo at the placo
            return false;
        }
    }
}
