using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsPresentation;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace WPFMapApp
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // Cấu hình bản đồ
            MapControl.MapProvider = GMapProviders.GoogleMap;
            MapControl.Position = new PointLatLng(21.028511, 105.804817); // Hà Nội
            MapControl.MinZoom = 2;
            MapControl.MaxZoom = 18;
            MapControl.Zoom = 10;
            MapControl.ShowCenter = false;

            // Đọc và giải mã dữ liệu JSON
            string filePath = @"D:\Lập trình .NET\MAP C001\MAP C001\Assets\districtsData.json";

            // Kiểm tra nếu tệp JSON tồn tại
            if (File.Exists(filePath))
            {
                string jsonData = File.ReadAllText(filePath);

                // Deserialize the entire RootObject
                var rootObject = JsonConvert.DeserializeObject<Client.Model.DataModel.RootObject>(jsonData);

                // If deserialization succeeds, you can access Coordinates and EnvironmentalData
                if (rootObject != null)
                {
                    var coordinateDataList = rootObject.Coordinates;
                    var environmentalDataList = rootObject.EnvironmentalData;

                    // Tạo marker cho các quận trên bản đồ
                    foreach (var coordinate in coordinateDataList)
                    {
                        // Lấy dữ liệu môi trường tương ứng với tọa độ hiện tại
                        var environmentalData = environmentalDataList.Find(e => e.CoordinateId == coordinate.Id);

                        // Tạo marker với vị trí quận
                        var marker = new GMapMarker(new PointLatLng(coordinate.Lat, coordinate.Lon))
                        {
                            Shape = new System.Windows.Controls.Image
                            {
                                Source = new System.Windows.Media.Imaging.BitmapImage(new Uri("pack://application:,,,/Assets/marker_icon.png")), // Hình ảnh icon đẹp
                                Width = 40,
                                Height = 40
                            }
                        };

                        // Đăng ký sự kiện click cho marker
                        marker.Shape.MouseLeftButtonDown += (s, e) =>
                        {
                            ShowDistrictInfo(coordinate, environmentalData);
                        };

                        // Thêm marker vào bản đồ
                        MapControl.Markers.Add(marker);
                    }
                }
                else
                {
                    MessageBox.Show("Error deserializing JSON data.");
                }
            }
            else
            {
                MessageBox.Show("The districts data file is missing.");
            }
        }

        private void ShowDistrictInfo(Client.Model.DataModel.Coordinate coordinate, Client.Model.DataModel.EnvironmentalData environmentalData)
        {
            // Hiển thị thông tin tiêu đề
            InfoTitle.Text = $"District: {coordinate.District}";

            // Cập nhật từng dòng thông tin
            InfoDescription.Text = $"Description: {coordinate.Description}";
            if (environmentalData != null)
            {
                InfoTemperature.Text = $"Temperature: {environmentalData.Temp - 273.15:F2} °C";
                InfoAQI.Text = $"AQI: {environmentalData.Aqi}";
                InfoPressure.Text = $"Pressure: {environmentalData.Pressure} hPa";
                InfoHumidity.Text = $"Humidity: {environmentalData.Humidity}%";
                InfoNO2.Text = $"NO2: {environmentalData.No2:F2} µg/m³";
                InfoCO.Text = $"CO: {environmentalData.Co:F2} µg/m³";
                InfoSO2.Text = $"SO2: {environmentalData.So2:F2} µg/m³";
                InfoPM25.Text = $"PM2.5: {environmentalData.Pm2_5:F2} µg/m³";
            }
            else
            {
                InfoTemperature.Text = "Temperature: N/A";
                InfoAQI.Text = "AQI: N/A";
                InfoPressure.Text = "Pressure: N/A";
                InfoHumidity.Text = "Humidity: N/A";
                InfoNO2.Text = "NO2: N/A";
                InfoCO.Text = "CO: N/A";
                InfoSO2.Text = "SO2: N/A";
                InfoPM25.Text = "PM2.5: N/A";
            }

            // Hiển thị khung thông tin
            InfoBorder.Visibility = Visibility.Visible;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            InfoBorder.Visibility = Visibility.Collapsed;
        }
    }
}

namespace Client.Model
{
    public class DataModel
    {
        public class Coordinate
        {
            public int Id { get; set; }
            public double Lon { get; set; }
            public double Lat { get; set; }
            public string District { get; set; }
            public string Description { get; set; }
        }

        public class EnvironmentalData
        {
            public string Id { get; set; }
            public int CoordinateId { get; set; }
            public long Dt { get; set; }
            public string DateTime { get; set; }
            public double Temp { get; set; }
            public double FeelsLike { get; set; }
            public double Pressure { get; set; }
            public int Humidity { get; set; }
            public double TempMin { get; set; }
            public double TempMax { get; set; }
            public int Aqi { get; set; }
            public double Co { get; set; }
            public double No { get; set; }
            public double No2 { get; set; }
            public double O3 { get; set; }
            public double So2 { get; set; }
            public double Pm2_5 { get; set; }
            public double Pm10 { get; set; }
            public double Nh3 { get; set; }
        }

        public class RootObject
        {
            public List<Coordinate> Coordinates { get; set; }
            public List<EnvironmentalData> EnvironmentalData { get; set; }
        }
    }
}
