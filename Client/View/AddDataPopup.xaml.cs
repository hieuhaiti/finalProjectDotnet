using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Windows;
using static Client.Model.DataModel;

namespace Client.View
{
    public partial class AddDataPopup : Window
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiBaseUrl;

        public Coordinate NewCoordinate { get; private set; }
        public EnvironmentalDataEntry NewEnvironmentalData { get; private set; }

        public AddDataPopup()
        {
            InitializeComponent();
            _httpClient = new HttpClient();
            _apiBaseUrl = App.Configuration["api:localhost"];
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataTabControl.SelectedIndex == 0) // If "Station" tab is selected
            {
                if (double.TryParse(LongitudeTextBox.Text, out double lon) &&
                    double.TryParse(LatitudeTextBox.Text, out double lat) &&
                    !string.IsNullOrWhiteSpace(DistrictTextBox.Text) &&
                    !string.IsNullOrWhiteSpace(DescriptionTextBox.Text))
                {
                    NewCoordinate = new Coordinate
                    {
                        lon = lon,
                        lat = lat,
                        district = DistrictTextBox.Text,
                        description = DescriptionTextBox.Text
                    };

                    var success = await PostCoordinate(NewCoordinate);
                    if (success)
                    {
                        DialogResult = true;
                        Close();
                    }
                    else
                    {
                        MessageBox.Show("Failed to save Coordinate. Please try again.");
                    }
                }
                else
                {
                    MessageBox.Show("Please fill all fields with valid data.");
                }
            }
            else if (DataTabControl.SelectedIndex == 1) // If "Environmental Data" tab is selected
            {
                if (int.TryParse(EnvCoordinateIdTextBox.Text, out int coordinateId) &&
                    double.TryParse(EnvTempTextBox.Text, out double temp) &&
                    double.TryParse(EnvFeelsLikeTextBox.Text, out double feelsLike) &&
                    int.TryParse(EnvPressureTextBox.Text, out int pressure) &&
                    int.TryParse(EnvHumidityTextBox.Text, out int humidity) &&
                    double.TryParse(EnvTempMinTextBox.Text, out double tempMin) &&
                    double.TryParse(EnvTempMaxTextBox.Text, out double tempMax) &&
                    int.TryParse(EnvAqiTextBox.Text, out int aqi) &&
                    double.TryParse(EnvCoTextBox.Text, out double co) &&
                    double.TryParse(EnvNoTextBox.Text, out double no) &&
                    double.TryParse(EnvNo2TextBox.Text, out double no2) &&
                    double.TryParse(EnvO3TextBox.Text, out double o3) &&
                    double.TryParse(EnvSo2TextBox.Text, out double so2) &&
                    double.TryParse(EnvPm25TextBox.Text, out double pm2_5) &&
                    double.TryParse(EnvPm10TextBox.Text, out double pm10) &&
                    double.TryParse(EnvNh3TextBox.Text, out double nh3))
                {
                    var dateTimeNow = DateTime.UtcNow;
                    NewEnvironmentalData = new EnvironmentalDataEntry
                    {
                        coordinateId = coordinateId,
                        dt = new DateTimeOffset(dateTimeNow).ToUnixTimeSeconds(),
                        dateTime = dateTimeNow,
                        temp = temp,
                        feelsLike = feelsLike,
                        pressure = pressure,
                        humidity = humidity,
                        tempMin = tempMin,
                        tempMax = tempMax,
                        aqi = aqi,
                        co = co,
                        no = no,
                        no2 = no2,
                        o3 = o3,
                        so2 = so2,
                        pm2_5 = pm2_5,
                        pm10 = pm10,
                        nh3 = nh3
                    };

                    var success = await PostEnvironmentalData(NewEnvironmentalData);
                    if (success)
                    {
                        DialogResult = true;
                        Close();
                    }
                    else
                    {
                        MessageBox.Show("Failed to save Environmental Data. Please try again.");
                    }
                }
                else
                {
                    MessageBox.Show("Please fill all fields with valid data.");
                }
            }
        }

        private async Task<bool> PostCoordinate(Coordinate coordinate)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync($"{_apiBaseUrl}api/coordinates", coordinate);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error posting Coordinate: {ex.Message}");
                return false;
            }
        }

        private async Task<bool> PostEnvironmentalData(EnvironmentalDataEntry environmentalData)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync($"{_apiBaseUrl}api/EnvironmentalData/current", environmentalData);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error posting Environmental Data: {ex.Message}");
                return false;
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}