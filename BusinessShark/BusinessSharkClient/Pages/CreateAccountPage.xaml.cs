using BusinessSharkClient.Logic;
using BusinessSharkService;

namespace BusinessSharkClient.Pages;

public partial class CreateAccountPage : ContentPage
{
    private AuthService.AuthServiceClient _authServiceClient;
    private GlobalDataProvider _globalDataProvider;
    public CreateAccountPage(AuthService.AuthServiceClient authServiceClient, GlobalDataProvider globalDataProvider)
    {
        _authServiceClient = authServiceClient;
        _globalDataProvider = globalDataProvider;
        InitializeComponent();
    }

    // Обработчик кнопки "Create Account"
    private async void OnCreateAccountClicked(object sender, EventArgs e)
    {
        // Находим все поля на странице
        var loginEntry = this.FindByName<Entry>("LoginEntry");
        var nameEntry = this.FindByName<Entry>("NameEntry");
        var passwordEntry = this.FindByName<Entry>("PasswordEntry");
        var companyEntry = this.FindByName<Entry>("CompanyEntry");

        // Проверка на заполнение обязательных полей
        if (string.IsNullOrWhiteSpace(loginEntry?.Text) ||
            string.IsNullOrWhiteSpace(nameEntry?.Text) ||
            string.IsNullOrWhiteSpace(passwordEntry?.Text) ||
            string.IsNullOrWhiteSpace(companyEntry?.Text))
        {
            await DisplayAlert("Error", "Please fill in all required fields.", "OK");
            return;
        }

        // Пример сохранения или отправки данных на сервер
        // (здесь можно добавить вызов API или обращение к сервису)
        await DisplayAlert("Success", $"Account for {nameEntry.Text} has been created successfully!", "OK");

        // Переход обратно на страницу входа
        OnNavigateToLoginClicked(sender, e);
    }

    // Обработчик кнопки "Log in"
    private void OnNavigateToLoginClicked(object sender, EventArgs e)
    {
        if (Application.Current!.Windows.Count > 0)
        {
            Application.Current.Windows[0].Page = new LoginPage(_authServiceClient, _globalDataProvider);
        }
    }
}