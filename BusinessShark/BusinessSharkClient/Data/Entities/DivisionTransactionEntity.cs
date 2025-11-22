using System.ComponentModel.DataAnnotations;
using BusinessSharkClient.Data.Entities.Interfaces;

namespace BusinessSharkClient.Data.Entities
{
    public class DivisionTransactionEntity : IEntity
    {
        [Key]
        public int Id { get; set; }
        public bool IsDirty { get; set; }
        public bool IsDeleted { get; set; }

        public int DivisionId { get; set; }
        public DateTime TransactionDate { get; set; }
        public double SalesProductsAmount { get; set; }
        public double PurchasedProductsAmount { get; set; }
        public double TransportCostsAmount { get; set; }
        public double EmployeeSalariesAmount { get; set; }
        public double MaintenanceCostsAmount { get; set; }
        public double IncomeTaxAmount { get; set; }
        public double RentalCostsAmount { get; set; }
        public double EmployeeTrainingAmount { get; set; }
        public double CustomAmount { get; set; }
        public double AdvertisingCostsAmount { get; set; }
        public double ReplenishmentAmount { get; set; }
        public int QuantityProduced { get; set; }
        public double QualityProduced { get; set; }

        public object[] GetKeyValues() => [Id];
    }
}
