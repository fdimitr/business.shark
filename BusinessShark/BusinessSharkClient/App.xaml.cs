using BusinessSharkClient.Logic;
using BusinessSharkClient.View;
using BusinessSharkService;

namespace BusinessSharkClient
{
    public partial class App : Application
    {
        AuthService.AuthServiceClient _authServiceClient;
        GlobalDataProvider _globalDataProvider;

        public App(AuthService.AuthServiceClient authServiceClient, GlobalDataProvider globalDataProvider)
        {
            InitializeComponent();
            _authServiceClient = authServiceClient;
            _globalDataProvider = globalDataProvider;
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new LoginView(_authServiceClient, _globalDataProvider));
        }
    }
}