namespace Day4UserConsole.Repositories;

public class InMemoryUserRepository : IUserRepository
{
    private readonly List<Models.User> _users = new()
    {
        new Models.User { Id = 1, Name = "佐藤 太郎", Email = "sato@example.com", Age = 28 },
        new Models.User { Id = 2, Name = "鈴木 一郎", Email = "suzuki@example.com", Age = 35 }
    };

    public List<Models.User> GetAll()
    {
        return _users.ToList();
    }

    public Models.User? GetById(int id)
    {
        return _users.FirstOrDefault(u => u.Id == id);
    }

    public List<Models.User> SearchByName(string keyword)
    {
        return _users
            .Where(u => u.Name.Contains(keyword, StringComparison.OrdinalIgnoreCase))
            .ToList();
    }

    public void Add(Models.User user)
    {
        int newId = _users.Any() ? _users.Max(u => u.Id) + 1 : 1;
        var newUser = new Models.User
        {
            Id = newId,
            Name = user.Name,
            Email = user.Email,
            Age = user.Age
        };
        _users.Add(newUser);
    }

    public bool Delete(int id)
    {
        var target = GetById(id);
        if (target == null)
        {
            return false;
        }

        _users.Remove(target);
        return true;
    }
}