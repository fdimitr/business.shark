using BusinessSharkService.DataAccess.Models.Divisions;
using BusinessSharkService.Handlers.Interfaces;

namespace BusinessSharkService.Handlers.Divisions
{
    public class DistributionCenterHandler(IWorldContext worldHandler) : BaseDivisionHandler<DistributionCenter>(worldHandler)
    {
        public override void CalculateCosts(DistributionCenter baseDivision)
        {
            throw new NotImplementedException();
        }

        public override void CompleteCalculation(DistributionCenter baseDivision)
        {
        }

        public override void StartCalculation(DistributionCenter baseDivision)
        {
        }
    }
}
