using BusinessSharkClient.Data.Sync;
using BusinessSharkClient.Logic;
using BusinessSharkService;
using Grpc.Core;

namespace BusinessSharkClient.Pages;

public partial class LoginPage : ContentPage
{
    private readonly AuthService.AuthServiceClient _authServiceClient;
    private readonly GlobalDataProvider _globalDataProvider;
    private readonly SyncEngine _syncEngine;

    public LoginPage(AuthService.AuthServiceClient authServiceClient, GlobalDataProvider globalDataProvider, SyncEngine syncEngine)
	{
		InitializeComponent();
        _authServiceClient = authServiceClient;
        _globalDataProvider = globalDataProvider;
        _syncEngine = syncEngine;
        Shell.SetFlyoutBehavior(this, FlyoutBehavior.Disabled);
    }

    private void OnCreateAccountClicked(object sender, EventArgs e)
    {
        // Goto to shell application main page
        if (Application.Current!.Windows.Count > 0)
        {
            Application.Current.Windows[0].Page = new CreateAccountPage(_authServiceClient, _globalDataProvider);
        }
    }

    private async void OnLoginClicked(object sender, EventArgs e)
	{
        try
        {
            ShowPopup("Authentication ....");
            var loginResult = await _authServiceClient.LoginAsync(new LoginRequest
                {
                    Username = EmailEntry.Text,
                    Password = PasswordEntry.Text,
                },
                deadline: DateTime.UtcNow.AddSeconds(5));

            if (string.IsNullOrEmpty(loginResult.AccessToken))
            {
                await DisplayAlert("Error", "Invalid username or password", "OK");
            }
            else
            {
                await SecureStorage.Default.SetAsync("access_token", loginResult.AccessToken);
                await SecureStorage.Default.SetAsync("refresh_token", loginResult.RefreshToken);
                await SecureStorage.Default.SetAsync("current_user", EmailEntry.Text);
                await SecureStorage.Default.SetAsync("player_id", loginResult.PlayerId.ToString());
                await SecureStorage.Default.SetAsync("company_id", loginResult.CompanyId.ToString());
                await SecureStorage.Default.SetAsync("company_name", loginResult.CompanyName);

                // Load global data
                ShowPopup("Synchronizing data ....");
                await _syncEngine.StartGlobalDataSync(CancellationToken.None);

                await _syncEngine.StartBackgroundSync(loginResult.CompanyId, CancellationToken.None);

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
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"An unexpected error occurred: {ex.Message}", "OK");
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