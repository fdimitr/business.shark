using BusinessSharkService.DataAccess.Models.Divisions;
using BusinessSharkService.Handlers.Interfaces;

namespace BusinessSharkService.Handlers
{
    public class StorageHandler(IWorldHandler worldHandler) : BaseDivisionHandler<Storage>(worldHandler)
    {
        public override void CompleteCalculation(Storage baseDivision)
        {
            throw new NotImplementedException();
        }

        public override void StartCalculation(Storage baseDivision)
        {
            throw new NotImplementedException();
        }
    }
}
