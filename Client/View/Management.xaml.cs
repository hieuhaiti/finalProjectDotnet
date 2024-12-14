using Client.Model;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;
using static Client.Model.DataModel;

namespace Client.View
{
    public partial class Management : Page, INotifyPropertyChanged
    {
        private ObservableCollection<EnvironmentalDataEntry> _environmentalData;
        private ObservableCollection<Coordinate> _stationData;
        private int _currentPageEnvironmental = 1;
        private int _currentPageStation = 1;
        private int _itemsPerPage = 25;
        private HttpClient _httpClient;
        private string _apiBaseUrl;

        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<EnvironmentalDataEntry> EnvironmentalData
        {
            get => _environmentalData;
            set
            {
                _environmentalData = value;
                OnPropertyChanged(nameof(EnvironmentalData));
            }
        }

        public ObservableCollection<Coordinate> StationData
        {
            get => _stationData;
            set
            {
                _stationData = value;
                OnPropertyChanged(nameof(StationData));
            }
        }

        public Management()
        {
            InitializeComponent();
            DataContext = this;
            _httpClient = new HttpClient();
            _apiBaseUrl = App.Configuration["api:localhost"];
            LoadEnvironmentalData();
            LoadStationData();
        }

        private async Task LoadEnvironmentalData()
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<List<EnvironmentalDataEntry>>($"{_apiBaseUrl}api/environmentaldata/all");

                if (response != null)
                {
                    EnvironmentalData = new ObservableCollection<EnvironmentalDataEntry>(response.OrderByDescending(x => x.dateTime));
                    UpdatePaginationEnvironmental();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading environmental data: {ex.Message}");
            }
        }

        private async Task LoadStationData()
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<List<Coordinate>>($"{_apiBaseUrl}api/coordinates");

