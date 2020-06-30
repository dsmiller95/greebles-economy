using Assets.MapGen.TileManagement;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Assets.MapGen.TileManagement.HexTileMapManager;

namespace Assets.MapGen
{
    public class SingleHexCell : MonoBehaviour, ITilemapMember
    {
        public static IList<SingleHexCell> lastSelected = new List<SingleHexCell>();

        public MeshRenderer renderer;
        public Material[] materialOptions;
        private int materialIndex;

        private Camera cam;
        // Start is called before the first frame update
        void Start()
        {
            materialIndex = 0;// Random.Range(0, materialOptions.Length);
            renderer.material = materialOptions[materialIndex];

            cam = GameObject.FindGameObjectsWithTag("MainCamera")
                .Where(gameObject => gameObject.name == "Scene Camera").First()
                .GetComponent<Camera>();
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                //Debug.Log("mousedown");
                Ray ray = cam.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, 1000))
                {
                    var hitGameObject = hit.transform.gameObject;
                    //Debug.Log($"found hit{hitGameObject.name}");
                    if (hitGameObject.transform.parent.gameObject == gameObject)
                    {
                        MouseDown();
                    }
                }
            }
        }

        private void ToggleMaterial()
        {
            materialIndex = (materialIndex + 1) % materialOptions.Length;
            renderer.material = materialOptions[materialIndex];
        }

        private void MouseDown()
        {
            Debug.Log("mousedown detected");
            lastSelected.Add(this);
            if (lastSelected.Count > 2)
            {
                lastSelected.RemoveAt(0);
            }
            if (lastSelected.Count == 2)
            {
                var manager = myItem.MapManager;
                var newPath = manager.GetRouteBetweenMembers(lastSelected[0].myItem, lastSelected[1].myItem);

                foreach (var coord in newPath)
                {
                    var hexCell = manager.GetItemsAtLocation<SingleHexCell>(coord).First();
                    hexCell.ToggleMaterial();
                }
            }
        }

        private TileMapItem myItem;
        public void SetMapItem(TileMapItem item)
        {
            myItem = item;
        }

        public void UpdateWorldSpace()
        {
            var placeSpace = myItem.PositionInTilePlane;
            var worldPlacement = new Vector3(placeSpace.x, 0, placeSpace.y);
            transform.localPosition = worldPlacement;
        }
    }
}