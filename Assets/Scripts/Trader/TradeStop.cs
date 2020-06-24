using Assets.Scripts.Resources;
using TradeModeling.Inventories;
using UnityEngine;

namespace Assets.Scripts.Trader
{
    public abstract class TradeStop : MonoBehaviour
    {
        public abstract SpaceFillingInventory<ResourceType> tradeInventory { get; }
    }
}
