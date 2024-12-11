using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Server.Data;
using Server.Models;

namespace Server.Services
{
    public class InitDataService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly AppDbContext _dbContext;


        public InitDataService(IConfiguration configuration, AppDbContext dbContext)
        {
            _httpClient = new HttpClient();
            _apiKey = configuration["OpenWeatherMap:ApiKey"] ?? throw new ArgumentNullException(nameof(configuration), "API key cannot be null");
            _dbContext = dbContext;
        }

        public async Task InitializeDataIfEmptyAsync()
        {
            if (!_dbContext.Coordinates.Any())
            {
                AddDistrictCoordinates();
            }
            if (!_dbContext.EnvironmentalDataEntries.Any())
            {
                await AddEnvironmentalDataForOneYear();
            }

        }

        private void AddDistrictCoordinates()
        {
            var districts = new List<Coordinate>
            {
                new Coordinate { lon = 105.8125, lat = 21.0333, district = "BaDinh", description = "Ba Đình"},
                new Coordinate { lon = 105.8542, lat = 21.0285, district = "HoanKiem", description = "Hoàn Kiếm" },
                new Coordinate { lon = 105.8172, lat = 21.0685, district = "TayHo", description = "Tây Hồ" },
                new Coordinate { lon = 105.8950, lat = 21.0401, district = "LongBien", description = "Long Biên" },
                new Coordinate { lon = 105.7912, lat = 21.0328, district = "CauGiay", description = "Cầu Giấy" },
                new Coordinate { lon = 105.8290, lat = 21.0181, district = "DongDa", description = "Đống Đa" },
                new Coordinate { lon = 105.8483, lat = 21.0064, district = "HaiBaTrung", description = "Hai Bà Trưng" },
                new Coordinate { lon = 105.8515, lat = 20.9754, district = "HoangMai", description = "Hoàng Mai" },
                new Coordinate { lon = 105.8019, lat = 20.9931, district = "ThanhXuan", description = "Thanh Xuân" },
                new Coordinate { lon = 105.7680, lat = 20.9590, district = "HaDong", description = "Hà Đông" },
                new Coordinate { lon = 105.7606, lat = 21.0134, district = "NamTuLiem", description = "Nam Từ Liêm" },
                new Coordinate { lon = 105.7446, lat = 21.0639, district = "BacTuLiem", description = "Bắc Từ Liêm"  }
            };
            _dbContext.Coordinates.AddRange(districts);
            _dbContext.SaveChanges();

        }

        private async Task AddEnvironmentalDataForOneYear()
        {
            var coordinates = _dbContext.Coordinates.ToList();
            var startDate = 1702486800;
            var endDate = 1733245200;

            foreach (var coordinate in coordinates)
            {
                var airPollutionData = await GetHistoryAirPollutionDataAsync(coordinate.lat, coordinate.lon, startDate, endDate);
                long interval = 7 * 24 * 60 * 60;
                long currentStart = startDate;

                var entries = new List<EnvironmentalDataEntry>();

                while (currentStart < endDate)
                {
                    long currentEnd = Math.Min(currentStart + interval, endDate);
                    var weatherData = await GetHistoryWeatherDataAsync(coordinate.lat, coordinate.lon, currentStart, currentEnd);

                    foreach (var weather in weatherData.list)
                    {
                        var airPollution = airPollutionData.list.FirstOrDefault(entry => entry.dt == weather.dt);

                        entries.Add(new EnvironmentalDataEntry
                        {
                            coordinateId = coordinate.id,
                            dt = weather.dt,
                            dateTime = UnixTimeStampToDateTime(weather.dt),
                            temp = weather.main.temp,
                            feelsLike = weather.main.feels_like,
                            pressure = weather.main.pressure,
                            humidity = weather.main.humidity,
                            tempMin = weather.main.temp_min,
                            tempMax = weather.main.temp_max,
                            aqi = airPollution?.main.aqi ?? 0,
                            co = airPollution?.components.co ?? 0,
                            no = airPollution?.components.no ?? 0,
                            no2 = airPollution?.components.no2 ?? 0,
                            o3 = airPollution?.components.o3 ?? 0,
                            so2 = airPollution?.components.so2 ?? 0,
                            pm2_5 = airPollution?.components.pm2_5 ?? 0,
                            pm10 = airPollution?.components.pm10 ?? 0,
                            nh3 = airPollution?.components.nh3 ?? 0
                        });
                    }
                    currentStart = currentEnd + 3600;
                }
                _dbContext.EnvironmentalDataEntries.AddRange(entries);
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task<AirPollutionResponse> GetHistoryAirPollutionDataAsync(double lat, double lon, long start, long end)
        {
            var url = $"https://api.openweathermap.org/data/2.5/air_pollution/history?lat={lat}&lon={lon}&start={start}&end={end}&appid={_apiKey}";
            var response = await _httpClient.GetStringAsync(url);
            var airPollutionResponse = JsonSerializer.Deserialize<AirPollutionResponse>(response, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
            if (airPollutionResponse == null)
            {
                throw new InvalidOperationException("Deserialization of air pollution data failed.");
            }
            return airPollutionResponse;
        }

        public async Task<HistoricalWeatherResponse> GetHistoryWeatherDataAsync(double lat, double lon, long start, long end)
        {
            var url = $"https://history.openweathermap.org/data/2.5/history/city?lat={lat}&lon={lon}&type=hour&start={start}&end={end}&units=metric&appid={_apiKey}";
            var response = await _httpClient.GetStringAsync(url);
            var weatherResponse = JsonSerializer.Deserialize<HistoricalWeatherResponse>(response, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
            if (weatherResponse == null)
            {
                throw new InvalidOperationException("Deserialization of weather data failed.");
            }
            return weatherResponse;
        }
        public static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTime = dateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dateTime;
        }

    }


}
