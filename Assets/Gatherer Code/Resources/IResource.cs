using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradeModeling.Inventories;

interface IResource
{
    ResourceType _type { get; }
    void Eat(SpaceFillingInventory<ResourceType> inventory, float amount = -1);
}
