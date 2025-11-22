using BusinessSharkClient.Data.Entities;
using BusinessSharkClient.Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BusinessSharkClient.Logic
{
    public class ToolsProvider(ILocalRepository<ToolsEntity> repo)
    {
        public async Task<ToolsEntity> LoadAsync(int divisionId)
        {
            return await repo.Query().Where(x => x.DivisionId == divisionId).FirstOrDefaultAsync() ?? new ToolsEntity();
        }
    }
}
