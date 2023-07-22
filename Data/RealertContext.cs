using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Realert.Models;

namespace Realert.Data
{
    public class RealertContext : DbContext
    {
        public RealertContext (DbContextOptions<RealertContext> options)
            : base(options)
        {
        }

        public DbSet<Realert.Models.PriceAlertNotification> PriceAlertNotification { get; set; } = default!;

        public DbSet<Realert.Models.NewPropertyAlertNotification> NewPropertyAlertNotification { get; set; } = default!;
    }
}
