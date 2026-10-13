using Day4UserConsole.Models;

namespace Day4UserConsole.Repositories;

public class InMemoryUserRepository : IUserRepository
{
    // ① ユーザーデータを「IDをキーにしたDictionary」で管理
    private readonly Dictionary<int, User> _users = new();
    
    // ② 重複を許さないHashSetで「登録済みメールアドレス」を管理
    private readonly HashSet<string> _emails = new();

    public InMemoryUserRepository()
    {
        // 初期データをDictionaryとHashSetにセット
        var user1 = new User { Id = 1, Name = "佐藤 太郎", Email = "sato@example.com", Age = 28 };
        var user2 = new User { Id = 2, Name = "鈴木 一郎", Email = "suzuki@example.com", Age = 35 };

        _users.Add(user1.Id, user1);
        _emails.Add(user1.Email);

        _users.Add(user2.Id, user2);
        _emails.Add(user2.Email);
    }

    public List<User> GetAll()
    {
        // 辞書の「値（User）」だけを取り出してListに変換
        return _users.Values.ToList();
    }

    public User? GetById(int id)
    {
        // Dictionaryの機能を使って一瞬で検索（あれば返す、なければnull）
        _users.TryGetValue(id, out var user);
        return user;
    }

    public List<User> SearchByName(string keyword)
    {
        // LINQのWhereを使って名前で絞り込み
        return _users.Values
            .Where(u => u.Name.Contains(keyword, StringComparison.OrdinalIgnoreCase))
            .ToList();
    }

    public void Add(User user)
    {
        // HashSetの機能を使ってメールアドレスの重複をチェック
        if (_emails.Contains(user.Email))
        {
            // 重複していたら例外（エラー）を投げる
            throw new ArgumentException($"メールアドレス '{user.Email}' は既に登録されています。");
        }

        // LINQのMaxを使って現在の最大IDを取得し、+1する
        int newId = _users.Any() ? _users.Keys.Max() + 1 : 1;

        var newUser = new User
        {
            Id = newId,
            Name = user.Name,
            Email = user.Email,
            Age = user.Age
        };

        // DictionaryとHashSetの両方に追加
        _users.Add(newId, newUser);
        _emails.Add(newUser.Email);
    }

    public bool Delete(int id)
    {
        if (_users.TryGetValue(id, out var target))
        {
            // 削除するときは、DictionaryとHashSetの両方から消す
            _users.Remove(id);
            _emails.Remove(target.Email);
            return true;
        }
        return false;
    }
}