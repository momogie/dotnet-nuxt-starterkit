namespace Shared;
public class HangfireQueueConfig
{
    public string Name { get; set; } = "";
    public int WorkerCount { get; set; }
}

public class HangfireSettings
{
    public List<HangfireQueueConfig> Queues { get; set; } = [];
}
