namespace BusinessSharkClient
{
    public partial class MainPage : ContentPage
    {
        int count = 0;
        BusinessSharkService.Greeter.GreeterClient _greeterClient;

        public MainPage(BusinessSharkService.Greeter.GreeterClient greeterClient)
        {
            InitializeComponent();
            _greeterClient = greeterClient;
        }

        private async void OnCounterClicked(object? sender, EventArgs e)
        {
            count++;

            if (count == 1)
                CounterBtn.Text = $"Clicked {count} time";
            else
                CounterBtn.Text = $"Clicked {count} times";

            var response = await _greeterClient.SayHelloAsync(new BusinessSharkService.HelloRequest
            {
                Name = Environment.MachineName
            });
            ResponseLabel.Text = response.Message;
            SemanticScreenReader.Announce(CounterBtn.Text);
        }
    }
}
