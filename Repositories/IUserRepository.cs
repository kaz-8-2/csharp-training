namespace Day4UserConsole.Repositories;

public interface IUserRepository
{
    List<Models.User> GetAll();
    Models.User? GetById(int id);
    List<Models.User> SearchByName(string keyword);
    void Add(Models.User user);
    bool Delete(int id);
}