using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BusinessSharkService.DataAccess.Models.Items;
using BusinessSharkService.DataAccess.Models.Location;
using BusinessSharkService.DataAccess.Models.Player;
using Microsoft.EntityFrameworkCore;

namespace BusinessSharkService.DataAccess.Models.Divisions
{
    public abstract class BaseDivision
    {
        [Key] public int DivisionId { get; set; }

        public int CompanyId { get; set; }
        public Company? Company { get; set; }

        [Required] [StringLength(30)] public required string Name { get; set; }

        public int CityId { get; set; }
        public City? City { get; set; }

        [StringLength(300)] public string? Description { get; set; }

        [Comment("The volume capacity of the division warehouse")]
        public int VolumeCapacity { get; set; }

        [Comment("The cost of renting this division per month. Based on the city metric and city location")]
        public float RentalCost { get; set; }

        [Comment("Warehouses for the division")]
        public List<Warehouse>? Warehouses
        {
            get => _warehouses;
            set
            {
                _warehouses = value ?? throw new InvalidOperationException("Warehouses list cannot be null for division " + this.DivisionId);

                InputWarehouse = _warehouses.FirstOrDefault(w => w.Type == (int)WarehouseType.Input);
                if (InputWarehouse == null) throw new InvalidOperationException("Input warehouse not found for division " + this.DivisionId);

                OutputWarehouse = _warehouses.FirstOrDefault(w => w.Type == (int)WarehouseType.Output);
                if (OutputWarehouse == null) throw new InvalidOperationException("Output warehouse not found for division " + this.DivisionId);
            }
        }

        public List<DeliveryRoute> DeliveryRoutes { get; set; } = new();

        public Tools? Tools { get; set; }

        public Employees? Employees { get; set; }

        [NotMapped]
        public Warehouse? InputWarehouse;

        [NotMapped]
        public Warehouse? OutputWarehouse;


        [NotMapped]
        public List<WarehouseProduct>? WarehouseProductInput => InputWarehouse?.Products;


        [NotMapped]
        public List<WarehouseProduct>? WarehouseProductOutput => OutputWarehouse?.Products;


        private List<Warehouse>? _warehouses;

    }
}
