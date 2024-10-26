namespace NChronJobExample;

public class MockRepository
{
    public List<Tuple<Guid, string>> RuntimeStartupJobList = new List<Tuple<Guid, string>>()
    {
        //new Tuple<Guid, string>(Guid.NewGuid(), "* * * * *"), //TODO upon uncommenting it will work as described in docs
        new Tuple<Guid, string>(Guid.NewGuid(), "0 12 * * *"),
        new Tuple<Guid, string>(Guid.NewGuid(), "0 17 * * *"),
        new Tuple<Guid, string>(Guid.NewGuid(), "0 18 * * *"),
    };
    
    public List<Tuple<Guid, string>> RuntimeJobList = new List<Tuple<Guid, string>>()
    {
        new Tuple<Guid, string>(Guid.NewGuid(), "0 18 * * *"),
        new Tuple<Guid, string>(Guid.NewGuid(), "0 19 * * *"),
        new Tuple<Guid, string>(Guid.NewGuid(), "0 19 * * *"),
    };

    public Dictionary<Guid, DateTime> LastJobExecutionDictionary = new();
    
    public void UpdateLastJobExecution(Guid jobId, DateTime executionTime)
    {
        LastJobExecutionDictionary[jobId] = executionTime;
    }
}