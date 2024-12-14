using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.Models;
namespace Server.Services
{
    public class EnvironmentalDataService
    {
        private readonly AppDbContext _dbContext;
        private readonly InitDataService _initDataService;
        private readonly ConvertDt _convertDt;

        public EnvironmentalDataService(AppDbContext dbContext, InitDataService initDataService, ConvertDt convertDt)
        {
            _dbContext = dbContext;
            _initDataService = initDataService;
            _convertDt = convertDt;
        }

        public async Task<List<EnvironmentalDataEntry>> GetCurrentDataAsync()
        {
            var allData = await _dbContext.EnvironmentalDataEntries
                .OrderByDescending(e => e.dt)
                .ToListAsync();

            var latestData = allData
                .GroupBy(e => e.coordinateId)
                .Select(g => g.First())
                .OrderBy(e => e.coordinateId)
                .ToList();

            return latestData;

        }

        public async Task<EnvironmentalDataEntry> GetByIdAsync(Guid id)
        {
            var entry = await _dbContext.EnvironmentalDataEntries.FindAsync(id);
            if (entry == null) throw new KeyNotFoundException($"EnvironmentalDataEntry with ID {id} not found.");
            return entry;
        }

        public async Task<List<EnvironmentalDataEntry>> GetAllAsync()
        {
            return await _dbContext.EnvironmentalDataEntries.ToListAsync();
        }

        public async Task<List<EnvironmentalDataEntry>> GetAllByDistrictIdAsync(int districtId)
        {
            return await _dbContext.EnvironmentalDataEntries
                .Where(e => e.coordinateId == districtId)
                .ToListAsync();
        }

        public async Task<List<EnvironmentalDataEntry>> GetAllByDistrictNameAsync(string districtName)
        {
            Coordinate Coordinate = await _dbContext.Coordinates.Where(e => e.district == districtName).FirstOrDefaultAsync() ?? throw new KeyNotFoundException($"Coordinate with district name {districtName} not found.");
            int idDistric = Coordinate.id;
            return await GetAllByDistrictIdAsync(idDistric);
        }


        public async Task<List<EnvironmentalDataEntry>> GetAllByDateAsync(long date)
        {
            var dt = _convertDt.UnixTimeStampToDateTime(date);
            var roundedDt = new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, 0, 0);

            return await _dbContext.EnvironmentalDataEntries
                .Where(e => e.dateTime.Year == roundedDt.Year &&
                            e.dateTime.Month == roundedDt.Month &&
                            e.dateTime.Day == roundedDt.Day &&
                            e.dateTime.Hour == roundedDt.Hour)
                .ToListAsync();
        }

        public async Task<List<EnvironmentalDataEntry>> GetByDateRangeAsync(long startDate, long endDate)
        {
            return await _dbContext.EnvironmentalDataEntries
                .Where(e => e.dt >= startDate && e.dt <= endDate)
                .ToListAsync();
        }


        public async Task<List<EnvironmentalDataEntry>> GetAllByDistrictIdAndDate(int districtId, long startDate, long endDate)
        {
            return await _dbContext.EnvironmentalDataEntries
                .Where(e => e.coordinateId == districtId && e.dt >= startDate && e.dt <= endDate)
                .ToListAsync();
        }

        public async Task<List<EnvironmentalDataEntry>> GetAllByDistrictNameAndDate(string districtName, long startDate, long endDate)
        {
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
            Coordinate Coordinate = await _dbContext.Coordinates.Where(e => e.district == districtName).FirstOrDefaultAsync();
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            int idDistric = Coordinate.id;
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            return await GetAllByDistrictIdAndDate(idDistric, startDate, endDate);
        }

