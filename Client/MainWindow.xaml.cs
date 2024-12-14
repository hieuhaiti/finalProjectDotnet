using Client.View;
using System.Windows;
using System.Windows.Controls;
using MahApps.Metro.IconPacks;

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
        // Maximize or restore the window size
        private void MaximizeRestoreButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
            {
                this.WindowState = WindowState.Normal;
                MaximizeRestoreButton.Content = new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    Children =
            {
                new PackIconMaterial { Kind = PackIconMaterialKind.WindowMaximize, Width = 20, Height = 20 },
                new TextBlock { Text = "Maximize", Margin = new Thickness(10, 0, 0, 0) }
            }
                };
            }
            else
            {
                this.WindowState = WindowState.Maximized;
                MaximizeRestoreButton.Content = new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    Children =
            {
                new PackIconMaterial { Kind = PackIconMaterialKind.WindowRestore, Width = 20, Height = 20 },
                new TextBlock { Text = "Restore", Margin = new Thickness(10, 0, 0, 0) }
            }
                };
            }
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
