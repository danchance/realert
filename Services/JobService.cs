using Microsoft.EntityFrameworkCore;
using Realert.Data;

namespace Realert.Services
{
    public class JobService : IHostedService, IDisposable
    {
        private readonly ILogger<JobService> _logger;
        private readonly IServiceScopeFactory _scopeFactory;
        private Timer? _timer = null;

        /*
         * Define Job Ids for the jobs run by this service.
         */
        private const int _priceAlertJobId = 1;
        private const int _newPropertyAlertJobId = 2;

        public JobService(ILogger<JobService> logger, IServiceScopeFactory scopeFactory) 
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
        }

        /*
         * Start timer to run jobs every hour.
         */
        public Task StartAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Job Service started.");

            _timer = new Timer(RunJobs, null, TimeSpan.Zero, TimeSpan.FromHours(1));

            return Task.CompletedTask;  
        }

        /*
         * Disable the timer.
         */
        public Task StopAsync(CancellationToken stoppingToken) 
        {
            _logger.LogInformation("Job Service stopped.");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        /*
         * Dispose of the timer.
         */
        public void Dispose() 
        {
            _timer?.Dispose();
        }

        /*
         * Runs active jobs setup in the Job table. Current supported jobs are:
         *  - Price Alert (runs once a day).
         *  - New Property Alert (runs once a day).
         */
        private async void RunJobs(object? state)
        {
            using var scope = _scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetService<RealertContext>();
            if (dbContext == null)
            {
                return;
            }

            // Price Alert Job.
            var job = await dbContext.Job.FirstOrDefaultAsync(j => j.Id == _priceAlertJobId);
            if (job != null)
            {
                var hoursSinceLastRun = (DateTime.Now - job.LastRun).TotalHours;
                // Run job if its:
                //  - Set as active, and
                //  - Not currently running, and
                //  - At least 24 hours since the last run.
                if (job.IsActive && !job.IsRunning && hoursSinceLastRun >= 24)
                {
                    _logger.LogInformation("Price Alert job started.");

                    // Set job status to currently running.
                    job.IsRunning = true;
                    dbContext.Job.Update(job);
                    await dbContext.SaveChangesAsync();

                    var priceAlertService = scope.ServiceProvider.GetService<IPriceAlertService>();
                    if (priceAlertService != null)
                    {
                        await priceAlertService.PerformScanAsync();
                    }

                    // Update Price Alert job info.
                    job.IsRunning = false;
                    job.LastRun = DateTime.Now;
                    dbContext.Job.Update(job);
                    await dbContext.SaveChangesAsync();

                    _logger.LogInformation("Price Alert job finished.");
                }
            }
            else
            {
                // Job is not setup correctly, log warning.
                _logger.LogWarning("Price Alert job is not setup.");
            }

            // New Property Alert Job.
            job = await dbContext.Job.FirstOrDefaultAsync(j => j.Id == _newPropertyAlertJobId);
            if (job != null)
            {
                var hoursSinceLastRun = (DateTime.Now - job.LastRun).TotalHours;
                // Run job if its:
                //  - Set as active, and
                //  - Not currently running, and
                //  - At least 24 hours since the last run.
                if (job.IsActive && !job.IsRunning && hoursSinceLastRun >= 24)
                {
                    _logger.LogInformation("New Property Alert job started.");

                    // Set job status to currently running.
                    job.IsRunning = true;
                    dbContext.Job.Update(job);
                    await dbContext.SaveChangesAsync();

                    var newPropertyAlertService = scope.ServiceProvider.GetService<INewPropertyAlertService>();
                    if (newPropertyAlertService != null)
                    {
                        await newPropertyAlertService.PerformScanAsync();
                    }

                    // Update Price Alert job info.
                    job.IsRunning = false;
                    job.LastRun = DateTime.Now;
                    dbContext.Job.Update(job);
                    await dbContext.SaveChangesAsync();

                    _logger.LogInformation("New Property Alert job finished.");
                }
            }
            else
            {
                // Job is not setup correctly, log warning.
                _logger.LogWarning("New Property Alert job is not setup.");
            }
        }
    }
}
