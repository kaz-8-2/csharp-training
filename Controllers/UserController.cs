using Day4UserConsole.Repositories;
using Day4UserConsole.Models;

namespace Day4UserConsole.Controllers;

public class UserController
{
    private readonly IUserRepository _repository;

    // DI（コンストラクタ注入）でリポジトリを受け取る
    public UserController(IUserRepository repository)
    {
        _repository = repository;
    }

    public void RegisterUser()
    {
        Console.WriteLine("\n--- ユーザー登録 ---");
        Console.Write("名前を入力: ");
        string? name = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(name))
        {
            Console.WriteLine("エラー: 名前は必須入力です。");
            return;
        }

        Console.Write("メールアドレスを入力: ");
        string? email = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(email))
        {
            Console.WriteLine("エラー: メールアドレスは必須入力です。");
            return;
        }

        Console.Write("年齢を入力: ");
        if (!int.TryParse(Console.ReadLine(), out int age) || age < 0)
        {
            Console.WriteLine("エラー: 年齢は0以上の数値を入力してください。");
            return;
        }

        // ▼▼▼ ここから変更 ▼▼▼
        try
        {
            _repository.Add(new User { Name = name, Email = email, Age = age });
            Console.WriteLine("ユーザーを登録しました。");
        }
        catch (ArgumentException ex)
        {
            // HashSetで重複検知した例外をここでキャッチして表示する
            Console.WriteLine($"エラー: {ex.Message}");
        }
        // ▲▲▲ ここまで変更 ▲▲▲
    }

    public void ShowAllUsers()
    {
        Console.WriteLine("\n--- ユーザー一覧 ---");
        var users = _repository.GetAll();
        if (!users.Any())
        {
            Console.WriteLine("登録されているユーザーはいません。");
            return;
        }

        foreach (var user in users)
        {
            Console.WriteLine($"ID: {user.Id} | 名前: {user.Name} | 年齢: {user.Age} | メール: {user.Email}");
        }
    }

    public void SearchUsers()
    {
        Console.WriteLine("\n--- ユーザー検索 ---");
        Console.Write("検索キーワード (名前) を入力: ");
        string? keyword = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(keyword))
        {
            Console.WriteLine("エラー: 検索キーワードを入力してください。");
            return;
        }

        var results = _repository.SearchByName(keyword);
        if (!results.Any())
        {
            Console.WriteLine("該当するユーザーは見つかりませんでした。");
            return;
        }

        foreach (var user in results)
        {
            Console.WriteLine($"ID: {user.Id} | 名前: {user.Name} | 年齢: {user.Age} | メール: {user.Email}");
        }
    }

    public void DeleteUser()
    {
        Console.WriteLine("\n--- ユーザー削除 ---");
        Console.Write("削除するユーザーIDを入力: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        {
            Console.WriteLine("エラー: 有効な数値を入力してください。");
            return;
        }

        bool success = _repository.Delete(id);
        if (success)
        {
            Console.WriteLine($"ID: {id} のユーザーを削除しました。");
        }
        else
        {
            Console.WriteLine($"エラー: ID: {id} のユーザーは見つかりませんでした。");
        }
    }
}