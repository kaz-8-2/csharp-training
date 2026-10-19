using Moq;
using UserManagementApp.Controllers;
using UserManagementApp.Models;
using UserManagementApp.Repositories;
using Xunit;

namespace UserManagementApp.Tests;

public class UserControllerTests : IDisposable
{
    private readonly Mock<IUserRepository> _mockRepository;
    private readonly UserController _controller;
    private readonly TextWriter _originalOut;
    private readonly TextReader _originalIn;
    private readonly StringWriter _stringWriter;

    public UserControllerTests()
    {
        // 1. リポジトリのモック作成
        _mockRepository = new Mock<IUserRepository>();

        // 2. モックを注入したコントローラー作成
        _controller = new UserController(_mockRepository.Object);

        // 3. 標準入出力を退避し、テスト用のメモリバッファへ差し替え
        _originalOut = Console.Out;
        _originalIn = Console.In;
        _stringWriter = new StringWriter();
        Console.SetOut(_stringWriter);
    }

    public void Dispose()
    {
        // テスト終了時に標準入出力を元の状態に復元
        Console.SetOut(_originalOut);
        Console.SetIn(_originalIn);
        _stringWriter.Dispose();
    }

    // 擬似的なキーボード入力をセットするヘルパーメソッド
    private void SetUserInput(params string[] lines)
    {
        var input = string.Join(Environment.NewLine, lines);
        Console.SetIn(new StringReader(input));
    }

    // ==========================================
    // 正常系テストケース
    // ==========================================

    [Fact]
    public async Task ShowAllUsersAsync_ユーザーが存在する場合_一覧を画面に出力すること()
    {
        // Arrange
        var mockUsers = new List<UserDetails>
        {
            new UserDetails { Id = 1, Name = "山田太郎", Age = 30, Email = "yamada@example.com", DepartmentName = "開発部" }
        };

        _mockRepository
            .Setup(r => r.GetAllSortedAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockUsers);

        // Act
        await _controller.ShowAllUsersAsync();
        var output = _stringWriter.ToString();

        // Assert
        Assert.Contains("山田太郎", output);
        Assert.Contains("開発部", output);
        Assert.Contains("yamada@example.com", output);
        _mockRepository.Verify(r => r.GetAllSortedAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task RegisterUserAsync_正しい入力値_リポジトリのAddAsyncが呼ばれ登録完了が出力されること()
    {
        // Arrange (名前, メールアドレス, 年齢, 部署ID)
        SetUserInput("佐藤次郎", "sato@example.com", "28", "2");

        // Act
        await _controller.RegisterUserAsync();
        var output = _stringWriter.ToString();

        // Assert
        Assert.Contains("ユーザーを登録しました。", output);
        // AddAsync が確実に 1 回呼ばれたことを検証
        _mockRepository.Verify(
            r => r.AddAsync(It.Is<User>(u => u.Name == "佐藤次郎" && u.Email == "sato@example.com" && u.Age == 28 && u.DepartmentId == 2),
            It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Fact]
    public async Task DeleteUserAsync_存在するIDを指定_削除成功メッセージが出力されること()
    {
        // Arrange
        SetUserInput("1");
        _mockRepository
            .Setup(r => r.DeleteAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        await _controller.DeleteUserAsync();
        var output = _stringWriter.ToString();

        // Assert
        Assert.Contains("ID: 1 のユーザーを削除しました。", output);
        _mockRepository.Verify(r => r.DeleteAsync(1, It.IsAny<CancellationToken>()), Times.Once);
    }

    // ==========================================
    // 異常系・境界値テストケース
    // ==========================================

    [Fact]
    public async Task ShowAllUsersAsync_ユーザーが0件の場合_未登録メッセージが出力されること()
    {
        // Arrange
        _mockRepository
            .Setup(r => r.GetAllSortedAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<UserDetails>());

        // Act
        await _controller.ShowAllUsersAsync();
        var output = _stringWriter.ToString();

        // Assert
        Assert.Contains("登録されているユーザーはいません。", output);
    }

    [Theory]
    [InlineData("", "test@example.com", "20", "1", "名前は必須入力です")]
    [InlineData("山田", "", "20", "1", "メールアドレスは必須入力です")]
    [InlineData("山田", "test@example.com", "-1", "1", "年齢は0以上の数値を入力してください")]
    [InlineData("山田", "test@example.com", "abc", "1", "年齢は0以上の数値を入力してください")]
    [InlineData("山田", "test@example.com", "20", "0", "無効な部署です")]
    [InlineData("山田", "test@example.com", "20", "4", "無効な部署です")]
    public async Task RegisterUserAsync_不正な入力値_リポジトリ呼び出しを行わずバリデーションエラーを出力すること(
        string name, string email, string age, string deptId, string expectedError)
    {
        // Arrange
        SetUserInput(name, email, age, deptId);

        // Act
        await _controller.RegisterUserAsync();
        var output = _stringWriter.ToString();

        // Assert
        Assert.Contains(expectedError, output);
        // 入力検証エラーのため、リポジトリの AddAsync は一度も呼ばれてはいけない
        _mockRepository.Verify(r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task RegisterUserAsync_メール重複例外発生時_ArgumentExceptionのエラーを出力すること()
    {
        // Arrange
        SetUserInput("重複太郎", "dup@example.com", "30", "1");

        // リポジトリが ArgumentException をスローする挙動をシミュレート
        _mockRepository
            .Setup(r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new ArgumentException("メールアドレス 'dup@example.com' は既に登録されています。"));

        // Act
        await _controller.RegisterUserAsync();
        var output = _stringWriter.ToString();

        // Assert
        Assert.Contains("エラー: メールアドレス 'dup@example.com' は既に登録されています。", output);
    }

    [Fact]
    public async Task DeleteUserAsync_数値以外のID入力_エラーを出力し削除処理を行わないこと()
    {
        // Arrange
        SetUserInput("invalid_id");

        // Act
        await _controller.DeleteUserAsync();
        var output = _stringWriter.ToString();

        // Assert
        Assert.Contains("有効な数値を入力してください", output);
        _mockRepository.Verify(r => r.DeleteAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task DeleteUserAsync_存在しないID_失敗メッセージを出力すること()
    {
        // Arrange
        SetUserInput("999");
        _mockRepository
            .Setup(r => r.DeleteAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        await _controller.DeleteUserAsync();
        var output = _stringWriter.ToString();

        // Assert
        Assert.Contains("ID: 999 のユーザーは見つかりませんでした。", output);
    }
}