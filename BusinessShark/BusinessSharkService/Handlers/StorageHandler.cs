using BusinessSharkService.DataAccess.Models.Divisions;
using BusinessSharkService.Handlers.Interfaces;

namespace BusinessSharkService.Handlers
{
    public class StorageHandler(IWorldContext worldHandler) : BaseDivisionHandler<Storage>(worldHandler)
    {
        public override void CompleteCalculation(Storage baseDivision)
        {
        }

        public override void StartCalculation(Storage baseDivision)
        {
        }
    }
}
