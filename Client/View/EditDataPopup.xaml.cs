using System;
using System.Windows;
using static Client.Model.DataModel;

namespace Client.View
{
    public partial class EditDataPopup : Window
    {
        public Coordinate EditedCoordinate { get; private set; }
        public EnvironmentalDataEntry EditedEnvironmentalData { get; private set; }

        public EditDataPopup(Coordinate coordinate = null, EnvironmentalDataEntry environmentalData = null)
        {
            InitializeComponent();

            if (coordinate != null)
            {
                DataTabControl.SelectedIndex = 0;
                LongitudeTextBox.Text = coordinate.lon.ToString();
                LatitudeTextBox.Text = coordinate.lat.ToString();
                DistrictTextBox.Text = coordinate.district;
                DescriptionTextBox.Text = coordinate.description;
            }
            else if (environmentalData != null)
            {
                DataTabControl.SelectedIndex = 1;
                EnvCoordinateIdTextBox.Text = environmentalData.coordinateId.ToString();
                EnvTempTextBox.Text = environmentalData.temp.ToString();
                EnvFeelsLikeTextBox.Text = environmentalData.feelsLike.ToString();
                EnvPressureTextBox.Text = environmentalData.pressure.ToString();
                EnvHumidityTextBox.Text = environmentalData.humidity.ToString();
                EnvAqiTextBox.Text = environmentalData.aqi.ToString();
                EnvCoTextBox.Text = environmentalData.co.ToString();
                EnvPm25TextBox.Text = environmentalData.pm2_5.ToString();
                EnvSo2TextBox.Text = environmentalData.so2.ToString();
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataTabControl.SelectedIndex == 0) // If "Station" tab is selected
            {
                if (double.TryParse(LongitudeTextBox.Text, out double lon) &&
                    double.TryParse(LatitudeTextBox.Text, out double lat) &&
                    !string.IsNullOrWhiteSpace(DistrictTextBox.Text) &&
                    !string.IsNullOrWhiteSpace(DescriptionTextBox.Text))
                {
                    EditedCoordinate = new Coordinate
                    {
                        lon = lon,
                        lat = lat,
                        district = DistrictTextBox.Text,
                        description = DescriptionTextBox.Text
                    };

                    DialogResult = true;
                    Close();
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
                    int.TryParse(EnvAqiTextBox.Text, out int aqi) &&
                    double.TryParse(EnvCoTextBox.Text, out double co) &&
                    double.TryParse(EnvPm25TextBox.Text, out double pm2_5) &&
                    double.TryParse(EnvSo2TextBox.Text, out double so2))
                {
                    var dateTimeNow = DateTime.UtcNow;
                    EditedEnvironmentalData = new EnvironmentalDataEntry
                    {
                        coordinateId = coordinateId,
                        dt = new DateTimeOffset(dateTimeNow).ToUnixTimeSeconds(),
                        dateTime = dateTimeNow,
                        temp = temp,
                        feelsLike = feelsLike,
                        pressure = pressure,
                        humidity = humidity,
                        aqi = aqi,
                        co = co,
                        pm2_5 = pm2_5,
                        so2 = so2
                    };

                    DialogResult = true;
                    Close();
                }
                else
                {
                    MessageBox.Show("Please fill all fields with valid data.");
                }
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}