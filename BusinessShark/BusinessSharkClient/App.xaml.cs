using BusinessSharkClient.View;
using BusinessSharkService;

namespace BusinessSharkClient
{
    public partial class App : Application
    {
        AuthService.AuthServiceClient _authServiceClient;

        public App(AuthService.AuthServiceClient authServiceClient)
        {
            InitializeComponent();
            _authServiceClient = authServiceClient;
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new LoginView(_authServiceClient));
        }
    }
}