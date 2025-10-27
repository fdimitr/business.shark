using System.ComponentModel;
using System.Runtime.CompilerServices;

public class SummaryModel : INotifyPropertyChanged
{
    public bool IsFirstLoad { get; set; } = false;

    private double _balance;
    public double Balance
    {
        get => _balance;
        set { _balance = value; OnPropertyChanged(); }
    }

    private double _profit;
    public double Profit
    {
        get => _profit;
        set { _profit = value; OnPropertyChanged(nameof(Profit)); }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    private void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}