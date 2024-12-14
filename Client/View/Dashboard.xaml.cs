using LiveCharts;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace Client.View
{

    public partial class Dashboard : Page
    {
        private string _apiBaseUrl;

        public Dashboard()
        {
            InitializeComponent();
            DataContext = this;
            _apiBaseUrl = App.Configuration["api:localhost"];

        }
        public class EnvironmentalData
        {
            [JsonProperty("dateTime")]
            public DateTime DateTime { get; set; }

            [JsonProperty("temp")]
            public double Temp { get; set; }

            [JsonProperty("feelsLike")]
            public double FeelsLike { get; set; }

            [JsonProperty("tempMin")]
            public double TempMin { get; set; }

            [JsonProperty("tempMax")]
            public double TempMax { get; set; }

            [JsonProperty("pressure")]
            public double Pressure { get; set; }

            [JsonProperty("humidity")]
            public double Humidity { get; set; }

            [JsonProperty("aqi")]
            public double AQI { get; set; }

            [JsonProperty("co")]
            public double CO { get; set; }

            [JsonProperty("no")]
            public double NO { get; set; }

            [JsonProperty("no2")]
            public double NO2 { get; set; }

            [JsonProperty("o3")]
            public double O3 { get; set; }

            [JsonProperty("so2")]
            public double SO2 { get; set; }

            [JsonProperty("pm2_5")]
            public double PM2_5 { get; set; }

            [JsonProperty("pm10")]
            public double PM10 { get; set; }

            [JsonProperty("nh3")]
            public double NH3 { get; set; }
        }

        private List<string> MonthLabels = new List<string>
        {
            "Tháng 1", "Tháng 2", "Tháng 3", "Tháng 4", "Tháng 5", "Tháng 6",
            "Tháng 7", "Tháng 8", "Tháng 9", "Tháng 10", "Tháng 11", "Tháng 12"
        };

        public List<double> MonthlyValues { get; set; }


        private async Task<EnvironmentalData> FetchCurrentDataAsync(int weatherDistrictId)
        {
            string apiUrl = $"{_apiBaseUrl}api/EnvironmentalData/district/id/{weatherDistrictId}";
            try
            {
                using (var client = new HttpClient())
                {
                    var response = await client.GetAsync(apiUrl);
                    if (response.IsSuccessStatusCode)
                    {
                        var jsonData = await response.Content.ReadAsStringAsync();

                        // Deserialize toàn bộ mảng
                        var dataList = JsonConvert.DeserializeObject<List<EnvironmentalData>>(jsonData);

                        // Lấy phần tử cuối cùng (mới nhất)
                        return dataList?.LastOrDefault();
                    }
                    else
                    {
                        MessageBox.Show($"Không thể tải dữ liệu từ API. Mã trạng thái: {response.StatusCode}");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi gọi API: {ex.Message}\n\nChi tiết: {ex.StackTrace}");
            }
            return null;
        }

        private async Task UpdateWeatherTable()
        {
            try
            {
                var selectedWeatherDistrict = DistrictForWeatherSelector.SelectedItem as ComboBoxItem;

                if (selectedWeatherDistrict == null)
                {
                    TableView.Visibility = Visibility.Hidden;
                    TableTitle.Text = "Vui lòng chọn quận";
                    LeftColumnContent.Text = "";
                    RightColumnContent.Text = "";
                    CurrentTimeTextBlock.Text = "Thời gian: Đang cập nhật";
                    return;
                }

                int weatherDistrictId;
                if (selectedWeatherDistrict.Tag != null && int.TryParse(selectedWeatherDistrict.Tag.ToString(), out weatherDistrictId))
                {
                    var data = await FetchCurrentDataAsync(weatherDistrictId);

                    if (data != null)
                    {
                        TableView.Visibility = Visibility.Visible;
                        TableTitle.Text = $"Dữ liệu môi trường mới nhất tại {selectedWeatherDistrict.Content}";

                        string formattedTime = FormatDateTime(data.DateTime);
                        CurrentTimeTextBlock.Text = $"Thời gian: {formattedTime}";

                        // Cột trái
                        LeftColumnContent.Text =
                            $"🌡️ Nhiệt độ: {data.Temp}°C\n" +
                            $"🌡️ Cảm giác: {data.FeelsLike}°C\n" +
                            $"🌡️ Nhiệt độ cao nhất: {data.TempMax}°C\n" +
                            $"🌡️ Nhiệt độ thấp nhất: {data.TempMin}°C\n" +
                            $"💧 Độ ẩm: {data.Humidity}%\n";


                        // Cột giữa
                        MiddleColumnContent.Text =
                            $"📊 Áp suất: {data.Pressure} hPa\n" +
                            $"🌈 Chỉ số AQI: {data.AQI}\n" +
                            $"💨 CO: {data.CO}\n" +
                            $"💨 NO: {data.NO}\n" +
                            $"💨 NO2: {data.NO2}\n";

                        // Cột phải
                        RightColumnContent.Text =
                                $"💨 O3: {data.O3}\n" +
                                $"💨 SO2: {data.SO2}\n" +
                                $"💨 PM2.5: {data.PM2_5}\n" +
                                $"💨 PM10: {data.PM10}\n" +
                                $"💨 NH3: {data.NH3}\n";
                    }
                    else
                    {
                        TableView.Visibility = Visibility.Hidden;
                        TableTitle.Text = "Không có dữ liệu hiện tại";
                        LeftColumnContent.Text = "Lỗi: Không thể tải dữ liệu từ API.";
                        RightColumnContent.Text = "";
                        CurrentTimeTextBlock.Text = "Thời gian: Đang cập nhật";
                    }
                }
                else
                {
                    MessageBox.Show("Không thể xác định ID quận", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi cập nhật bảng: {ex.Message}\n\nChi tiết: {ex.StackTrace}",
                    "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Phương thức hỗ trợ để định dạng DateTime
        private string FormatDateTime(DateTime dateTime)
        {
            return dateTime.ToString("dd/MM/yyyy HH:mm");
        }


        private async Task<List<EnvironmentalData>> FetchHistoricalDataAsync(int chartDistrictId)
        {
            string apiUrl = $"{_apiBaseUrl}api/EnvironmentalData/district/id/{chartDistrictId}";
            try
            {
                using (var client = new HttpClient())
                {
                    var response = await client.GetAsync(apiUrl);
                    if (response.IsSuccessStatusCode)
                    {
                        var jsonData = await response.Content.ReadAsStringAsync();

                        // Directly deserialize the JSON array
                        return JsonConvert.DeserializeObject<List<EnvironmentalData>>(jsonData);
                    }
                    else
                    {
                        MessageBox.Show($"Không thể tải dữ liệu từ API. Mã trạng thái: {response.StatusCode}");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi gọi API: {ex.Message}\n\nChi tiết: {ex.StackTrace}");
            }
            return null;
        }

        private double GetMetricValue(EnvironmentalData data, string metricName)
        {
            switch (metricName)
            {
                case "temp": return data.Temp;
                case "feelsLike": return data.FeelsLike;
                case "tempMin": return data.TempMin;
                case "tempMax": return data.TempMax;
                case "pressure": return data.Pressure;
                case "humidity": return data.Humidity;
                case "aqi": return data.AQI;
                case "co": return data.CO;
                case "no": return data.NO;
                case "no2": return data.NO2;
                case "o3": return data.O3;
                case "so2": return data.SO2;
                case "pm2_5": return data.PM2_5;
                case "pm10": return data.PM10;
                case "nh3": return data.NH3;
                default:
                    throw new ArgumentException($"Metric {metricName} not recognized");
            }
        }

        private async Task UpdateMonthlyChart()
        {
            try
            {
                var selectedChartDistrict = DistrictForChartSelector.SelectedItem as ComboBoxItem;
                var selectedMetric = ComparisonType.SelectedItem as ComboBoxItem;
                var yearSelector = YearSelector.SelectedItem as ComboBoxItem;

                if (selectedChartDistrict == null || selectedMetric == null || yearSelector == null)
                {
                    MonthlyChart.Visibility = Visibility.Hidden;
                    ChartTitle.Text = "Vui lòng chọn chỉ số và quận";
                    return;
                }

                int chartDistrictId;
                if (selectedChartDistrict.Tag != null && int.TryParse(selectedChartDistrict.Tag.ToString(), out chartDistrictId))
                {
                    string metricName = selectedMetric.Content.ToString();
                    int selectedYear = int.Parse(yearSelector.Content.ToString());

                    // Lấy toàn bộ dữ liệu cho quận được chọn
                    var allData = await FetchHistoricalDataAsync(chartDistrictId);

                    if (allData != null && allData.Count > 0)
                    {
                        // Xử lý dữ liệu nhận được
                        var monthlyData = new List<double>(12);
                        for (int month = 1; month <= 12; month++)
                        {
                            var monthlyValues = allData
                                .Where(d => d.DateTime.Year == selectedYear && d.DateTime.Month == month)
                                .Select(d => GetMetricValue(d, metricName))
                                .ToList();

                            double averageValue = monthlyValues.Count > 0
                                ? monthlyValues.Average()
                                : 0;

                            monthlyData.Add(averageValue);
                        }

                        MonthlyChart.Series[0].Values = new ChartValues<double>(monthlyData);
                        MonthlyChart.Visibility = Visibility.Visible;
                        ChartTitle.Text = $"Chỉ số {metricName} tại {selectedChartDistrict.Content} (Tháng 1 - Tháng 12 năm {selectedYear})";
                    }
                    else
                    {
                        MonthlyChart.Visibility = Visibility.Hidden;
                        ChartTitle.Text = "Không có dữ liệu cho chỉ số này hoặc lỗi khi gọi API";
                    }
                }
                else
                {
                    MessageBox.Show("Không thể xác định ID quận", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi cập nhật biểu đồ: {ex.Message}\n\nChi tiết: {ex.StackTrace}",
                    "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void DistrictForWeatherSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                await UpdateWeatherTable();
                await UpdateMonthlyChart();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi xử lý sự kiện chọn quận: {ex.Message}\n\nChi tiết: {ex.StackTrace}",
                    "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void DistrictForChartSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                await UpdateMonthlyChart();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi cập nhật biểu đồ: {ex.Message}\n\nChi tiết: {ex.StackTrace}",
                    "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void ComparisonType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                await UpdateMonthlyChart();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi cập nhật biểu đồ theo chỉ số: {ex.Message}\n\nChi tiết: {ex.StackTrace}",
                    "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void YearSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                await UpdateMonthlyChart();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi cập nhật biểu đồ theo năm: {ex.Message}\n\nChi tiết: {ex.StackTrace}",
                    "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private long GetUnixTimestamp(int year, int month, int day)
        {
            DateTime date = new DateTime(year, month, day);
            return (long)(date - new DateTime(1970, 1, 1)).TotalSeconds;
        }
    }
}