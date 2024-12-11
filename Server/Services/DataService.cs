using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.Models;

public class EnvironmentalDataService
{
    private readonly AppDbContext _dbContext;

    public EnvironmentalDataService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<EnvironmentalDataEntry>> GetCurrentDataAsync()
    {
        return await _dbContext.EnvironmentalDataEntries
            .Where(e => e.coordinateId >= 1 && e.coordinateId <= 12)
            .OrderByDescending(e => e.dateTime)
            .ToListAsync();
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
        var dt = DateTimeOffset.FromUnixTimeMilliseconds(date).DateTime.Date;
        return await _dbContext.EnvironmentalDataEntries
            .Where(e => e.dateTime.Date == dt)
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
        Coordinate Coordinate = await _dbContext.Coordinates.Where(e => e.district == districtName).FirstOrDefaultAsync();
        int idDistric = Coordinate.id;
        return await GetAllByDistrictIdAndDate(idDistric, startDate, endDate);
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

    public async Task<EnvironmentalDataEntry> UpdateAsync(Guid id, EnvironmentalDataEntry updatedEntry)
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