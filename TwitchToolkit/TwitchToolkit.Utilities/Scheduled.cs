/*
 * File: Scheduled.cs
 * Project: TwitchToolkit
 * 
 * Updated: September 13, 2025
 * 
 * Summary of Changes:
 * 1. Added thread safety with locking for all operations
 * 2. Added null checking for job parameters
 * 3. Improved efficiency by reducing list operations
 * 4. Added exception handling for job execution
 * 5. Added documentation comments
 */

using System;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace TwitchToolkit.Utilities
{
    /// <summary>
    /// Manages scheduled jobs with thread-safe operations
    /// </summary>
    public class Scheduled
    {
        private readonly List<ScheduledJob> jobs;
        private readonly object lockObject = new object();

        public Scheduled()
        {
            jobs = new List<ScheduledJob>();
        }

        /// <summary>
        /// Checks all jobs and executes those that are due
        /// </summary>
        public void CheckAllJobs()
        {
            List<ScheduledJob> jobsToRun = new List<ScheduledJob>();
            List<ScheduledJob> jobsToKeep = new List<ScheduledJob>();

            lock (lockObject)
            {
                // Decrement all jobs that aren't due yet
                foreach (ScheduledJob job in jobs.Where(j => j.MinutesTillExpire > 0))
                {
                    job.Decrement();
                    jobsToKeep.Add(job);
                }

                // Collect jobs that are due to run
                jobsToRun.AddRange(jobs.Where(j => j.MinutesTillExpire == 0));
            }

            // Run jobs outside the lock to minimize locking time
            foreach (ScheduledJob job in jobsToRun)
            {
                try
                {
                    job.RunJob();
                }
                catch (Exception ex)
                {
                    Log.Error($"[TwitchToolkit] Error executing scheduled job: {ex}");
                }
            }

            // Update the jobs list (remove completed jobs)
            lock (lockObject)
            {
                jobs.Clear();
                jobs.AddRange(jobsToKeep);
            }
        }

        /// <summary>
        /// Adds a new job to the schedule
        /// </summary>
        /// <param name="job">The job to add</param>
        /// <exception cref="ArgumentNullException">Thrown if job is null</exception>
        public void AddNewJob(ScheduledJob job)
        {
            if (job == null)
                throw new ArgumentNullException(nameof(job));

            lock (lockObject)
            {
                jobs.Add(job);
            }
        }

        /// <summary>
        /// Gets the number of pending jobs
        /// </summary>
        public int JobCount
        {
            get
            {
                lock (lockObject)
                {
                    return jobs.Count;
                }
            }
        }
    }
}
/**
 * Scheduled.cs
using System.Collections.Generic;
using System.Linq;

namespace TwitchToolkit.Utilities;

public class Scheduled
{
	private List<ScheduledJob> jobs;

	public Scheduled()
	{
		jobs = new List<ScheduledJob>();
	}

	public void CheckAllJobs()
	{
		List<ScheduledJob> jobstodecrement = jobs.Where((ScheduledJob k) => k.MinutesTillExpire > 0).ToList();
		foreach (ScheduledJob job in jobstodecrement)
		{
			job.Decrement();
		}
		List<ScheduledJob> jobstorun = jobs.Where((ScheduledJob k) => k.MinutesTillExpire == 0).ToList();
		foreach (ScheduledJob job2 in jobstorun)
		{
			job2.RunJob();
			jobs = jobs.Where((ScheduledJob l) => l != job2).ToList();
		}
	}

	public void AddNewJob(ScheduledJob job)
	{
		jobs.Add(job);
	}
**/