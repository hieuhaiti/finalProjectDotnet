using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Server.Services
{
    public class ScheduledDataCrawlService : IHostedService, IDisposable
    {
        private Timer? _timer;
        private readonly IServiceProvider _serviceProvider;

        public ScheduledDataCrawlService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("Starting scheduled data crawl service...");
            using (var scope = _serviceProvider.CreateScope())
            {
                var dataCrawlService = scope.ServiceProvider.GetRequiredService<CrawDataService>();
                await dataCrawlService.CrawlDataAsync();
            }

            ScheduleNextRun();
        }

        private void ScheduleNextRun()
        {
            var utcNow = DateTime.UtcNow;
            var timeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"); // UTC+7
            var now = TimeZoneInfo.ConvertTimeFromUtc(utcNow, timeZone);

            var nextFullHour = now.AddHours(1).Date.AddHours(now.Hour + 1);
            var delayToNextRun = (nextFullHour - now).TotalMilliseconds;

            Console.WriteLine($"Next run scheduled at {nextFullHour} (in {delayToNextRun / 1000} seconds)");

            _timer = new Timer(async _ =>
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var dataCrawlService = scope.ServiceProvider.GetRequiredService<CrawDataService>();
                    await dataCrawlService.CrawlDataAsync();
                }

                ScheduleNextRun(); // Schedule the next run
            }, null, (int)delayToNextRun, Timeout.Infinite);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("Stopping scheduled data crawl service...");
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}