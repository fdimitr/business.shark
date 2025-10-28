using BusinessSharkService.DataAccess.Models.Divisions.RawMaterialProducers;
using BusinessSharkService.Handlers.Interfaces;

namespace BusinessSharkService.Handlers
{
    public class MineHandler(IWorldHandler worldHandler) : BaseDivisionHandler<Mine>(worldHandler)
    {
        public override void StartCalculation(Mine baseDivision)
        {
        }

        public override void CompleteCalculation(Mine baseDivision)
        {
        }
    }
}
