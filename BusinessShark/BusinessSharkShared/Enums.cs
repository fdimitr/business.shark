using System.ComponentModel;

namespace BusinessSharkShared
{
    public enum ProductType
    {
        Wood = 1,
        Leather = 2,
        Bed = 3,
        Chair = 4,
        Table = 5,
        Clay = 6,
        Sofa = 7,
        Coal = 8,
        IronOre = 9,
        CooperOre = 10,
        Clayware = 11,
        Qartz = 12,
    }

    public enum WarehouseType
    {
        Input = 0,
        Output = 1
    }

    public enum DivisionType
    {
        Sawmill = 0,
        Factory = 1,
        DistributionCenter = 2,
        Store = 3
    }
}