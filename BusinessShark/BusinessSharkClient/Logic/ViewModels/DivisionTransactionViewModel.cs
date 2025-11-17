using BusinessSharkService;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BusinessSharkClient.Logic.ViewModels
{
    public class DivisionTransactionViewModel : ObservableObject
    {
        public int DivisionTransactionsId { get; set; }
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

        // Сost of Replenishment of Raw Materials
        public double ReplenishmentAmount { get; set; }

        // Production statistics
        public int QuantityProduced { get; set; }
        public double QualityProduced { get; set; }

        public double Balance { get; set; }

        public static DivisionTransactionViewModel GetLastTransaction(List<DivisionTransactionsGrpc> divisionTransactionsGrpcs)
        {
            var result = new DivisionTransactionViewModel();
            var dt = divisionTransactionsGrpcs.OrderByDescending(dt => dt.TransactionDate).Take(1).FirstOrDefault();
            if (dt != null)
            {
                result.Balance = dt.SalesProductsAmount -
                    (dt.PurchasedProductsAmount +
                     dt.TransportCostsAmount +
                     dt.EmployeeSalariesAmount +
                     dt.MaintenanceCostsAmount +
                     dt.IncomeTaxAmount +
                     dt.RentalCostsAmount +
                     dt.EmployeeTrainingAmount +
                     dt.CustomAmount +
                     dt.AdvertisingCostsAmount +
                     dt.RentalCostsAmount);

                result.QuantityProduced = dt.QuantityProduced;
                result.QualityProduced = dt.QualityProduced;
            }

            return result;
        }
    }
}
