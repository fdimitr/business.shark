using BusinessSharkClient.Logic;
using BusinessSharkClient.View.Divisions;

namespace BusinessSharkClient.View;

public partial class CompanyView : ContentPage
{
    private SawmillProvider _sawmillProvider;

    public CompanyView(SawmillProvider sawmillProvider)
	{
		InitializeComponent();
        _sawmillProvider = sawmillProvider;

        BindingContext = this;
    }

    public Command<string> SelectSectionCommand => new(OnSectionSelected);

    private void OnSectionSelected(string section)
    {
        // Очистим предыдущее содержимое
        DynamicContentArea.Content = null;

        // Подгружаем нужный контрол
        IView newContent = section switch
        {
            "Sawmills" => new SawmillListView(_sawmillProvider),
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