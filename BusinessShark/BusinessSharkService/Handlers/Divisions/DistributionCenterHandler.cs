using BusinessSharkService.DataAccess.Models.Divisions;
using BusinessSharkService.Handlers.Interfaces;

namespace BusinessSharkService.Handlers.Divisions
{
    public class DistributionCenterHandler(IWorldContext worldHandler) : DivisionHandler<DistributionCenter>(worldHandler)
    {
        public override void CalculateCosts(DistributionCenter division, int quantityProduced, double qualityProduced)
        {
            throw new NotImplementedException();
        }

        public override void CompleteCalculation(DistributionCenter division)
        {
        }

        public override void StartCalculation(DistributionCenter division)
        {
        }
    }
}
