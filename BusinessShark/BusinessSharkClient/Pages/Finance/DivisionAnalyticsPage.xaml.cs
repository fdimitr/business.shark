using BusinessSharkClient.Logic.ViewModels;
using LiveChartsCore.Measure;

namespace BusinessSharkClient.Pages.Finance;

public partial class DivisionAnalyticsPage : ContentPage
{
    private readonly DivisionAnalyticsViewModel _vm;

    public DivisionAnalyticsPage()
    {
        InitializeComponent();

        _vm = new DivisionAnalyticsViewModel();
        BindingContext = _vm;

        // Configure chart interaction and appearance in code (safer than XAML enums)
        // Enable zoom & pan on both axes
        MainChart.ZoomMode = ZoomAndPanMode.Both;
        MainChart.LegendPosition = LegendPosition.Top;
        MainChart.TooltipPosition = TooltipPosition.Top; // default tooltip position
        MainChart.LegendTextSize = 12;
        // Clean, modern look for grid lines and axes
        if (_vm.XAxes != null) MainChart.XAxes = _vm.XAxes;
        if (_vm.YAxes != null) MainChart.YAxes = _vm.YAxes;

        // Populate the table grid (3 rows: Income, Expense, Profit; columns: Day 1..30)
        BuildTable();
    }

    private void BuildTable()
    {
        var days = _vm.Days; // 30 items expected
        DataGrid.RowDefinitions.Clear();
        DataGrid.ColumnDefinitions.Clear();
        DataGrid.Children.Clear();

        // First column: row headers
        DataGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

        // Add 30 columns for days
        foreach (var d in days)
        {
            DataGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
        }

        // Rows: header row + 3 data rows
        // Header row
        DataGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

        // Data rows: Income, Expense, Profit
        for (int r = 0; r < 3; r++)
            DataGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

        // Add header cell (top-left)
        var headerFrame = MakeCell("", true);
        DataGrid.Add(headerFrame, 0, 0);

        // Fill day headers
        for (int c = 0; c < days.Count; c++)
        {
            var dayLabel = MakeCell(days[c].Date.ToString("dd MMM"), true);
            DataGrid.Add(dayLabel, c + 1, 0);
        }

        // Row labels
        var rowTitles = new[] { "Income", "Expense", "Profit" };
        for (int r = 0; r < 3; r++)
        {
            var rowLabel = MakeCell(rowTitles[r], true);
            DataGrid.Add(rowLabel, 0, r + 1);
        }

        // Fill data cells
        for (int c = 0; c < days.Count; c++)
        {
            var d = days[c];
            var vals = new[] { d.Income, d.Expense, d.Profit };
            for (int r = 0; r < 3; r++)
            {
                var txt = vals[r].ToString("N2");
                var cell = MakeCell(txt, false);
                DataGrid.Add(cell, c + 1, r + 1);
            }
        }

        // Add thin separators using BoxViews inside cells: each cell has a bottom and right border handled by frame border color and margins.
        // To make the table look crisp we used Frame inside each cell (MakeCell).
    }

    private Frame MakeCell(string text, bool isHeader)
    {
        var lbl = new Label
        {
            Text = text,
            FontSize = isHeader ? 12 : 13,
            FontAttributes = isHeader ? FontAttributes.Bold : FontAttributes.None,
            LineBreakMode = LineBreakMode.NoWrap,
            VerticalOptions = LayoutOptions.Center,
            HorizontalOptions = LayoutOptions.Center,
        };

        var frame = new Frame
        {
            Padding = new Thickness(8, 6),
            Content = lbl,
            BorderColor = Color.FromArgb("#E6E6E6"),
            BackgroundColor = isHeader ? Color.FromArgb("#FAFAFA") : Color.FromArgb("#FFFFFF"),
            HasShadow = false,
            CornerRadius = 0,
            Margin = new Thickness(0)
        };

        return frame;
    }
}