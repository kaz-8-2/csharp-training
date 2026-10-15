using UserManagementApp.Controllers;
using UserManagementApp.Repositories;

IUserRepository repository = new UserRepository();
UserController controller = new UserController(repository);

while (true)
{
    Console.WriteLine("\n=============================");
    Console.WriteLine("ユーザー管理アプリケーション");
    Console.WriteLine("=============================");
    Console.WriteLine("1. ユーザー登録");
    Console.WriteLine("2. ユーザー一覧表示");
    Console.WriteLine("3. ユーザー検索 (名前)");
    Console.WriteLine("4. ユーザー削除");
    Console.WriteLine("5. ユーザー統計情報表示");
    Console.WriteLine("9. 終了");
    Console.Write("メニューを選択してください: ");
    
    string? choice = Console.ReadLine();

    switch (choice)
    {
        case "1":
            await controller.RegisterUserAsync();
            break;
        case "2":
            await controller.ShowAllUsersAsync();
            break;
        case "3":
            await controller.SearchUsersAsync();
            break;
        case "4":
            await controller.DeleteUserAsync();
            break;
        case "5":
            await controller.ShowSummaryAsync();
            break;
        case "9":
            Console.WriteLine("アプリケーションを終了します。");
            return;
        default:
            Console.WriteLine("エラー: 無効な選択です。もう一度入力してください。");
            break;
    }
}