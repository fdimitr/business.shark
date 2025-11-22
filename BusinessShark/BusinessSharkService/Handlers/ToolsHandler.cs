using BusinessSharkService.DataAccess;
using BusinessSharkService.DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace BusinessSharkService.Handlers
{
    public class ToolsHandler(DataContext dataContext)
    {
        public async Task<List<Tools>> LoadAsync(int companyId, DateTime updatedAt)
        {
            var query = from tool in dataContext.Tools
                        join division in dataContext.Divisions
                            on tool.DivisionId equals division.DivisionId
                        where division.CompanyId == companyId && tool.UpdatedAt > updatedAt
                        select tool;

            return await query.AsNoTracking().ToListAsync();
        }
    }
}
