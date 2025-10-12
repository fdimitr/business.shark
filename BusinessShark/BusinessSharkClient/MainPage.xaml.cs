using BusinessSharkService;
using Grpc.Core;

namespace BusinessSharkClient
{
    public partial class MainPage : ContentPage
    {
        int count = 0;
        BusinessSharkService.Greeter.GreeterClient _greeterClient;
        BusinessSharkService.AuthService.AuthServiceClient _authServiceClient;

        public MainPage(BusinessSharkService.Greeter.GreeterClient greeterClient, BusinessSharkService.AuthService.AuthServiceClient authServiceClient)
        {
            InitializeComponent();
            _greeterClient = greeterClient;
            _authServiceClient = authServiceClient;
        }

        private async void OnCounterClicked(object? sender, EventArgs e)
        {
            count++;

            if (count == 1)
                CounterBtn.Text = $"Clicked {count} time";
            else
                CounterBtn.Text = $"Clicked {count} times";

            // 1. Авторизация
            //var login = await _authServiceClient.LoginAsync(new LoginRequest
            //{
            //    Username = "admin",
            //    Password = "12345"
            //});

            //var headers = new Metadata { { "Authorization", $"Bearer {login.AccessToken}" } };
            var response = await _greeterClient.SayHelloAsync(new BusinessSharkService.HelloRequest
            {
                Name = Environment.MachineName
            }/*, headers*/);

            ResponseLabel.Text = response.Message;
            SemanticScreenReader.Announce(CounterBtn.Text);
        }
    }
}
