namespace Day4UserConsole.Models;

public class UserSummary
{
    public int TotalCount { get; init; }
    public double AverageAge { get; init; }
    public int MinAge { get; init; }
    public int MaxAge { get; init; }
    public List<string> EmailDomainList { get; init; } = new();
}