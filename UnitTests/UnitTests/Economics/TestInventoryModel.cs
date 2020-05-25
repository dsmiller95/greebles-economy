using Assets.Economics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTests.Economics
{
    class TestInventoryModel : IExchangeInventory
    {
        private Dictionary<string, float> selfInventory;
        public float selfBank;
        private Dictionary<string, float> marketInventory;
        public TestInventoryModel(
            (string, float)[] selfInventory,
            (string, float)[] marketInventory,
            float selfBank)
        {
            this.selfInventory = selfInventory.ToDictionary(x => x.Item1, x => x.Item2);
            this.marketInventory = marketInventory.ToDictionary(x => x.Item1, x => x.Item2);
            this.selfBank = selfBank;
        }

        private TestInventoryModel(TestInventoryModel other)
        {
            this.selfInventory = new Dictionary<string, float>(other.selfInventory);
            this.marketInventory = new Dictionary<string, float>(other.marketInventory);
            this.selfBank = other.selfBank;
        }

        public float GetSelf(string resource)
        {
            return this.selfInventory[resource];
        }
        public void AddSelf(string resource, float amount)
        {
            this.selfInventory[resource] += amount;
            if (this.selfInventory[resource] < 0)
            {
                throw new Exception($"Self inventory went below 0 on {resource}");
            }
        }
        public float GetMarket(string resource)
        {
            return this.marketInventory[resource];
        }
        public void AddMarket(string resource, float amount)
        {
            this.marketInventory[resource] += amount;
            if (this.marketInventory[resource] < 0)
            {
                throw new Exception($"Market inventory went below 0 on {resource}");
            }
        }

        public IExchangeInventory CreateSimulatedClone()
        {
            return new TestInventoryModel(this);
        }

        public float GetCurrentSelfMoney()
        {
            return this.selfBank;
        }
    }
}
