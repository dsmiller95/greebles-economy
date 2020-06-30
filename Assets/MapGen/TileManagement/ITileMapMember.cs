using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering;
using static Assets.MapGen.TileManagement.HexTileMapManager;

namespace Assets.MapGen.TileManagement
{
    public interface ITileMapMember
    {
        void SetMapItem(TileMapItem item);
        void UpdateWorldSpace();
    }
}
