namespace Day4UserConsole.Models;

public class User
{
    public int Id { get; init; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int Age { get; set; }
    public int DepartmentId { get; set; } 
}