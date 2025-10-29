using BusinessSharkService.DataAccess.Models.Divisions.RawMaterialProducers;
using BusinessSharkService.Handlers.Interfaces;

namespace BusinessSharkService.Handlers.Divisions
{
    public class MineHandler(IWorldContext worldHandler) : BaseDivisionHandler<Mine>(worldHandler)
    {
        public override void StartCalculation(Mine baseDivision)
        {
        }

        public override void CompleteCalculation(Mine baseDivision)
        {
        }

        public override void CalculateCosts(Mine baseDivision)
        {
            throw new NotImplementedException();
        }
    }
}
