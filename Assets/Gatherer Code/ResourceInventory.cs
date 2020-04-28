using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Gatherer_Code
{
    public class ResourceInventory
    {

        int wood = 0;
        int food = 0;

        public int getResource(ResourceType type)
        {
            switch (type)
            {
                case ResourceType.Food: return food;
                case ResourceType.Wood: return wood;
                default: throw new NotImplementedException();
            }
        }
        public int addResource(ResourceType type, int amount)
        {
            switch (type)
            {
                case ResourceType.Food: return food += amount;
                case ResourceType.Wood: return wood += amount;
                default: throw new NotImplementedException();
            }
        }
    }
}
