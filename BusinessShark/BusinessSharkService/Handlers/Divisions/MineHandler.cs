using BusinessSharkService.DataAccess.Models.Divisions.RawMaterialProducers;
using BusinessSharkService.Handlers.Interfaces;

namespace BusinessSharkService.Handlers.Divisions
{
    public class MineHandler(IWorldContext worldHandler) : DivisionHandler<Mine>(worldHandler)
    {
        public override void StartCalculation(Mine Division)
        {
        }

        public override void CompleteCalculation(Mine Division)
        {
        }

        public override void CalculateCosts(Mine Division)
        {
            throw new NotImplementedException();
        }
    }
}
