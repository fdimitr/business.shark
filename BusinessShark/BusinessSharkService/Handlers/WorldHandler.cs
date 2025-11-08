using System.Collections.Frozen;
using System.Diagnostics;
using BusinessSharkService.DataAccess;
using BusinessSharkService.DataAccess.Models.Finance;
using BusinessSharkService.Handlers.Context;
using BusinessSharkService.Handlers.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BusinessSharkService.Handlers
{
    public class WorldHandler
    {
        private readonly ILogger<WorldHandler> _logger;
        private readonly CountryHandler _countryHandler;
        private readonly DataContext _dbContext;
        private readonly IWorldContext _worldContext;

        public WorldHandler(ILogger<WorldHandler> logger, DataContext dbContext, IWorldContext worldContext, CountryHandler countryHandler)
        {
            _logger = logger;
            _worldContext = worldContext;
            _dbContext = dbContext;
            _countryHandler = countryHandler;

            _worldContext.CurrentDate = dbContext.Worlds.First().CurrentDate;

            // Load All ItemDefinitions
            _worldContext.ProductDefinitions = dbContext.ProductDefinitions
                .AsNoTracking()
                .ToDictionary(i => i.ProductDefinitionId, i => i)
                .ToFrozenDictionary();

            // Load All Countries with related Cities
            _worldContext.Countries = dbContext.Countries
                .AsNoTracking()
                .Include(c => c.Cities)
                .ToList();
        }

        public async Task LoadCalculationData()
        {
            foreach (var country in _worldContext.Countries)
            {
                foreach (var city in country.Cities)
                {
                    city.Divisions = await _dbContext.Divisions
                        .Where(s => s.CityId == city.CityId)
                        .Include(s => s.Warehouses!)
                          .ThenInclude(w => w.Products)
                        .Include(s => s.DeliveryRoutes)
                        .Include(f => f.Employees)
                        .Include(f => f.Tools)
                        .ToListAsync();
                }
            }
        }

        public async Task SaveCalculationData()
        {
            await _dbContext.SaveChangesAsync();
        }

        public async Task Calculate(CancellationToken stoppingToken)
        {
            try
            {
                _worldContext.CurrentDate = _worldContext.CurrentDate.AddDays(1);

                var stopwatch = new Stopwatch();
                stopwatch.Start();

                await LoadCalculationData();
                _worldContext.FillDivisions();

                StartCalculation(stoppingToken);
                CompleteCalculation(stoppingToken);

                stopwatch.Stop();
                SummarizingСalculation(stoppingToken, stopwatch.ElapsedMilliseconds);

                if (!stoppingToken.IsCancellationRequested)
                {
                    await SaveCalculationData();
                }

                _dbContext.Worlds.First().CurrentDate = _worldContext.CurrentDate;
                _logger.LogInformation("Calculation completed in {ElapsedMilliseconds} ms", stopwatch.ElapsedMilliseconds);
            }
            finally
            {
                ClearCalculationEntities();
            }
        }

        /// <summary>
        /// Clears all division entities from each city within every country in the world context.
        /// </summary>
        /// <remarks>This method iterates through all countries and their respective cities in the world
        /// context, removing all division entities from each city. Use this method to reset the division data across
        /// all cities.</remarks>
        private void ClearCalculationEntities()
        {
            foreach (var country in _worldContext.Countries)
            {
                foreach (var city in country.Cities)
                {
                    city.Divisions.Clear();
                }
            }
        }

        /// <summary>
        /// Aggregates financial data for each company and creates a summary transaction record.
        /// </summary>
        /// <remarks>This method iterates over all companies in the database context, aggregates financial
        /// data from each division within a company, and creates a new financial transaction record for each company.
        /// The operation can be cancelled by signaling the provided <paramref name="stoppingToken"/>.</remarks>
        /// <param name="stoppingToken">A token to monitor for cancellation requests, which can be used to stop the operation prematurely.</param>
        /// <param name="calculationMillisecond"></param>
        private void SummarizingСalculation(CancellationToken stoppingToken, long calculationMillisecond)
        {
            foreach(var company in _dbContext.Companies)
            {
                var financialTransactions = new FinancialTransaction
                {
                    CompanyId = company.CompanyId,
                    TransactionDate = _worldContext.CurrentDate,
                };

                if (stoppingToken.IsCancellationRequested) break;
                foreach(var division in company.Divisions)
                {
                    // Here you can add summarizing logic for each division
                    var divTran = division.CurrentTransactions;

                    financialTransactions.SalesProductsAmount += divTran.SalesProductsAmount;
                    financialTransactions.PurchasedProductsAmount += divTran.PurchasedProductsAmount;
                    financialTransactions.TransportCostsAmount += divTran.TransportCostsAmount;
                    financialTransactions.EmployeeSalariesAmount += divTran.EmployeeSalariesAmount;
                    financialTransactions.MaintenanceCostsAmount += divTran.MaintenanceCostsAmount;
                    financialTransactions.IncomeTaxAmount += divTran.IncomeTaxAmount;
                    financialTransactions.RentalCostsAmount += divTran.RentalCostsAmount;
                    financialTransactions.EmployeeTrainingAmount += divTran.EmployeeTrainingAmount;
                    financialTransactions.CustomAmount += divTran.CustomAmount;
                    financialTransactions.AdvertisingCostsAmount += divTran.AdvertisingCostsAmount;
                    financialTransactions.ReplenishmentAmount += divTran.ReplenishmentAmount;
                }

                financialTransactions.CalculationMilliseconds = calculationMillisecond;
                _dbContext.FinancialTransactions.Add(financialTransactions);

                var total = financialTransactions.SalesProductsAmount
                            - financialTransactions.PurchasedProductsAmount
                            - financialTransactions.TransportCostsAmount
                            - financialTransactions.EmployeeSalariesAmount
                            - financialTransactions.MaintenanceCostsAmount
                            - financialTransactions.IncomeTaxAmount
                            - financialTransactions.RentalCostsAmount
                            - financialTransactions.EmployeeTrainingAmount
                            - financialTransactions.CustomAmount
                            - financialTransactions.AdvertisingCostsAmount
                            - financialTransactions.ReplenishmentAmount;

                company.Balance += total;
            }
        }

        public void StartCalculation(CancellationToken stoppingToken)
        {
            foreach (var country in _worldContext.Countries)
            {
                if (stoppingToken.IsCancellationRequested) break;
                _countryHandler.StartCalculation(stoppingToken, country);
            }
        }

        public void CompleteCalculation(CancellationToken stoppingToken)
        {
            foreach (var country in _worldContext.Countries)
            {
                if (stoppingToken.IsCancellationRequested) break;
                _countryHandler.CompleteCalculation(stoppingToken, country);
            }
        }
    }
}
