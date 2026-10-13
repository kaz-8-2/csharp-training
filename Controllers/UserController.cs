using Day4UserConsole.Repositories;
using Day4UserConsole.Models;

namespace Day4UserConsole.Controllers;

public class UserController
{
    private readonly IUserRepository _repository;

    public UserController(IUserRepository repository)
    {
        _repository = repository;
    }

    public void RegisterUser()
    {
        Console.WriteLine("\n--- ユーザー登録 ---");
        Console.Write("名前を入力: ");
        string? name = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(name)) { Console.WriteLine("エラー: 名前は必須入力です。"); return; }

        Console.Write("メールアドレスを入力: ");
        string? email = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(email)) { Console.WriteLine("エラー: メールアドレスは必須入力です。"); return; }

        Console.Write("年齢を入力: ");
        if (!int.TryParse(Console.ReadLine(), out int age) || age < 0) { Console.WriteLine("エラー: 年齢は0以上の数値を入力してください。"); return; }

        // ★ 部署の入力を追加
        Console.Write("部署を選択 (1:営業部, 2:開発部, 3:総務部): ");
        if (!int.TryParse(Console.ReadLine(), out int deptId) || deptId < 1 || deptId > 3) 
        { 
            Console.WriteLine("エラー: 無効な部署です。"); return; 
        }

        try
        {
            _repository.Add(new User { Name = name, Email = email, Age = age, DepartmentId = deptId });
            Console.WriteLine("ユーザーを登録しました。");
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine($"エラー: {ex.Message}");
        }
    }

    public void ShowAllUsers()
    {
        Console.WriteLine("\n--- ユーザー一覧 (年齢順) ---");
        var users = _repository.GetAllSorted(); // ★変更箇所

        if (!users.Any()) { Console.WriteLine("登録されているユーザーはいません。"); return; }

        foreach (var user in users)
        {
            // 部署名が追加されている！
            Console.WriteLine($"ID: {user.Id} | 部署: {user.DepartmentName} | 名前: {user.Name} | 年齢: {user.Age} | メール: {user.Email}");
        }
    }

    public void SearchUsers()
    {
        Console.WriteLine("\n--- ユーザー検索 ---");
        Console.Write("検索キーワード (名前) を入力: ");
        string? keyword = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(keyword)) { Console.WriteLine("エラー: 検索キーワードを入力してください。"); return; }

        var results = _repository.SearchByName(keyword);
        if (!results.Any()) { Console.WriteLine("該当するユーザーは見つかりませんでした。"); return; }

        foreach (var user in results)
        {
            Console.WriteLine($"ID: {user.Id} | 名前: {user.Name} | 年齢: {user.Age} | メール: {user.Email}");
        }
    }

    public void DeleteUser()
    {
        Console.WriteLine("\n--- ユーザー削除 ---");
        Console.Write("削除するユーザーIDを入力: ");
        if (!int.TryParse(Console.ReadLine(), out int id)) { Console.WriteLine("エラー: 有効な数値を入力してください。"); return; }

        bool success = _repository.Delete(id);
        if (success) Console.WriteLine($"ID: {id} のユーザーを削除しました。");
        else Console.WriteLine($"エラー: ID: {id} のユーザーは見つかりませんでした。");
    }

    public void ShowSummary()
    {
        Console.WriteLine("\n--- ユーザー統計情報 ---");
        var summary = _repository.GetSummary();

        if (summary.TotalCount == 0) { Console.WriteLine("登録されているユーザーがいません。"); return; }

        Console.WriteLine($"総ユーザー数: {summary.TotalCount}名");
        Console.WriteLine($"平均年齢    : {summary.AverageAge}歳");
        Console.WriteLine($"最年少 / 最年長: {summary.MinAge}歳 / {summary.MaxAge}歳");
        Console.WriteLine($"利用ドメイン一覧: {string.Join(", ", summary.EmailDomainList)}");

        // ★ 年代別構成比を表示
        Console.WriteLine("\n【年代別 構成比】");
        foreach (var group in summary.AgeGroupCounts)
        {
            Console.WriteLine($" - {group.Key}: {group.Value}名");
        }
    }
}