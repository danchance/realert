using System.ComponentModel.DataAnnotations;

namespace Realert.Models
{
    /// <summary>
    /// Model <see cref="Job"/> holds details of all scheduled jobs/tasks.
    public class Job
    {
        /// <value>
        /// Unique Id.
        /// </value>
        public int Id { get; set; }

        /// <value>
        /// Name/Description of the job.
        /// </value>
        public string? Name { get; set; }

        /// <value>
        /// Determines if the job should be run.
        /// </value>
        public bool IsActive { get; set; }

        /// <value>
        /// Determines if the job is currently running.
        /// </value>
        public bool IsRunning { get; set; }

        /// <value>
        /// Date and time the job was last run.
        /// </value>
        public DateTime LastRun { get; set; }
    }
}
