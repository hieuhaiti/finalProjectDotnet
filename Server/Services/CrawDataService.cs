using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.Models;

namespace Server.Services
{
    public class CrawDataService
    {
        private readonly AppDbContext _dbContext;
        private readonly ConvertDt _convertDt;
        private readonly InitDataService _initDataService;

        public CrawDataService(AppDbContext dbContext, ConvertDt convertDt, InitDataService initDataService)
        {
            _dbContext = dbContext;
            _convertDt = convertDt;
            _initDataService = initDataService;


        }
        public async Task CrawlDataAsync()
        {
            try
            {
                var latestEntry = await _dbContext.EnvironmentalDataEntries
                               .OrderByDescending(e => e.dt)
                               .FirstOrDefaultAsync();
                long latestTime = latestEntry != null ? latestEntry.dt : 0;
                long currentTime = _convertDt.DateTimeToUnixTimeStamp(DateTime.UtcNow);

                var coordinates = await _dbContext.Coordinates.ToListAsync();
                foreach (var coordinate in coordinates)
                {
                    await _initDataService.AddEnvironmentalDataFor1District(coordinate.id, latestTime, currentTime);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during data crawling: {ex.Message}");
            }
        }
    }

}
