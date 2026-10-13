namespace Day4UserConsole.Models;

public class UserDetails
{
    public int Id { get; init; }
    public string Name { get; set; } = string.Empty;
    public int Age { get; set; }
    public string Email { get; set; } = string.Empty;
    public string DepartmentName { get; set; } = string.Empty; // 部署名が追加されている
}