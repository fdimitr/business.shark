using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace BusinessSharkClient.Logic.Models
{
    public class ShellHeaderViewModel : INotifyPropertyChanged
    {
        private string _title = string.Empty;
        private string _companyName = string.Empty;

        public string Title
        {
            get => _title;
            set
            {
                if (_title != value)
                {
                    _title = value;
                    OnPropertyChanged();
                }
            }
        }

        public string CompanyName
        {
            get => _companyName;

            set
            {
                if (_companyName != value)
                {
                    _companyName = value;
                    OnPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

}