        public async Task AddByDateToDate(int districtId, long startDate, long endDate)
        {
            await _initDataService.AddEnvironmentalDataFor1District(districtId, startDate, endDate);
        }
        public async Task AddByDateToNow(int districtId, long startDate)
        {

            var roundedDt = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, 0, 0);
            long endDate = _convertDt.DateTimeToUnixTimeStamp(roundedDt);
            await _initDataService.AddEnvironmentalDataFor1District(districtId, startDate, endDate);
        }

        public async Task<EnvironmentalDataEntry> AddCurrentDataAsync(EnvironmentalDataEntry entry)
        {
            var existingEntry = await _dbContext.EnvironmentalDataEntries
                .FirstOrDefaultAsync(e => e.coordinateId == entry.coordinateId && e.dt == entry.dt);
            if (existingEntry != null) throw new InvalidOperationException("Entry already exists.");

            _dbContext.EnvironmentalDataEntries.Add(entry);
            await _dbContext.SaveChangesAsync();
            return entry;
        }

        public async Task<EnvironmentalDataEntry?> UpdateAsync(Guid id, EnvironmentalDataEntry updatedEntry)
        {
            var entry = await _dbContext.EnvironmentalDataEntries.FindAsync(id);
            if (entry == null) return null;

            entry.temp = updatedEntry.temp;
            entry.feelsLike = updatedEntry.feelsLike;
            entry.pressure = updatedEntry.pressure;
            entry.humidity = updatedEntry.humidity;
            entry.tempMin = updatedEntry.tempMin;
            entry.tempMax = updatedEntry.tempMax;
            entry.aqi = updatedEntry.aqi;
            entry.co = updatedEntry.co;
            entry.no = updatedEntry.no;
            entry.no2 = updatedEntry.no2;
            entry.o3 = updatedEntry.o3;
            entry.so2 = updatedEntry.so2;
            entry.pm2_5 = updatedEntry.pm2_5;
            entry.pm10 = updatedEntry.pm10;
            entry.nh3 = updatedEntry.nh3;

            await _dbContext.SaveChangesAsync();
            return entry;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var entry = await _dbContext.EnvironmentalDataEntries.FindAsync(id);
            if (entry == null) return false;

            _dbContext.EnvironmentalDataEntries.Remove(entry);
            await _dbContext.SaveChangesAsync();
            return true;
        }
    }
    public class CoordinatesService
    {
        private readonly AppDbContext _dbContext;

        public CoordinatesService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // Get all districts
        public async Task<List<Coordinate>> GetAllDistrictsAsync()
        {
            return await _dbContext.Coordinates.ToListAsync();
        }

        // Add new district
        public async Task<Coordinate> AddDistrictAsync(double lon, double lat, string district, string description)
        {
            Coordinate NewDistrict = new Coordinate
            {
                lon = lon,
                lat = lat,
                district = district,
                description = description
            };
            _dbContext.Coordinates.Add(NewDistrict);
            await _dbContext.SaveChangesAsync();
            return NewDistrict;
        }

        // Update district
        public async Task<Coordinate?> UpdateDistrictAsync(int id, Coordinate updatedDistrict)
        {
            var existingDistrict = await _dbContext.Coordinates.FindAsync(id);
            if (existingDistrict == null) return null;

            existingDistrict.lon = updatedDistrict.lon;
            existingDistrict.lat = updatedDistrict.lat;
            existingDistrict.district = updatedDistrict.district;
            existingDistrict.description = updatedDistrict.description;

            await _dbContext.SaveChangesAsync();
            return existingDistrict;
        }

        // Delete district
        public async Task<bool> DeleteDistrictAsync(int id)
        {
            // Tìm `District` trong bảng `Coordinates`
            var district = await _dbContext.Coordinates.FindAsync(id);
            if (district == null)
                return false;

            // Kiểm tra xem `District` có được tham chiếu bởi `EnvironmentalData` không
            bool isReferenced = await _dbContext.EnvironmentalDataEntries.AnyAsync(ed => ed.coordinateId == id);
            if (isReferenced)
            {
                // Không thể xóa vì có dữ liệu tham chiếu
                throw new InvalidOperationException("Cannot delete district because it is referenced by Environmental Data.");
            }

            // Xóa `District`
            _dbContext.Coordinates.Remove(district);
            await _dbContext.SaveChangesAsync();
            return true;
        }

    }
}