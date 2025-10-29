namespace BusinessSharkService.DataAccess.Models.Divisions.RawMaterialProducers
{
    public class SawmillTransactions
    {
        public int SawmillTransactionsId { get; set; }
        public int DivisionId { get; set; }

        public DateTime TransactionDate { get; set; }

        public double SalesProductsAmount { get; set; }
        public double PlantingAmount{ get; set; }
        public double EmployeeSalariesAmount { get; set; }
        public double MaintenanceCostsAmount { get; set; }
        public double RentalCostsAmount { get; set; }
        public double EmployeeTrainingAmount { get; set; }
    }
}
