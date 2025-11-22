using System.Collections.ObjectModel;
using BusinessSharkClient.Data.Entities;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace BusinessSharkClient.Logic.ViewModels
{
    public class DivisionAnalyticsViewModel
    {
        public ObservableCollection<Models.DivisionAnalyticsModel> Days { get; }

        // LiveCharts series array (four line series)
        public ISeries[] Series { get; }

        // X and Y axes configuration
        public Axis[] XAxes { get; }
        public Axis[] YAxes { get; }

        public DivisionAnalyticsViewModel(List<DivisionTransactionEntity> divisionTransactions)
        {
            Days = [];
            foreach (var dt in divisionTransactions)
            {
                Days.Add(new Models.DivisionAnalyticsModel
                {
                    Date = dt.TransactionDate,
                    Income = dt.SalesProductsAmount,
                    Expense = dt.PurchasedProductsAmount +
                              dt.TransportCostsAmount +
                              dt.EmployeeSalariesAmount +
                              dt.MaintenanceCostsAmount +
                              dt.IncomeTaxAmount +
                              dt.RentalCostsAmount +
                              dt.EmployeeTrainingAmount +
                              dt.CustomAmount +
                              dt.AdvertisingCostsAmount +
                              dt.ReplenishmentAmount,
                    Profit = dt.SalesProductsAmount -
                             (dt.PurchasedProductsAmount +
                              dt.TransportCostsAmount +
                              dt.EmployeeSalariesAmount +
                              dt.MaintenanceCostsAmount +
                              dt.IncomeTaxAmount +
                              dt.RentalCostsAmount +
                              dt.EmployeeTrainingAmount +
                              dt.CustomAmount +
                              dt.AdvertisingCostsAmount +
                              dt.ReplenishmentAmount),
                    Quality = dt.QualityProduced,
                    Quantity = dt.QuantityProduced
                });
            }

            Series =
            [
                new LineSeries<double>
                {
                    Name = "Income",
                    Values = Days.Select(d => d.Income).ToArray(),
                    GeometrySize = 6,
                    Stroke = new SolidColorPaint(SKColors.Green) { StrokeThickness = 3 },
                    GeometryStroke = new SolidColorPaint(SKColors.Green) { StrokeThickness = 3 },
                    Fill = null,
                    LineSmoothness = 0
                },
                new LineSeries<double>
                {
                    Name = "Expense",
                    Values = Days.Select(d => d.Expense).ToArray(),
                    GeometrySize = 6,
                    Stroke = new SolidColorPaint(SKColors.IndianRed) { StrokeThickness = 3 },
                    GeometryStroke = new SolidColorPaint(SKColors.IndianRed) { StrokeThickness = 3 },
                    Fill = null,
                    LineSmoothness = 0
                },
                new LineSeries<double>
                {
                    Name = "Profit",
                    Values = Days.Select(d => d.Profit).ToArray(),
                    GeometrySize = 6,
                    Stroke = new SolidColorPaint(SKColors.Blue) { StrokeThickness = 3 },
                    GeometryStroke = new SolidColorPaint(SKColors.Blue) { StrokeThickness = 3 },
                    Fill = null,
                    LineSmoothness = 0
                }
            ];

            XAxes =
            [
                new Axis
                {
                    Labels = Days.Select(d => d.Date.ToString("dd MMM")).ToArray(),
                    ShowSeparatorLines = true,
                    SeparatorsPaint = new SolidColorPaint(SKColors.LightGray) { StrokeThickness = 1 },
                    UnitWidth = 1,
                    TextSize = 12
                }
            ];

            YAxes =
            [
                new Axis
                {
                    Labeler = value => value.ToString("N0"),
                    ShowSeparatorLines = true,
                    SeparatorsPaint = new SolidColorPaint(SKColors.LightGray) { StrokeThickness = 1 },
                    TextSize = 12
                }
            ];
        }
    }
}