                if (response != null)
                {
                    StationData = new ObservableCollection<Coordinate>(response);
                    UpdatePaginationStation();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading station data: {ex.Message}");
            }
        }

        private async void AddButton_Click(object sender, RoutedEventArgs e)
        {
            var addDataPopup = new AddDataPopup();
            bool? result = addDataPopup.ShowDialog();

            if (result == true)
            {
                if (addDataPopup.NewCoordinate != null)
                {
                    await LoadStationData();
                    MessageBox.Show("New Station added successfully.");
                }
                else if (addDataPopup.NewEnvironmentalData != null)
                {
                    await LoadEnvironmentalData();
                    MessageBox.Show("New Environmental Data added successfully.");
                }
            }
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            string query = SearchBox.Text.ToLower();
            if (EnvironmentalDataGrid.IsVisible)
            {
                var filteredData = EnvironmentalData.Where(x => x.id.ToString().Contains(query)).ToList();
                EnvironmentalDataGrid.ItemsSource = new ObservableCollection<EnvironmentalDataEntry>(filteredData);
            }
            else if (CoordinatesGrid.IsVisible)
            {
                var filteredData = StationData.Where(x => x.id.ToString().Contains(query)).ToList();
                CoordinatesGrid.ItemsSource = new ObservableCollection<Coordinate>(filteredData);
            }
        }

        private void UpdatePaginationEnvironmental()
        {
            if (EnvironmentalData != null)
            {
                var paginatedData = EnvironmentalData.Skip((_currentPageEnvironmental - 1) * _itemsPerPage).Take(_itemsPerPage).ToList();
                EnvironmentalDataGrid.ItemsSource = paginatedData;

                int totalPages = (int)Math.Ceiling((double)EnvironmentalData.Count / _itemsPerPage);
                PageInfoTextEnvironmental.Text = $"Page {_currentPageEnvironmental} of {totalPages}";
            }
        }

        private void UpdatePaginationStation()
        {
            if (StationData != null)
            {
                var paginatedData = StationData.Skip((_currentPageStation - 1) * _itemsPerPage).Take(_itemsPerPage).ToList();
                CoordinatesGrid.ItemsSource = paginatedData;

                int totalPages = (int)Math.Ceiling((double)StationData.Count / _itemsPerPage);
                PageInfoTextStation.Text = $"Page {_currentPageStation} of {totalPages}";
            }
        }

        private void PreviousPageButtonEnvironmental_Click(object sender, RoutedEventArgs e)
        {
            if (_currentPageEnvironmental > 1)
            {
                _currentPageEnvironmental--;
                UpdatePaginationEnvironmental();
            }
        }

        private void NextPageButtonEnvironmental_Click(object sender, RoutedEventArgs e)
        {
            int totalPages = (int)Math.Ceiling((double)EnvironmentalData.Count / _itemsPerPage);
            if (_currentPageEnvironmental < totalPages)
            {
                _currentPageEnvironmental++;
                UpdatePaginationEnvironmental();
            }
        }

        private void PreviousPageButtonStation_Click(object sender, RoutedEventArgs e)
        {
            if (_currentPageStation > 1)
            {
                _currentPageStation--;
                UpdatePaginationStation();
            }
        }

        private void NextPageButtonStation_Click(object sender, RoutedEventArgs e)
        {
            int totalPages = (int)Math.Ceiling((double)StationData.Count / _itemsPerPage);
            if (_currentPageStation < totalPages)
            {
                _currentPageStation++;
                UpdatePaginationStation();
            }
        }

        private async void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var entry = button?.DataContext as EnvironmentalDataEntry;
            if (entry != null)
            {
                var result = MessageBox.Show($"Are you sure you want to delete entry: {entry.id}?", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    var success = await DeleteEnvironmentalData(entry.id);
                    if (success)
                    {
                        EnvironmentalData.Remove(entry);
                        UpdatePaginationEnvironmental();
                        MessageBox.Show($"Deleted entry: {entry.id}");
                    }
                    else
                    {
                        MessageBox.Show("Failed to delete entry. Please try again.");
                    }
                }
            }
        }

        private async void DeleteStationButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var coordinate = button?.DataContext as Coordinate;
            if (coordinate != null)
            {
                var result = MessageBox.Show($"Are you sure you want to delete station: {coordinate.id}?", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    var success = await DeleteCoordinate(coordinate.id);
                    if (success)
                    {
                        StationData.Remove(coordinate);
                        UpdatePaginationStation();
                        MessageBox.Show($"Deleted station: {coordinate.id}");
                    }
                    else
                    {
                        MessageBox.Show("Failed to delete station. Please try again.");
                    }
                }
            }
        }

        private async Task<bool> DeleteEnvironmentalData(Guid id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"{_apiBaseUrl}api/EnvironmentalData/{id}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting Environmental Data: {ex.Message}");
                return false;
            }
        }

        private async Task<bool> DeleteCoordinate(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"{_apiBaseUrl}api/coordinates/{id}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting Coordinate: {ex.Message}");
                return false;
            }
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var entry = button?.DataContext as EnvironmentalDataEntry;
            if (entry != null)
            {
                var editDataPopup = new EditDataPopup(environmentalData: entry);
                bool? result = editDataPopup.ShowDialog();

                if (result == true && editDataPopup.EditedEnvironmentalData != null)
                {
                    var index = EnvironmentalData.IndexOf(entry);
                    EnvironmentalData[index] = editDataPopup.EditedEnvironmentalData;
                    UpdatePaginationEnvironmental();
                    MessageBox.Show("Environmental Data updated successfully.");
                }
            }
        }

        private void EditStationButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var coordinate = button?.DataContext as Coordinate;
            if (coordinate != null)
            {
                var editDataPopup = new EditDataPopup(coordinate: coordinate);
                bool? result = editDataPopup.ShowDialog();

                if (result == true && editDataPopup.EditedCoordinate != null)
                {
                    var index = StationData.IndexOf(coordinate);
                    StationData[index] = editDataPopup.EditedCoordinate;
                    UpdatePaginationStation();
                    MessageBox.Show("Station updated successfully.");
                }
            }
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            SearchButton_Click(sender, e);
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class ApiResponse<T>
    {
        public List<T> Data { get; set; }
    }
}