using System.ComponentModel.DataAnnotations;

namespace Realert.Models
{
    public class Job
    {
        public int Id { get; set; }

        /*
         * Job name.
         */
        public string? Name { get; set; }

        /*
         * If the job should be run.
         */
        public bool IsActive { get; set; }

        /*
         * If the job is currently running.
         */
        public bool IsRunning { get; set; }

        /*
         * The day the job was last run.
         */
        [DataType(DataType.Date)]
        public DateTime? LastRun { get; set; }
    }
}
