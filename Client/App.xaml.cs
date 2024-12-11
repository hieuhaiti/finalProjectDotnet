using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Windows;
using System.Windows.Input;

namespace Client
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
{
        private object _currentPage;
        public object CurrentPage
        {
            get => _currentPage;
            set
            {
                _currentPage = value;
                OnPropertyChanged(nameof(CurrentPage));
            }
        }

        public ICommand NavigateCommand { get; }

        public MainViewModel()
        {
            NavigateCommand = new RelayCommand<string>(Navigate);
            CurrentPage = new Page1(); // Trang mặc định
        }

        private void Navigate(string pageName)
        {
            CurrentPage = pageName switch
            {
                "Page1" => new Page1(),
                "Page2" => new Page2(),
                "Page3" => new Page3(),
                _ => CurrentPage
            };
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}