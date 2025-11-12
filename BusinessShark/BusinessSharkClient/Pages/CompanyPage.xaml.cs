using BusinessSharkClient.Logic;
using BusinessSharkClient.View.Divisions;

namespace BusinessSharkClient.Pages;

public partial class CompanyPage : ContentPage
{
    private SawmillProvider _sawmillProvider;
    private GlobalDataProvider _globalDataProvider; 
    private DivisionTransactionProvider _transactionProvider;
    private DivisionWarehouseProvider _warehouseProvider;

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

    public CompanyPage(GlobalDataProvider globalDataProvider, SawmillProvider sawmillProvider, DivisionTransactionProvider divisionTransactionProvider, DivisionWarehouseProvider warehouseProvider)
	{
		InitializeComponent();
        _sawmillProvider = sawmillProvider;
        _globalDataProvider = globalDataProvider;
        _transactionProvider = divisionTransactionProvider;
        _warehouseProvider = warehouseProvider;
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
            "Sawmills" => new SawmillListView(_globalDataProvider, _sawmillProvider, _transactionProvider, _warehouseProvider),
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