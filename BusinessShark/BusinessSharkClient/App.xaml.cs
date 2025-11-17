using BusinessSharkClient.Logic;
using BusinessSharkClient.Logic.Models;
using BusinessSharkClient.Pages;
using BusinessSharkService;

namespace BusinessSharkClient
{
    public partial class App : Application
    {
        public class DashedLineDrawable : IDrawable
        {
            public void Draw(ICanvas canvas, RectF dirtyRect)
            {
                canvas.StrokeColor = Color.FromRgb(235,235,235);
                canvas.StrokeSize = 1;
                canvas.StrokeDashPattern = new float[] { 4, 2 }; // 4px линия, 2px пробел
                canvas.DrawLine(0, 0, dirtyRect.Width, 0);
            }
        }

        public static SummaryModel Summary { get; } = new();

        private CompanyService.CompanyServiceClient _companyServiceClient;
        private AuthService.AuthServiceClient _authServiceClient;
        private GlobalDataProvider _globalDataProvider;
        private SummaryService.SummaryServiceClient _summaryServiceClient;

        public App(AuthService.AuthServiceClient authServiceClient,
            CompanyService.CompanyServiceClient companyServiceClient,
            SummaryService.SummaryServiceClient summaryServiceClient,
            GlobalDataProvider globalDataProvider)
        {
            InitializeComponent();
            _authServiceClient = authServiceClient;
            _companyServiceClient = companyServiceClient;
            _globalDataProvider = globalDataProvider;
            _summaryServiceClient = summaryServiceClient;

            BindingContext = Summary;

            Resources.Add("DashedLineDrawable", new DashedLineDrawable());
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new LoginPage(_authServiceClient, _globalDataProvider));
        }

        public async void OnLoad(object sender, EventArgs e)
        {
            if (!Summary.IsFirstLoad) await RefreshSummary();
        }

        public async void OnRefreshClicked(object sender, EventArgs e)
        {
            await RefreshSummary();
        }

        private async Task RefreshSummary()
        {
            int playerId = int.Parse(await SecureStorage.Default.GetAsync("player_id") ?? "0");
            var summary = await _summaryServiceClient.LoadAsync(new SummaryRequest { PlayerId = playerId });

            Summary.Balance = summary.Balance;
            Summary.Profit = summary.Income - summary.Expenses;
            Summary.IsFirstLoad = true;
        }
    }
}