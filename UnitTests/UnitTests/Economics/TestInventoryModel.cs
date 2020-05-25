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
        private Dictionary<string, float> inventory;
        public float bank;
        public TestInventoryModel(
            (string, float)[] inventory,
            float bank)
        {
            this.inventory = inventory.ToDictionary(x => x.Item1, x => x.Item2);
            this.bank = bank;
        }

        private TestInventoryModel(TestInventoryModel other)
        {
            this.inventory = new Dictionary<string, float>(other.inventory);
            this.bank = other.bank;
        }

        public float Get(string resource)
        {
            return this.inventory[resource];
        }
        public void Add(string resource, float amount)
        {
            this.inventory[resource] += amount;
            if (this.inventory[resource] < 0)
            {
                throw new Exception($"Self inventory went below 0 on {resource}");
            }
        }

        public IExchangeInventory CreateSimulatedClone()
        {
            return new TestInventoryModel(this);
        }

        public float GetCurrentFunds()
        {
            return this.bank;
        }
    }
}
