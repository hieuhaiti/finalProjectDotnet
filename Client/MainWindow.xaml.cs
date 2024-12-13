using Client.View; 
using System.Windows;

namespace Client
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            MainContentFrame.Content = new Dashboard();
        }

        // Navigate to Dashboard Page
        private void DashboardButton_Click(object sender, RoutedEventArgs e)
        {
            MainContentFrame.Content = new Dashboard();
        }

        // Navigate to Management Page
        private void ManagementButton_Click(object sender, RoutedEventArgs e)
        {
            MainContentFrame.Content = new Management(); 
        }

        // Navigate to Map Page
        private void MapButton_Click(object sender, RoutedEventArgs e)
        {
            MainContentFrame.Content = new Map(); 
        }
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Are you sure you want to exit?", "Confirm Exit", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                this.Close();
            }
        }

    }
}
