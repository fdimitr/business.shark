using BusinessSharkClient.Data.Sync;
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
                canvas.StrokeDashPattern = [4, 2]; // 4px линия, 2px пробел
                canvas.DrawLine(0, 0, dirtyRect.Width, 0);
            }
        }

        public static SummaryModel Summary { get; } = new();

        private readonly AuthService.AuthServiceClient _authServiceClient;
        private readonly GlobalDataProvider _globalDataProvider;
        private readonly SummaryService.SummaryServiceClient _summaryServiceClient;
        private readonly SyncEngine _syncEngine;

        public App(AuthService.AuthServiceClient authServiceClient,
            SummaryService.SummaryServiceClient summaryServiceClient,
            SyncEngine syncEngine,
            GlobalDataProvider globalDataProvider)
        {
            InitializeComponent();
            _authServiceClient = authServiceClient;
            _globalDataProvider = globalDataProvider;
            _summaryServiceClient = summaryServiceClient;
            _syncEngine = syncEngine;

            BindingContext = Summary;

            Resources.Add("DashedLineDrawable", new DashedLineDrawable());
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new LoginPage(_authServiceClient, _globalDataProvider, _syncEngine));
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