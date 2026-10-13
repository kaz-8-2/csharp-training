using Day4UserConsole.Repositories;
using Day4UserConsole.Controllers;

IUserRepository repository = new InMemoryUserRepository();
UserController controller = new UserController(repository);

while (true)
{
    Console.WriteLine("\n=== ユーザー管理システム ===");
    Console.WriteLine("1. ユーザー登録");
    Console.WriteLine("2. ユーザー一覧表示");
    Console.WriteLine("3. ユーザー検索 (名前)");
    Console.WriteLine("4. ユーザー削除");
    Console.WriteLine("9. 終了");
    Console.Write("選択してください > ");

    string? input = Console.ReadLine();

    switch (input)
    {
        case "1":
            controller.RegisterUser();
            break;
        case "2":
            controller.ShowAllUsers();
            break;
        case "3":
            controller.SearchUsers();
            break;
        case "4":
            controller.DeleteUser();
            break;
        case "9":
            Console.WriteLine("システムを終了します。");
            return;
        default:
            Console.WriteLine("無効な選択です。1〜4、または9を入力してください。");
            break;
    }
}