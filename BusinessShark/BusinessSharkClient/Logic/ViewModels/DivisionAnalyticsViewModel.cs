using BusinessSharkClient.Logic.Models;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace BusinessSharkClient.Logic.ViewModels
{
    public class DivisionAnalyticsViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public ObservableCollection<Models.DivisionAnaliticsModel> Days { get; }

        // LiveCharts series array (four line series)
        public ISeries[] Series { get; }

        // X and Y axes configuration
        public Axis[] XAxes { get; }
        public Axis[] YAxes { get; }

        public DivisionAnalyticsViewModel()
        {
            Days = new ObservableCollection<DivisionAnaliticsModel>();
            var rnd = new Random(42);
            var start = DateTime.Today.AddDays(-29);

            for (int i = 0; i < 30; i++)
            {
                var date = start.AddDays(i);
                var income = 800 + 400 * Math.Sin(i * 0.25) + rnd.NextDouble() * 150;
                var expense = income * (0.55 + rnd.NextDouble() * 0.15);
                var tax = income * 0.18;
                var profit = income - expense - tax;
                Days.Add(new DivisionAnaliticsModel
                {
                    Date = date,
                    Income = Math.Round(income, 2),
                    Expense = Math.Round(expense, 2),
                    Tax = Math.Round(tax, 2),
                    Profit = Math.Round(profit, 2)
                });
            }

            Series = new ISeries[]
            {
                new LineSeries<double>
                {
                    Name = "Доход",
                    Values = Days.Select(d => d.Income).ToArray(),
                    GeometrySize = 6,
                    Stroke = new SolidColorPaint(SKColors.DeepSkyBlue) { StrokeThickness = 2 },
                    Fill = null,
                    LineSmoothness = 0
                },
                new LineSeries<double>
                {
                    Name = "Расход",
                    Values = Days.Select(d => d.Expense).ToArray(),
                    GeometrySize = 6,
                    Stroke = new SolidColorPaint(SKColors.IndianRed) { StrokeThickness = 2 },
                    Fill = null,
                    LineSmoothness = 0
                },
                new LineSeries<double>
                {
                    Name = "Налог",
                    Values = Days.Select(d => d.Tax).ToArray(),
                    GeometrySize = 6,
                    Stroke = new SolidColorPaint(SKColors.Goldenrod) { StrokeThickness = 2 },
                    Fill = null,
                    LineSmoothness = 0
                },
                new LineSeries<double>
                {
                    Name = "Прибыль",
                    Values = Days.Select(d => d.Profit).ToArray(),
                    GeometrySize = 6,
                    Stroke = new SolidColorPaint(SKColors.ForestGreen) { StrokeThickness = 2 },
                    Fill = null,
                    LineSmoothness = 0
                }
            };

            XAxes = new Axis[]
            {
                new Axis
                {
                    Labels = Days.Select(d => d.Date.ToString("dd MMM")).ToArray(),
                    ShowSeparatorLines = true,
                    SeparatorsPaint = new SolidColorPaint(SKColors.LightGray) { StrokeThickness = 1 },
                    UnitWidth = 1
                }
            };

            YAxes = new Axis[]
            {
                new Axis
                {
                    Labeler = value => value.ToString("N0"),
                    Name = "₽",
                    SeparatorsPaint = new SolidColorPaint(SKColors.LightGray) { StrokeThickness = 1 }
                }
            };
        }
    }
}
