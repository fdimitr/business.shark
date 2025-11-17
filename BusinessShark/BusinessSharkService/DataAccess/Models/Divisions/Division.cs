using BusinessSharkService.DataAccess.Models.Finance;
using BusinessSharkService.DataAccess.Models.Items;
using BusinessSharkService.DataAccess.Models.Location;
using BusinessSharkService.DataAccess.Models.Player;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusinessSharkService.DataAccess.Models.Divisions
{
    public abstract class Division
    {
        [Key] 
        public int DivisionId { get; set; }

        public int CompanyId { get; set; }
        public Company? Company { get; set; }

        public int CityId { get; set; }
        public City? City { get; set; }

        [Required] 
        [StringLength(30)]
        public required string Name { get; set; }

        [StringLength(300)] 
        public string? Description { get; set; }

        public int DivisionSizeId { get; set; }
        public DivisionSize? DivisionSize { get; set; }

        [Comment("The cost of renting this division per month. Based on the city metric and city location")]
        public double RentalCost { get; set; }

        [Comment("A coefficient that directly affects the final quality produced (if any)")]
        public double QuantityBonus { get; set; }

        [Comment("A coefficient that directly affects the final quantity produced (if any)")]
        public double QualityBonus { get; set; }

        [Comment("Warehouses for the division")]
        public List<Warehouse>? Warehouses { get; set; }

        public List<DivisionTransaction>? DivisionTransactions { get; set; }

        public List<DeliveryRoute> DeliveryRoutes { get; set; } = new();

        public Tools? Tools { get; set; }

        public Employees? Employees { get; set; }

        [NotMapped]
        internal DivisionTransaction CurrentTransactions { get; set; } = new();

        [NotMapped]
        public Warehouse InputWarehouse
        {
            get
            {
                var inputWarehouse = Warehouses!.FirstOrDefault(w => w.Type == (int)WarehouseType.Input);
                if (inputWarehouse == null) throw new InvalidOperationException("Input warehouse not found for division " + this.DivisionId);
                return inputWarehouse;
            }
        }

        [NotMapped]
        public Warehouse OutputWarehouse { get
            {
                var outputWarehouse = Warehouses!.FirstOrDefault(w => w.Type == (int)WarehouseType.Output);
                if (outputWarehouse == null) throw new InvalidOperationException("Output warehouse not found for division " + this.DivisionId);
                return outputWarehouse;
            } }


        [NotMapped]
        public List<WarehouseProduct>? WarehouseProductInput => InputWarehouse?.Products;


        [NotMapped]
        public List<WarehouseProduct>? WarehouseProductOutput => OutputWarehouse?.Products;
    }
}
