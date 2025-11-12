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
        [Description("Leather Sofa")]
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
}