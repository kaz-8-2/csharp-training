using UserManagementApp.Models;

namespace UserManagementApp.Repositories;

public interface IUserRepository
{
    Task<List<UserDetails>> GetAllSortedAsync();
    Task<User?> GetByIdAsync(int id);
    Task<List<User>> SearchByNameAsync(string keyword);
    Task AddAsync(User user);
    Task<bool> DeleteAsync(int id);
    Task<UserSummary> GetSummaryAsync();
}