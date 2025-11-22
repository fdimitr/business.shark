using BusinessSharkClient.Logic;
using BusinessSharkClient.Logic.ViewModels;
using LiveChartsCore.Measure;

namespace BusinessSharkClient.Pages.Finance;

public partial class DivisionAnalyticsPage : ContentPage
{
    private readonly DivisionTransactionProvider _transactionProvider;
    private readonly int _divisionId;

    public DivisionAnalyticsPage(DivisionTransactionProvider transactionProvider, int divisionId)
    {
        _divisionId = divisionId;
        _transactionProvider = transactionProvider;

        InitializeComponent();

        BindingContext = this;
        Loaded += OnLoadingView;
    }

    private async void OnLoadingView(object? sender, EventArgs e)
    {
        try
        {
            var vm = await _transactionProvider.GetDivisionAnalytics(_divisionId);
            AnalyticLayout.BindingContext = vm;

            // Configure chart interaction and appearance in code (safer than XAML enums)
            // Enable zoom & pan on both axes
            MainChart.ZoomMode = ZoomAndPanMode.X;
            MainChart.LegendPosition = LegendPosition.Top;
            MainChart.TooltipPosition = TooltipPosition.Top; // default tooltip position
            MainChart.LegendTextSize = 12;
            // Clean, modern look for grid lines and axes
            MainChart.XAxes = vm.XAxes;
            MainChart.YAxes = vm.YAxes;

            // Populate the table grid (3 rows: Income, Expense, Profit; columns: Day 1..30)
            BuildAnalyticsTable(vm);
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"An unexpected error occurred: {ex.Message}", "OK");
        }
    }

    private async void OnBackButtonClicked(object sender, EventArgs e)
    {
        try
        {
            // Check if we can go back
            if (Navigation.NavigationStack.Count > 1)
            {
                await Navigation.PopAsync();
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"An unexpected error occurred: {ex.Message}", "OK");
        }
    }

    private void BuildAnalyticsTable(DivisionAnalyticsViewModel vm)
    {
        var days = vm.Days;
        DataGridAnalytics.RowDefinitions.Clear();
        DataGridAnalytics.ColumnDefinitions.Clear();
        DataGridAnalytics.Children.Clear();

        // First column: start row headers
        DataGridAnalytics.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
        // Last column: end row headers
        DataGridAnalytics.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

        // Add columns for days
        foreach (var d in days)
        {
            DataGridAnalytics.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
        }

        // Rows: header row + 3 data rows
        // Header row
        DataGridAnalytics.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

        // Data rows: Income, Expense, Profit, Quantity, Quality
        for (int r = 0; r < 5; r++)
            DataGridAnalytics.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

        // Add start header cell (top-left)
        var headerFrame = MakeCell("", true);
        DataGridAnalytics.Add(headerFrame, 0, 0);

        // Add end header cell (top-left)
        headerFrame = MakeCell("", true);
        DataGridAnalytics.Add(headerFrame, days.Count + 2, 0);

        // Fill day headers
        for (int c = 0; c < days.Count; c++)
        {
            var dayLabel = MakeCell(days[c].Date.ToString("dd MMM"), true);
            DataGridAnalytics.Add(dayLabel, c + 1, 0);
        }

        // Row Start labels
        var rowTitles = new[] { "Income", "Expense", "Profit", "Quantity", "Quality" };
        for (int r = 0; r < 5; r++)
        {
            var rowLabel = MakeCell(rowTitles[r], true);
            DataGridAnalytics.Add(rowLabel, 0, r + 1);
        }

        // Fill data cells
        for (int c = 0; c < days.Count; c++)
        {
            var d = days[c];
            var vals = new[] { d.Income, d.Expense, d.Profit, d.Quantity, d.Quality };
            for (int r = 0; r < 5; r++)
            {
                var txt = vals[r].ToString("N2");
                var cell = MakeCell(txt, false);
                DataGridAnalytics.Add(cell, c + 1, r + 1);
            }
        }

        // Row End labels
        for (int r = 0; r < 5; r++)
        {
            var rowLabel = MakeCell(rowTitles[r], true);
            DataGridAnalytics.Add(rowLabel, days.Count + 2, r + 1);
        }
    }

    private Border MakeCell(string text, bool isHeader)
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

        var border = new Border
        {
            Padding = new Thickness(8, 6),
            Content = lbl,
            Stroke = Color.FromArgb("#E6E6E6"),
            BackgroundColor = isHeader ? Color.FromArgb("#FAFAFA") : Color.FromArgb("#FFFFFF"),
            Margin = new Thickness(0)
        };

        return border;
    }

    private async void OnDataScrollViewAnalyticsLoaded(object sender, EventArgs e)
    {
        try
        {
            // If content is added later, also hook SizeChanged.
            DataGridAnalytics.SizeChanged += async (_, __) => await ScrollGridToRightAsync();
            await ScrollGridToRightAsync();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"An unexpected error occurred: {ex.Message}", "OK");
        }
    }

    private async Task ScrollGridToRightAsync()
    {
        // Ensure layout pass complete
        await Task.Yield();

        // Option 1: width-based scroll
        var targetX = Math.Max(0, DataGridAnalytics.Width - DataScrollViewAnalytics.Width);
        if (targetX > 0)
            await DataScrollViewAnalytics.ScrollToAsync(targetX, 0, animated: false);
        else
        {
            // Option 2 fallback: last child
            var last = DataGridAnalytics.Children.LastOrDefault();
            if (last != null)
                await DataScrollViewAnalytics.ScrollToAsync((Element)last, ScrollToPosition.End, animated: false);
        }
    }
}