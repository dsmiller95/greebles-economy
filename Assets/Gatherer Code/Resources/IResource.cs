using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using TradeModeling.Inventories;

interface IResource
{
    ResourceType _type { get; }
    float amount { get; }
    Task Eat(SpaceFillingInventory<ResourceType> inventory, float amount = -1);
}
