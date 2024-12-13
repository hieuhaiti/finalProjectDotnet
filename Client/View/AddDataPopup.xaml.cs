using System;
using System.Windows;
using static Client.Model.DataModel;

namespace Client.View
{
    public partial class AddDataPopup : Window
    {
        public Coordinate NewCoordinate { get; private set; }
        public EnvironmentalDataEntry NewEnvironmentalData { get; private set; }

        public AddDataPopup()
        {
            InitializeComponent();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataTabControl.SelectedIndex == 0) // If "Station" tab is selected
            {
                // Validate and create Coordinate data
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

                    DialogResult = true; // Close window and signal success
                    Close();
                }
                else
                {
                    MessageBox.Show("Please fill all fields with valid data.");
                }
            }
            else if (DataTabControl.SelectedIndex == 1) // If "Environmental Data" tab is selected
            {
                // Validate and create Environmental Data
                if (int.TryParse(EnvCoordinateIdTextBox.Text, out int coordinateId) &&
                    double.TryParse(EnvTempTextBox.Text, out double temp) &&
                    double.TryParse(EnvFeelsLikeTextBox.Text, out double feelsLike) &&
                    int.TryParse(EnvPressureTextBox.Text, out int pressure) &&
                    int.TryParse(EnvHumidityTextBox.Text, out int humidity) &&
                    int.TryParse(EnvAqiTextBox.Text, out int aqi) &&
                    double.TryParse(EnvCoTextBox.Text, out double co) &&
                    double.TryParse(EnvPm25TextBox.Text, out double pm2_5))
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
                        tempMin = temp, // Add more logic to fill these fields as needed
                        tempMax = temp,
                        aqi = aqi,
                        co = co,
                        pm2_5 = pm2_5
                    };

                    DialogResult = true; // Close window and signal success
                    Close();
                }
                else
                {
                    MessageBox.Show("Please fill all fields with valid data.");
                }
            }
        }

        // Cancel button - Close the window without adding data
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false; // No action, just close the window
            Close();
        }
    }
}