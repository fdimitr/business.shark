using BusinessSharkService.DataAccess.Models.Divisions;
using BusinessSharkService.Handlers.Interfaces;

namespace BusinessSharkService.Handlers.Divisions
{
    public class DistributionCenterHandler(IWorldContext worldHandler) : DivisionHandler<DistributionCenter>(worldHandler)
    {
        public override void CalculateCosts(DistributionCenter Division, int quantityProduced, double qualityProduced)
        {
            throw new NotImplementedException();
        }

        public override void CompleteCalculation(DistributionCenter Division)
        {
        }

        public override void StartCalculation(DistributionCenter Division)
        {
        }
    }
}
