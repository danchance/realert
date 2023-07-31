using Microsoft.EntityFrameworkCore;
using Realert.Models;

namespace Realert.Data
{
    public class RealertContext : DbContext
    {
        public RealertContext(DbContextOptions<RealertContext> options)
            : base(options)
        {
        }

        public DbSet<PriceAlertNotification> PriceAlertNotification { get; set; } = default!;

        public DbSet<NewPropertyAlertNotification> NewPropertyAlertNotification { get; set; } = default!;

        public DbSet<Job> Job { get; set; } = default!;
    }
}
