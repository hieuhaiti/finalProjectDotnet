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
        private ObservableCollection<EnvironmentalDataEntry> EnvironmentalData;
        private ObservableCollection<Coordinate> StationData;
        private int CurrentPage = 1;
        private int ItemsPerPage = 20;
        private HttpClient _httpClient;
        private string _apiBaseUrl;
        private bool _isLoading;

        public event PropertyChangedEventHandler PropertyChanged;

        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                _isLoading = value;
                OnPropertyChanged(nameof(IsLoading));
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
            UpdatePagination();
        }

        private async void LoadEnvironmentalData()
        {
            IsLoading = true;
            try
            {
                // Directly deserialize into a List<EnvironmentalDataEntry>
                var response = await _httpClient.GetFromJsonAsync<List<EnvironmentalDataEntry>>($"{_apiBaseUrl}api/environmentaldata/all");

                if (response != null)
                {
                    // Use the response directly
                    EnvironmentalData = new ObservableCollection<EnvironmentalDataEntry>(response);
                    UpdatePagination();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading environmental data: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async void LoadStationData()
        {
            IsLoading = true;
            try
            {
                // Directly deserialize into a List<Coordinate>
                string url = $"{_apiBaseUrl}api/coordinates";
                var response = await _httpClient.GetFromJsonAsync<List<Coordinate>>(url);

                if (response != null)
                {
                    // Use the response directly
                    StationData = new ObservableCollection<Coordinate>(response);
                    CoordinatesGrid.ItemsSource = StationData;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading station data: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }


        // Handle Add Button Click for Environmental Data
        private async void AddButton_Click(object sender, RoutedEventArgs e)
        {
            // Open the AddDataPopup window
            AddDataPopup addDataPopup = new AddDataPopup();
            bool? result = addDataPopup.ShowDialog();

            if (result == true)
            {
                var newEntry = addDataPopup.NewEnvironmentalData;
                var createdEntry = await AddEnvironmentalData(newEntry);
                if (createdEntry != null)
                {
                    EnvironmentalData.Add(createdEntry);
                    UpdatePagination();
                    MessageBox.Show("New entry added!");
                }
                else
                {
                    MessageBox.Show("Failed to add new entry.");
                }
            }
        }

        // API call to add new environmental data
        private async Task<EnvironmentalDataEntry> AddEnvironmentalData(EnvironmentalDataEntry entry)
        {
            var response = await _httpClient.PostAsJsonAsync($"{_apiBaseUrl}api/environmentaldata/current", entry);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<EnvironmentalDataEntry>();
            }
            return null;
        }

        // Search function to filter data
        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            string query = SearchBox.Text.ToLower();
            var filteredData = EnvironmentalData.Where(x => x.coordinateId.ToString().Contains(query) || x.temp.ToString().Contains(query)).ToList();
            EnvironmentalDataGrid.ItemsSource = new ObservableCollection<EnvironmentalDataEntry>(filteredData);
        }

        // Update pagination
        private void UpdatePagination()
        {
            if (EnvironmentalData != null)
            {
                var paginatedData = EnvironmentalData.Skip((CurrentPage - 1) * ItemsPerPage).Take(ItemsPerPage).ToList();
                EnvironmentalDataGrid.ItemsSource = paginatedData;

                int totalPages = (int)Math.Ceiling((double)EnvironmentalData.Count / ItemsPerPage);
                PageInfoText.Text = $"Page {CurrentPage} of {totalPages}";
            }
        }


        // Pagination - Previous Page
        private void PreviousPageButton_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentPage > 1)
            {
                CurrentPage--;
                UpdatePagination();
            }
        }

        // Pagination - Next Page
        private void NextPageButton_Click(object sender, RoutedEventArgs e)
        {
            int totalPages = (int)Math.Ceiling((double)EnvironmentalData.Count / ItemsPerPage);
            if (CurrentPage < totalPages)
            {
                CurrentPage++;
                UpdatePagination();
            }
        }

        // Edit Environmental Data Entry
        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var entry = button?.DataContext as EnvironmentalDataEntry;
            if (entry != null)
            {
                // Implement edit logic here
                MessageBox.Show($"Editing entry: {entry.id}");
            }
        }

        // Delete Environmental Data Entry
        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var entry = button?.DataContext as EnvironmentalDataEntry;
            if (entry != null)
            {
                EnvironmentalData.Remove(entry);
                UpdatePagination();
                MessageBox.Show($"Deleted entry: {entry.id}");
            }
        }

        // Edit Station (Coordinate)
        private void EditStationButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var coordinate = button?.DataContext as Coordinate;
            if (coordinate != null)
            {
                // Implement edit logic here
                MessageBox.Show($"Editing station: {coordinate.id}");
            }
        }

        // Delete Station (Coordinate)
        private void DeleteStationButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var coordinate = button?.DataContext as Coordinate;
            if (coordinate != null)
            {
                StationData.Remove(coordinate);
                MessageBox.Show($"Deleted station: {coordinate.id}");
            }
        }

        // Search Box Text Changed
        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            SearchButton_Click(sender, e);
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

}