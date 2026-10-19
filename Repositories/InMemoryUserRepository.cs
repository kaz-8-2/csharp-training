using System.ComponentModel;
using System.Text.Json;
using UserManagementApp.Models;

namespace UserManagementApp.Repositories;

public class UserRepository : IUserRepository
{
    private readonly string _filePath = "users.json";

    private readonly List<Department> _departments = new()
    {
        new Department { Id = 1, Name = "営業部" },
        new Department { Id = 2, Name = "開発部" },
        new Department { Id = 3, Name = "総務部" }
    };

    private async Task<List<User>> LoadUsersAsync(CancellationToken cancellationToken = default)
    {
        if (!File.Exists(_filePath)) return new List<User>();

        string json = await File.ReadAllTextAsync(_filePath, cancellationToken);
        if (string.IsNullOrWhiteSpace(json)) return new List<User>();

        return JsonSerializer.Deserialize<List<User>>(json) ?? new List<User>();
    }

    private async Task SaveUsersAsync(List<User> users, CancellationToken cancellationToken = default)
    {
        var options = new JsonSerializerOptions { WriteIndented = true };
        string json = JsonSerializer.Serialize(users, options);
        await File.WriteAllTextAsync(_filePath, json, cancellationToken);
    }

    public async Task<List<UserDetails>> GetAllSortedAsync(CancellationToken cancellationToken = default)
    {
        var users = await LoadUsersAsync(cancellationToken);
        return users
            .Join(
                _departments,
                u => u.DepartmentId,
                d => d.Id,
                (u, d) => new UserDetails
                {
                    Id = u.Id, Name = u.Name, Age = u.Age, Email = u.Email, DepartmentName = d.Name
                }
            )
            .OrderByDescending(x => x.Age)
            .ThenBy(x => x.Name)
            .ToList();
    }

    public async Task<User?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var users = await LoadUsersAsync(cancellationToken);
        return users.FirstOrDefault(u => u.Id == id);
    }

    public async Task<List<User>> SearchByNameAsync(string keyword, CancellationToken cancellationToken = default)
    {
        var users = await LoadUsersAsync(cancellationToken);
        return users
            .Where(u => u.Name.Contains(keyword, StringComparison.OrdinalIgnoreCase))
            .ToList();
    }

    public async Task AddAsync(User user, CancellationToken cancellationToken = default)
    {
        var users = await LoadUsersAsync(cancellationToken);

        if (users.Any(u => u.Email == user.Email))
        {
            throw new ArgumentException($"メールアドレス '{user.Email}' は既に登録されています。");
        }

        int newId = users.Any() ? users.Max(u => u.Id) + 1 : 1;

        var newUser = new User
        {
            Id = newId,
            Name = user.Name,
            Email = user.Email,
            Age = user.Age,
            DepartmentId = user.DepartmentId
        };

        users.Add(newUser);
        await SaveUsersAsync(users, cancellationToken);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var users = await LoadUsersAsync(cancellationToken);
        var target = users.FirstOrDefault(u => u.Id == id);
        
        if (target != null)
        {
            users.Remove(target);
            await SaveUsersAsync(users, cancellationToken);
            return true;
        }
        return false;
    }

    public async Task<UserSummary> GetSummaryAsync(CancellationToken cancellationToken = default)
    {
        var users = await LoadUsersAsync(cancellationToken);
        if (!users.Any()) return new UserSummary();

        var ageGroups = users
            .GroupBy(u => (u.Age / 10) * 10)
            .OrderBy(g => g.Key)
            .ToDictionary(g => $"{g.Key}代", g => g.Count());

        return new UserSummary
        {
            TotalCount = users.Count,
            AverageAge = Math.Round(users.Average(u => u.Age), 1),
            MinAge = users.Min(u => u.Age),
            MaxAge = users.Max(u => u.Age),
            EmailDomainList = users.Select(u => u.Email.Split('@').Last()).Distinct().ToList(),
            AgeGroupCounts = ageGroups
        };
    }
}