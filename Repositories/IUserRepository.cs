using UserManagementApp.Models;

namespace UserManagementApp.Repositories;

public interface IUserRepository
{
    Task<List<UserDetails>> GetAllSortedAsync(CancellationToken cancellationToken = default);
    Task<User?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<List<User>> SearchByNameAsync(string keyword, CancellationToken cancellationToken = default);
    Task AddAsync(User user, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
    Task<UserSummary> GetSummaryAsync(CancellationToken cancellationToken = default);
}