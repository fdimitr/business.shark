using BusinessSharkService.DataAccess;
using BusinessSharkService.DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace BusinessSharkService.Handlers
{
    public class EmployeesHandler(DataContext dataContext)
    {
        public async Task<List<Employees>> LoadAsync(int companyId, DateTime updatedAt)
        {
            var query = from e in dataContext.Employees
                        join d in dataContext.Divisions on e.DivisionId equals d.DivisionId
                        where d.CompanyId == companyId && e.UpdatedAt > updatedAt
                        select e;

            return await query.AsNoTracking().ToListAsync();
        }
    }
}
