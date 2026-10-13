using Day4UserConsole.Models;

namespace Day4UserConsole.Repositories;

public interface IUserRepository
{
    List<UserDetails> GetAllSorted();
    User? GetById(int id);
    List<User> SearchByName(string keyword);
    void Add(User user);
    bool Delete(int id);
    UserSummary GetSummary(); // 追加
}