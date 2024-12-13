using Client.View;
using System.ComponentModel;
using System.Windows.Input;

public class MainViewModel : INotifyPropertyChanged
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

    // Explicitly initialize PropertyChanged event
    public event PropertyChangedEventHandler? PropertyChanged;

    public MainViewModel()
    {
        NavigateCommand = new RelayCommand<string>(Navigate);
        CurrentPage = new Management(); // Default page
    }

    private void Navigate(string pageName)
    {
        CurrentPage = pageName switch
        {
            "ManagementPage" => new Management(),
            "DashboardPage" => new Dashboard(),
            "MapPage" => new Map(),
            _ => CurrentPage
        };
    }

    protected void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }


    public class RelayCommand<T> : ICommand
    {
        private readonly Action<T> _execute;
        private readonly Func<T, bool> _canExecute;

        public RelayCommand(Action<T> execute, Func<T, bool> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public bool CanExecute(object? parameter) // Marked parameter as nullable
        {
            return _canExecute == null || _canExecute((T)parameter!); // Use null-forgiving operator
        }

        public void Execute(object? parameter) // Marked parameter as nullable
        {
            _execute((T)parameter!); // Use null-forgiving operator
        }

        public event EventHandler? CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }

}
