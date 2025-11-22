using BusinessSharkClient.Data.Entities;
using BusinessSharkClient.Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BusinessSharkClient.Logic
{
    public class EmployeesProvider(ILocalRepository<EmployeesEntity> repo)
    {
        public async Task<EmployeesEntity> LoadAsync(int divisionId)
        {
            return await repo.Query().Where(x => x.DivisionId == divisionId).FirstOrDefaultAsync() ?? new EmployeesEntity();
        }
    }
}
