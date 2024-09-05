using System.Collections.Generic;
using System.Threading.Tasks;
using KnowledgeMining.Common;
using KnowledgeMining.DTOs;
using KnowledgeMining.Models;

namespace KnowledgeMining.Data
{
    public interface IUserRepository
    {
        Task<IEnumerable<UserDto>> GetAllUsersAsync();
        Task<User> GetUserByIdAsync(int id);
        Task<int> CreateUserAsync(User user);
        Task<User> UpdateUserAsync(User user);
         Task<int?> DeleteUserAsync(int id);
        Task<User> GetUserByUsernameAndPasswordAsync(string username, string password);
        Task<string> GetUserRolesAsync(int userId);

    }
}
