using BusinessSharkService.DataAccess.Models.Divisions.RawMaterialProducers;
using BusinessSharkService.Handlers.Interfaces;

namespace BusinessSharkService.Handlers
{
    public class SawmillHandler(IWorldHandler worldHandler) : BaseDivisionHandler<Sawmill>(worldHandler)
    {
        public override void StartCalculation(Sawmill baseDivision)
        {
        }

        public override void CompleteCalculation(Sawmill baseDivision)
        {
        }
    }
}
