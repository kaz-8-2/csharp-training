using UserManagementApp.Models;
using UserManagementApp.Repositories;
using Xunit;

namespace UserManagementApp.Tests;

public class UserRepositoryTests : IDisposable
{
    private readonly string _testFilePath = "users.json";
    private readonly UserRepository _repository;

    public UserRepositoryTests()
    {
        // 各テスト実行前の前処理：古いテスト用JSONファイルを削除
        if (File.Exists(_testFilePath))
        {
            File.Delete(_testFilePath);
        }

        _repository = new UserRepository();
    }

    public void Dispose()
    {
        // 各テスト終了後の後処理：クリーンアップ
        if (File.Exists(_testFilePath))
        {
            File.Delete(_testFilePath);
        }
    }

    // 正常系テスト：新規ユーザーが正常に追加できるか
    [Fact]
    public async Task AddAsync_新規ユーザーを追加できること()
    {
        // Arrange (準備)
        var user = new User
        {
            Name = "テスト太郎",
            Email = "taro@example.com",
            Age = 25,
            DepartmentId = 2
        };

        // Act (実行)
        await _repository.AddAsync(user);
        var result = await _repository.GetByIdAsync(1);

        // Assert (検証)
        Assert.NotNull(result);
        Assert.Equal("テスト太郎", result.Name);
        Assert.Equal("taro@example.com", result.Email);
    }

    // 異常系テスト：同一メールアドレスの重複登録時に ArgumentException がスローされるか
    [Fact]
    public async Task AddAsync_メールアドレスが重複している場合は例外が発生すること()
    {
        // Arrange (準備)
        var user1 = new User { Name = "ユーザー1", Email = "duplicate@example.com", Age = 30, DepartmentId = 1 };
        var user2 = new User { Name = "ユーザー2", Email = "duplicate@example.com", Age = 28, DepartmentId = 2 };

        await _repository.AddAsync(user1);

        // Act & Assert (実行と検証を同時に行う)
        var exception = await Assert.ThrowsAsync<ArgumentException>(async () =>
        {
            await _repository.AddAsync(user2);
        });

        Assert.Contains("既に登録されています", exception.Message);
    }

    // 正常系テスト：ユーザー削除が成功すること
    [Fact]
    public async Task DeleteAsync_存在するIDを指定した場合は削除に成功しtrueを返すこと()
    {
        // Arrange (準備)
        var user = new User { Name = "削除対象", Email = "delete@example.com", Age = 40, DepartmentId = 3 };
        await _repository.AddAsync(user);

        // Act (実行)
        bool deleteResult = await _repository.DeleteAsync(1);
        var remainingUser = await _repository.GetByIdAsync(1);

        // Assert (検証)
        Assert.True(deleteResult);
        Assert.Null(remainingUser);
    }
}