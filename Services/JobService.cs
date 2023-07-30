using Microsoft.EntityFrameworkCore;
using Realert.Data;
using Realert.Interfaces;

namespace Realert.Services
{
    public sealed class JobService : IHostedService, IDisposable
    {
        // Define Job Ids for the jobs run by this service.
        private const int PriceAlertJobId = 1;
        private const int NewPropertyAlertJobId = 2;

        // Fields.
        private readonly ILogger<JobService> logger;
        private readonly IServiceScopeFactory scopeFactory;
        private Timer? timer = null;

        public JobService(ILogger<JobService> logger, IServiceScopeFactory scopeFactory)
        {
            this.logger = logger;
            this.scopeFactory = scopeFactory;
        }

        /// <inheritdoc/>
        public Task StartAsync(CancellationToken stoppingToken)
        {
            this.logger.LogInformation("Job Service started.");

            this.timer = new Timer(this.RunJobs, null, TimeSpan.Zero, TimeSpan.FromHours(1));

            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public Task StopAsync(CancellationToken stoppingToken)
        {
            this.logger.LogInformation("Job Service stopped.");

            this.timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            this.timer?.Dispose();
        }

        /// <summary>
        /// Method runs the Price Alert and New Property Alert jobs once per day.
        /// </summary>
        /// <param name="state">Unused.</param>
        private async void RunJobs(object? state)
        {
            using var scope = this.scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetService<RealertContext>();
            if (dbContext == null)
            {
                return;
            }

            // Price Alert Job.
            var job = await dbContext.Job.FirstOrDefaultAsync(j => j.Id == PriceAlertJobId);
            if (job != null)
            {
                var hoursSinceLastRun = (DateTime.Now - job.LastRun).TotalHours;

                // Run job if its:
                //  - Set as active, and
                //  - Not currently running, and
                //  - At least 24 hours since the last run.
                if (job.IsActive && !job.IsRunning && hoursSinceLastRun >= 24)
                {
                    this.logger.LogInformation("Price Alert job started.");

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

                    this.logger.LogInformation("Price Alert job finished.");
                }
            }
            else
            {
                // Job is not setup correctly, log warning.
                this.logger.LogWarning("Price Alert job is not setup.");
            }

            // New Property Alert Job.
            job = await dbContext.Job.FirstOrDefaultAsync(j => j.Id == NewPropertyAlertJobId);
            if (job != null)
            {
                var hoursSinceLastRun = (DateTime.Now - job.LastRun).TotalHours;

                // Run job if its:
                //  - Set as active, and
                //  - Not currently running, and
                //  - At least 24 hours since the last run.
                if (job.IsActive && !job.IsRunning && hoursSinceLastRun >= 24)
                {
                    this.logger.LogInformation("New Property Alert job started.");

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

                    this.logger.LogInformation("New Property Alert job finished.");
                }
            }
            else
            {
                // Job is not setup correctly, log warning.
                this.logger.LogWarning("New Property Alert job is not setup.");
            }
        }
    }
}