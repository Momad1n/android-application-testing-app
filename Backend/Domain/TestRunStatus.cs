namespace Backend.Domain;

public enum TestRunStatus
{
    Pending = 0,
    Running = 1,
    Passed = 2,
    Failed = 3,
    Timeout = 4
}