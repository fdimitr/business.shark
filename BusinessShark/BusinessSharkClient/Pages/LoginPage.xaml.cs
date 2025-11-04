using BusinessSharkClient.Logic;
using BusinessSharkService;
using Grpc.Core;

namespace BusinessSharkClient.View;

public partial class LoginPage : ContentPage
{
    private AuthService.AuthServiceClient _authServiceClient;
    private CompanyService.CompanyServiceClient _companyServiceClient;
    private GlobalDataProvider _globalDataProvider;

    public LoginPage(AuthService.AuthServiceClient authServiceClient, GlobalDataProvider globalDataProvider, CompanyService.CompanyServiceClient companyServiceClient)
	{
		InitializeComponent();
        _authServiceClient = authServiceClient;
        _globalDataProvider = globalDataProvider;
        _companyServiceClient = companyServiceClient;
        Shell.SetFlyoutBehavior(this, FlyoutBehavior.Disabled);
    }

    private async void OnLoginClicked(object sender, EventArgs e)
	{
        ShowPopup("Authentification ....");
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
                await SecureStorage.Default.SetAsync("player_id", loginResult.PlayerId.ToString());

                // Load global data
                ShowPopup("Synchronizing data ....");
                var company = await _companyServiceClient.GetByPlayerAsync(new GetByPlayerRequest { PlayerId = loginResult.PlayerId });
                await SecureStorage.Default.SetAsync("company_id", company.CompanyId.ToString());
                await SecureStorage.Default.SetAsync("company_name", company.Name);

                await _globalDataProvider.LoadData();

                // Goto to shell application main page
                if (Application.Current!.Windows.Count > 0)
                {
                    Application.Current.Windows[0].Page = new AppShell();
                }
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
        finally
        {
            HidePopup();
        }
    }

    private void ShowPopup(string message)
    {
        OverlayPanel.IsVisible = true;
        OverlayPanelText.Text = message;

        EmailEntry.IsEnabled = false;
        PasswordEntry.IsEnabled = false;
        BtnLogin.IsEnabled = false;
        BtnCreateAccount.IsEnabled = false;
    }

    private void HidePopup()
    {
        OverlayPanel.IsVisible = false;

        EmailEntry.IsEnabled = true;
        PasswordEntry.IsEnabled = true;
        BtnLogin.IsEnabled = true;
        BtnCreateAccount.IsEnabled = true;
    }
}