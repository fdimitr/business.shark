using BusinessSharkClient.View;

namespace BusinessSharkClient
{
    public partial class AppShell : Shell
    {
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
        }
    }
}
