using BusinessSharkService;
using Grpc.Core;

namespace BusinessSharkClient.View;

public partial class LoginView : ContentPage
{
    AuthService.AuthServiceClient _authServiceClient;

    public LoginView(AuthService.AuthServiceClient authServiceClient)
	{
		InitializeComponent();
        _authServiceClient = authServiceClient;
        Shell.SetFlyoutBehavior(this, FlyoutBehavior.Disabled);
    }

    private async void OnLoginClicked(object sender, EventArgs e)
	{
        try
        {
            var loginResult = await _authServiceClient.LoginAsync(new LoginRequest
            {
                Username = EmailEntry.Text,
                Password = PasswordEntry.Text
            });
            if (string.IsNullOrEmpty(loginResult.AccessToken))
            {
                await DisplayAlert("Error", "Invalid username or password", "OK");
                return;
            }
            else
            {
                await SecureStorage.Default.SetAsync("access_token", loginResult.AccessToken);
                await SecureStorage.Default.SetAsync("current_user", EmailEntry.Text);

                Application.Current.MainPage = new AppShell();
                //await Shell.Current.GoToAsync($"//{nameof(OfficeView)}", false);
            }

        }
        catch (RpcException rpcEx) when (rpcEx.StatusCode == StatusCode.Unavailable)
        {
            await DisplayAlert("Error", "Cannot connect to the server. Please try again later.", "OK");
            return;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"An unexpected error occurred: {ex.Message}", "OK");
            return;
        }
    }
}