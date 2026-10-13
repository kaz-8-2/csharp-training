using Day4UserConsole.Models;

namespace Day4UserConsole.Repositories;

public interface IUserRepository
{
    List<User> GetAll();
    User? GetById(int id);
    List<User> SearchByName(string keyword);
    void Add(User user);
    bool Delete(int id);
    UserSummary GetSummary(); // 追加
}