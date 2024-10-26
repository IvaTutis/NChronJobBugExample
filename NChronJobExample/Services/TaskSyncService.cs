namespace NChronJobExample.Services;

public class TaskSyncService : IHostedService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly MockRepository _repository;


    public TaskSyncService(IServiceProvider serviceProvider, MockRepository repository)
    {
        _serviceProvider = serviceProvider;
        _repository = repository;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var jobRegistrationService = scope.ServiceProvider.GetRequiredService<IJobRegistrationService>();

        var jobs = _repository.RuntimeStartupJobList;

        foreach (var job in jobs)
        {
            try
            {
                await jobRegistrationService.UpsertJob(job.Item1, job.Item2);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Failed starting NChronJob for schedule {job.Item2} with id {job.Item1}.");
            }
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}