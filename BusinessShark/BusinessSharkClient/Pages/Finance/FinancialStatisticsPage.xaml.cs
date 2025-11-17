using BusinessSharkClient.Logic;
using BusinessSharkClient.Logic.ViewModels;

namespace BusinessSharkClient.Pages.Finance;

public partial class FinancialStatisticsPage : ContentPage
{
    private DivisionTransactionProvider _transactionProvider;
    private int _divisionId;

    public FinancialStatisticsPage(DivisionTransactionProvider transactionProvider, int divisionId)
	{
        _divisionId = divisionId;
        _transactionProvider = transactionProvider;

        InitializeComponent();

        BindingContext = this;
        Loaded += OnLoadingView;
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

    private async void OnLoadingView(object? sender, EventArgs e)
    {
        try
        {
            var vm = await _transactionProvider.GetDivisionFinancialStatistics(_divisionId);
            FinancialStatisticsLayout.BindingContext = vm;

            // Populate the table grid (3 rows: Income, Expense, Profit; columns: Day 1..30)
            BuildStatisticsTable(vm);
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"An unexpected error occurred: {ex.Message}", "OK");
        }
    }

    private void BuildStatisticsTable(List<DivisionTransactionViewModel> transactions)
    {
        DataGridStatistics.RowDefinitions.Clear();
        DataGridStatistics.ColumnDefinitions.Clear();
        DataGridStatistics.Children.Clear();

        // First column: start row headers
        DataGridStatistics.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
        // Last column: end row headers
        DataGridStatistics.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

        // Add columns for days
        foreach (var d in transactions)
        {
            DataGridStatistics.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
        }

        // Data rows
        var headerRows = GetTableHeaders();
        for (int r = 0; r < headerRows.Length; r++)
            DataGridStatistics.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

        // Add start header cell (top-left)
        var headerFrame = MakeCell("", true);
        DataGridStatistics.Add(headerFrame, 0, 0);

        // Add end header cell (top-left)
        headerFrame = MakeCell("", true);
        DataGridStatistics.Add(headerFrame, transactions.Count + 2, 0);

        // Fill day headers
        for (int c = 0; c < transactions.Count; c++)
        {
            var dayLabel = MakeCell(transactions[c].TransactionDate.ToString("dd MMM"), true);
            DataGridStatistics.Add(dayLabel, c + 1, 0);
        }

        // Row Start labels
        for (int r = 0; r < headerRows.Length; r++)
        {
            var rowLabel = MakeCell(headerRows[r], true);
            DataGridStatistics.Add(rowLabel, 0, r + 1);
        }

        // Fill data cells
        for (int c = 0; c < transactions.Count; c++)
        {
            var d = transactions[c];
            var vals = GetTableData(d);
            for (int r = 0; r < vals.Length; r++)
            {
                var cell = MakeCell(vals[r], false);
                DataGridStatistics.Add(cell, c + 1, r + 1);
            }
        }

        // Row End labels
        for (int r = 0; r < headerRows.Length; r++)
        {
            var rowLabel = MakeCell(headerRows[r], true);
            DataGridStatistics.Add(rowLabel, transactions.Count + 2, r + 1);
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

    private async void OnDataScrollViewStatisticsLoaded(object sender, EventArgs e)
    {
        try
        {
            // If content is added later, also hook SizeChanged.
            DataGridStatistics.SizeChanged += async (_, __) => await ScrollGridToRightAsync();
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
        var targetX = Math.Max(0, DataGridStatistics.Width - DataScrollViewAnalytics.Width);
        if (targetX > 0)
            await DataScrollViewAnalytics.ScrollToAsync(targetX, 0, animated: false);
        else
        {
            // Option 2 fallback: last child
            var last = DataGridStatistics.Children.LastOrDefault();
            if (last != null)
                await DataScrollViewAnalytics.ScrollToAsync((Element)last, ScrollToPosition.End, animated: false);
        }
    }

    private string[] GetTableHeaders()
    {
        return
        [
            "Sales Amount",
            "Purchased Amount",
            "Transport Costs",
            "Employee Salaries",
            "Maintenance Costs",
            "Income Tax",
            "Rental Costs",
            "Employee Training",
            "Custom Amount",
            "Advertising Costs",
            "Replenishment Amount"
        ];
    }

    private string[] GetTableData(DivisionTransactionViewModel model)
    {
        return
        [
            model.SalesProductsAmount.ToString("N2"),
            model.PurchasedProductsAmount.ToString("N2"),
            model.TransportCostsAmount.ToString("N2"),
            model.EmployeeSalariesAmount.ToString("N2"),
            model.MaintenanceCostsAmount.ToString("N2"),
            model.IncomeTaxAmount.ToString("N2"),
            model.RentalCostsAmount.ToString("N2"),
            model.EmployeeTrainingAmount.ToString("N2"),
            model.CustomAmount.ToString("N2"),
            model.AdvertisingCostsAmount.ToString("N2"),
            model.ReplenishmentAmount.ToString("N2"),
        ];
    }
}