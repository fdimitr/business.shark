using BusinessSharkClient.Logic;
using BusinessSharkClient.View.Divisions;
using System.ComponentModel;

namespace BusinessSharkClient.View;

public partial class CompanyPage : ContentPage
{
    private SawmillProvider _sawmillProvider;
    private GlobalDataProvider _globalDataProvider; 

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

    public CompanyPage(GlobalDataProvider globalDataProvider, SawmillProvider sawmillProvider)
	{
		InitializeComponent();
        _sawmillProvider = sawmillProvider;
        _globalDataProvider = globalDataProvider;
        SelectedSection = "Stores";

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
            "Sawmills" => new SawmillListView(_globalDataProvider, _sawmillProvider),
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