using UserManagementApp.Models;

namespace UserManagementApp.Repositories;

public class InMemoryUserRepository : IUserRepository
{
    private readonly Dictionary<int, User> _users = new();
    private readonly HashSet<string> _emails = new();
    
    // 部署のマスターデータ
    private readonly List<Department> _departments = new()
    {
        new Department { Id = 1, Name = "営業部" },
        new Department { Id = 2, Name = "開発部" },
        new Department { Id = 3, Name = "総務部" }
    };

    public InMemoryUserRepository()
    {
        // 初期データ（テスト用に追加）
        var user1 = new User { Id = 1, Name = "佐藤 太郎", Email = "sato@example.com", Age = 28, DepartmentId = 1 };
        var user2 = new User { Id = 2, Name = "鈴木 一郎", Email = "suzuki@example.com", Age = 35, DepartmentId = 2 };
        var user3 = new User { Id = 3, Name = "高橋 次郎", Email = "takahashi@example.com", Age = 28, DepartmentId = 2 };

        _users.Add(user1.Id, user1); _emails.Add(user1.Email);
        _users.Add(user2.Id, user2); _emails.Add(user2.Email);
        _users.Add(user3.Id, user3); _emails.Add(user3.Email);
    }

    public List<UserDetails> GetAllSorted()
    {
        // ★ Join と OrderBy/ThenBy による結合・並び替え
        return _users.Values
            .Join(
                _departments,           // 結合対象
                u => u.DepartmentId,    // Users側のキー
                d => d.Id,              // Departments側のキー
                (u, d) => new UserDetails // 結合結果から新しいオブジェクトを生成
                {
                    Id = u.Id,
                    Name = u.Name,
                    Age = u.Age,
                    Email = u.Email,
                    DepartmentName = d.Name // ここで結合成功！
                }
            )
            .OrderByDescending(x => x.Age) // 第一条件：年齢の降順（高い順）
            .ThenBy(x => x.Name)           // 第二条件：同い年なら名前の昇順
            .ToList();
    }

    public User? GetById(int id)
    {
        _users.TryGetValue(id, out var user);
        return user;
    }

    public List<User> SearchByName(string keyword)
    {
        return _users.Values
            .Where(u => u.Name.Contains(keyword, StringComparison.OrdinalIgnoreCase))
            .ToList();
    }

    public void Add(User user)
    {
        if (_emails.Contains(user.Email))
        {
            throw new ArgumentException($"メールアドレス '{user.Email}' は既に登録されています。");
        }

        int newId = _users.Any() ? _users.Keys.Max() + 1 : 1;
        var newUser = new User
        {
            Id = newId,
            Name = user.Name,
            Email = user.Email,
            Age = user.Age,
            DepartmentId = user.DepartmentId // 部署IDを追加
        };

        _users.Add(newId, newUser);
        _emails.Add(newUser.Email);
    }

    public bool Delete(int id)
    {
        if (_users.TryGetValue(id, out var target))
        {
            _users.Remove(id);
            _emails.Remove(target.Email);
            return true;
        }
        return false;
    }

    public UserSummary GetSummary()
    {
        var userList = _users.Values.ToList();
        if (!userList.Any()) return new UserSummary();

        // ★ GroupBy による年代別集計
        var ageGroups = userList
            .GroupBy(u => (u.Age / 10) * 10) // 例: 28→20, 35→30
            .OrderBy(g => g.Key)             // 年代順に並び替え
            .ToDictionary(
                g => $"{g.Key}代",           // Dictionaryのキー ("20代" など)
                g => g.Count()               // Dictionaryの値 (所属人数)
            );

        return new UserSummary
        {
            TotalCount = userList.Count,
            AverageAge = Math.Round(userList.Average(u => u.Age), 1),
            MinAge = userList.Min(u => u.Age),
            MaxAge = userList.Max(u => u.Age),
            EmailDomainList = userList.Select(u => u.Email.Split('@').Last()).Distinct().ToList(),
            AgeGroupCounts = ageGroups // 年代別辞書をセット
        };
    }
}