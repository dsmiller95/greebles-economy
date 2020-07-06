using Assets.Scripts.MovementExtensions;
using Assets.UI.InfoPane;
using Assets.UI.SelectionManager;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.MapGen
{
    [RequireComponent(typeof(HexMember))]
    public class SingleHexCell : MonoBehaviour, IFocusable
    {
        public static IList<SingleHexCell> lastSelected = new List<SingleHexCell>();

        public MeshRenderer renderer;
        public Material[] materialOptions;
        public HexMember hexMember;

        private int materialIndex;

        // Start is called before the first frame update
        void Start()
        {
            materialIndex = 0;// Random.Range(0, materialOptions.Length);
            renderer.material = materialOptions[materialIndex];
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
                //DrawPathBasedOnHistory();
            }
            var myPosition = hexMember.PositionInTileMap;
            var mapManager = hexMember.MapManager;

            var cells = mapManager.GetPositionsWithinJumpDistance(myPosition, (int)5)
                .Select(position => new { position, distance = myPosition.DistanceTo(position)})
                .Where(info => info.distance % 2 == 0)
                .SelectMany(info =>
                {
                    return mapManager
                        .GetMembersAtLocation<HexMember>(info.position, member => member.GetComponent<SingleHexCell>() != null)
                        ?.Select(hexMember => hexMember.GetComponent<SingleHexCell>());

                });
            ToggleCells(cells);
        }

        private void DrawPathBasedOnHistory()
        {
            var manager = hexMember.MapManager;
            var newPath = manager.GetRouteBetweenMembers(lastSelected[0].hexMember, lastSelected[1].hexMember);

            foreach (var coord in newPath.waypoints)
            {
                var hexCell = manager.GetItemsAtLocation<SingleHexCell>(coord)?.First();
                hexCell?.ToggleMaterial();
            }
        }

        private void ToggleCells(IEnumerable<SingleHexCell> cells)
        {
            if(cells == null)
            {
                return;
            }
            foreach (var hexCell in cells)
            {
                hexCell?.ToggleMaterial();
            }
        }


        public InfoPaneConfiguration GetInfoPaneConfiguration()
        {
            return null;
        }

        public void OnMeDeselected()
        {
            Debug.Log($"{gameObject.name} deselected");
        }

        public void OnMeSelected(Vector3 pointHit)
        {
            MouseDown();
        }
    }
}