using BusinessSharkClient.Logic;
using BusinessSharkClient.View.Divisions;

namespace BusinessSharkClient.Pages;

public partial class CompanyPage : ContentPage
{
    private readonly SawmillProvider _sawmillProvider;
    private readonly GlobalDataProvider _globalDataProvider; 
    private readonly DivisionTransactionProvider _transactionProvider;
    private readonly DivisionWarehouseProvider _warehouseProvider;
    private readonly DivisionSizeProvider _divisionSizeProvider;
    private readonly ToolsProvider _toolsProvider;

    private string _selectedSection = "Stores";
    public string SelectedSection
    {
        get => _selectedSection;
        set
        {
            if (_selectedSection != value)
            {
                _selectedSection = value;
                OnPropertyChanged();
            }
        }
    }

    public CompanyPage(GlobalDataProvider globalDataProvider, SawmillProvider sawmillProvider, DivisionTransactionProvider divisionTransactionProvider, 
        DivisionWarehouseProvider warehouseProvider, DivisionSizeProvider divisionSizeProvider, ToolsProvider toolsProvider)
    {
		InitializeComponent();
        _sawmillProvider = sawmillProvider;
        _globalDataProvider = globalDataProvider;
        _transactionProvider = divisionTransactionProvider;
        _warehouseProvider = warehouseProvider;
        _divisionSizeProvider = divisionSizeProvider;
        _toolsProvider = toolsProvider;
        SelectedSection = "Stores";

        DynamicContentArea.Content = new StoresListView();
        BindingContext = this;
    }

    public Command<string> SelectSectionCommand => new(OnSectionSelected);

    private void OnSectionSelected(string section)
    {
        SelectedSection = section;

        // Очистим предыдущее содержимое
        DynamicContentArea.Content = null;

        // Подгружаем нужный контрол
        IView newContent = section switch
        {
            "Sawmills" => new SawmillListView(_globalDataProvider, _sawmillProvider, _transactionProvider, _warehouseProvider, _divisionSizeProvider, _toolsProvider),
            "Warehouses" => new WarehouseListView(),
            "Factories" => new FactoryListView(),
            "Stores" => new StoresListView(),
            "Mines" => new MinesListView(),
            _ => new Label { Text = "View not found", TextColor = Colors.Gray, HorizontalOptions = LayoutOptions.Center }
        };

        // Устанавливаем контент
        DynamicContentArea.Content = (Microsoft.Maui.Controls.View?)newContent;
    }
}