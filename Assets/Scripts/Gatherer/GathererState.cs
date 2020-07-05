using System;

namespace Assets.Scripts.Gatherer
{
    [Flags]
    public enum GathererState
    {
        Gathering = 1 << 0,
        GoingHome = 1 << 1,
        Selling = 1 << 2,
        GoingHomeToEat = 1 << 3,
        Consuming = 1 << 4,
        WaitingForMarket = 1 << 5,
        Sleeping = 1 << 6,
        Dying = 1 << 7,
        All = Gathering | GoingHome | Selling | GoingHomeToEat | Consuming | WaitingForMarket | Sleeping | Dying,
        None = 0
    }
}