using System;
using System.Collections.Generic;
using System.Linq;
using TradeModeling.Inventories;
using UniRx;
using UniRx.Triggers;

namespace Assets.Scripts.Resources.Inventory
{
    [Serializable]
    public struct StartingInventoryAmount
    {
        public ResourceType type;
        public float amount;
    }

    public class ResourceInventory : ObservableTriggerBase
    {
        public int inventoryCapacitySetForUI = 10;

        public StartingInventoryAmount[] startingInventoryAmounts;

        public TradingInventoryAdapter<ResourceType> backingInventory
        {
            get;
            private set;
        }

        private InventoryNotifier<ResourceType> inventoryNotifier;

        void Awake()
        {
            var initialInventory = new Dictionary<ResourceType, float>();
            var resourceTypes = Enum.GetValues(typeof(ResourceType)).Cast<ResourceType>();
            foreach (var resource in resourceTypes)
            {
                // create the key with default. Emit set events in Start(); once everyone has had a chance to subscribe to updates
                initialInventory[resource] = 0;
            }
            foreach (var startingAmount in startingInventoryAmounts)
            {
                initialInventory[startingAmount.type] = startingAmount.amount;
            }

            var itemSource = new SpaceFillingInventorySource<ResourceType>(
                initialInventory,
                ResourceConfiguration.spaceFillingItems,
                inventoryCapacitySetForUI);

            backingInventory = new TradingInventoryAdapter<ResourceType>(itemSource, ResourceType.Gold);
            inventoryNotifier = new InventoryNotifier<ResourceType>(backingInventory.itemSource, 200);

            //make sure that the observables get initialized by now, at the latest
            this.ResourceAmountsChangedAsObservable();
            this.ResourceCapacityChangedAsObservable();

            inventoryNotifier.resourceCapacityChanges += OnResourceCapacityChanged;
            inventoryNotifier.resourceAmountChanged += OnResourceAmountsChanged;
        }

        public void Start()
        {
            inventoryNotifier.NotifyAll();
        }

        public void Update()
        {
        }


        private ReplaySubject<ResourceChanged<ResourceType>> resourceAmountsChanged;
        private ReplaySubject<ResourceChanged<ResourceType>> resourceCapacityChanged;
        public IObservable<ResourceChanged<ResourceType>> ResourceAmountsChangedAsObservable()
        {
            return resourceAmountsChanged ?? (resourceAmountsChanged = new ReplaySubject<ResourceChanged<ResourceType>>());
        }
        private void OnResourceAmountsChanged(object sender, ResourceChanged<ResourceType> change)
        {
            resourceAmountsChanged.OnNext(change);
        }
        public IObservable<ResourceChanged<ResourceType>> ResourceCapacityChangedAsObservable()
        {
            return resourceCapacityChanged ?? (resourceCapacityChanged = new ReplaySubject<ResourceChanged<ResourceType>>());
        }
        private void OnResourceCapacityChanged(object sender, ResourceChanged<ResourceType> change)
        {
            resourceCapacityChanged.OnNext(change);
        }

        protected override void RaiseOnCompletedOnDestroy()
        {
            resourceAmountsChanged?.OnCompleted();
            resourceCapacityChanged?.OnCompleted();
        }
    }
}