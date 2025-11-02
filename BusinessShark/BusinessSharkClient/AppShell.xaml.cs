using BusinessSharkClient.Logic.Models;
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
            Routing.RegisterRoute(nameof(OfficeView), typeof(OfficeView));
            Routing.RegisterRoute(nameof(LoginView), typeof(LoginView));
            Routing.RegisterRoute(nameof(CompanyView), typeof(CompanyView));
            Routing.RegisterRoute(nameof(WorldView), typeof(WorldView));
            Routing.RegisterRoute(nameof(ConstructionView), typeof(ConstructionView));
            Routing.RegisterRoute(nameof(ProfileView), typeof(ProfileView));
            Routing.RegisterRoute(nameof(AnaliticsView), typeof(AnaliticsView));
            Routing.RegisterRoute(nameof(DocumentationView), typeof(DocumentationView));
            Routing.RegisterRoute(nameof(ProductListView), typeof(ProductListView));

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
