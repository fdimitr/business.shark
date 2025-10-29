using System.ComponentModel.DataAnnotations;

namespace BusinessSharkService.DataAccess.Models.Player
{
    public class FinancialTransaction
    {
        [Key]
        public int FinancialTransactionId { get; set; }
        public int PlayerId { get; set; }
        public Player? Player { get; set; }
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
    }
}
