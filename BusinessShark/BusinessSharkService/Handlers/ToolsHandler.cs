using BusinessSharkService.DataAccess;
using BusinessSharkService.DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace BusinessSharkService.Handlers
{
    public class ToolsHandler(DataContext dataContext)
    {
        public async Task<List<Tools>> LoadAsync(DateTime updatedAt)
        {
            return await dataContext.Tools
                .Where(c => c.UpdatedAt > updatedAt)
                .ToListAsync();
        }
    }
}
