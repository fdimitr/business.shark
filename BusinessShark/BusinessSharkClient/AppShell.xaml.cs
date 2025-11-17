using BusinessSharkClient.Logic.ViewModels;
using BusinessSharkClient.Pages;
using BusinessSharkClient.View;

namespace BusinessSharkClient
{
    public partial class AppShell : Shell
    {
        public ShellHeaderViewModel HeaderViewModel { get; } = new();

        public AppShell()
        {
            InitializeComponent();

            // Registering pages for navigation
            Routing.RegisterRoute(nameof(MainPage), typeof(MainPage));
            Routing.RegisterRoute(nameof(OfficePage), typeof(OfficePage));
            Routing.RegisterRoute(nameof(LoginPage), typeof(LoginPage));
            Routing.RegisterRoute(nameof(CompanyPage), typeof(CompanyPage));
            Routing.RegisterRoute(nameof(WorldPage), typeof(WorldPage));
            Routing.RegisterRoute(nameof(ConstructionPage), typeof(ConstructionPage));
            Routing.RegisterRoute(nameof(ProfilePage), typeof(ProfilePage));
            Routing.RegisterRoute(nameof(AnaliticsPage), typeof(AnaliticsPage));
            Routing.RegisterRoute(nameof(DocumentationPage), typeof(DocumentationPage));
            Routing.RegisterRoute(nameof(ProductListPage), typeof(ProductListPage));

            // Setting the BindingContext for data binding
            BindingContext = HeaderViewModel;

            // Subscribing to the Loaded event
            this.Loaded += OnShellLoaded;
        }

        private void OnShellLoaded(object? sender, EventArgs e)
        {
            // Ensure Shell.Current is not null
            if (Shell.Current is null)
                return;

            // Subscribing to the Navigated event to update header information
            Shell.Current.Navigated += async (_, _) =>
            {
                HeaderViewModel.Title = Shell.Current?.CurrentPage?.Title ?? "No name";
                HeaderViewModel.CompanyName = await SecureStorage.Default.GetAsync("company_name") ?? "Noname company";
            };

            // Initializing header information
            HeaderViewModel.Title = Shell.Current?.CurrentPage?.Title ?? "Main";
        }
    }
}
