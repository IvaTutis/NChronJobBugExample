using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NCronJob;
using Newtonsoft.Json;

namespace NChronJobExample.Services;

 public interface IJobRegistrationService
    {
        Task UpsertJob(Guid jobId, string cronExpression);
        Task DeleteJob(Guid jobId);
        List<RecurringJobSchedule> GetAllRecurringJobs();
    }

    public class JobRegistrationService : IJobRegistrationService
    {
        private readonly IRuntimeJobRegistry _jobRegistry;

        public JobRegistrationService(IRuntimeJobRegistry jobRegistry)
        {
            _jobRegistry = jobRegistry;
        }

        public Task UpsertJob(Guid jobId, string cronExpression)
        {
            DeleteJob(jobId);
            
            _jobRegistry.AddJob(n =>
            {
                n.AddJob<Job>(j =>
                {
                    j.WithCronExpression(cronExpression, TimeZoneInfo.Local)
                        .WithParameter(new JobParameters { JobId = jobId })
                        .WithName(jobId.ToString());
                });
            });

            return Task.CompletedTask;
        }

        public Task DeleteJob(Guid jobId)
        {

            var allJobs = _jobRegistry.GetAllRecurringJobs();
            var jobToRemove = allJobs.FirstOrDefault(job => job.JobName == jobId.ToString());

            if (jobToRemove != null)
            {
                _jobRegistry.RemoveJob(jobToRemove.JobName);
            }/*
            else
            {
                throw new Exception("No job found with ID: {jobId}");
            }*/

            return Task.CompletedTask;
        }

        public List<RecurringJobSchedule> GetAllRecurringJobs()
        {
            return _jobRegistry.GetAllRecurringJobs().ToList();
        }
    }

    public class JobParameters
    {
        public Guid JobId { get; set; }
    }

    public class Job : IJob
    {
        private readonly MockRepository _mockRepository;

        public Job(MockRepository mockRepository)
        {
            _mockRepository = mockRepository;
        }

        public Task RunAsync(IJobExecutionContext context, CancellationToken token)
        {
            var jobParameters = context.Parameter as JobParameters ??
                                throw new InvalidOperationException(
                                    $"Cannot parse the job context parameters to {nameof(JobParameters)} for job with parameters {JsonConvert.SerializeObject(context.Parameter)}!");

            // Store the execution time in the MockRepository
            _mockRepository.UpdateLastJobExecution(jobParameters.JobId, DateTime.Now);

            // Implement your job execution logic here
            Console.WriteLine($"Job {jobParameters.JobId} executed at {DateTime.Now}");

            return Task.CompletedTask;
        }
    }