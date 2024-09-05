using KnowledgeMining.Models;

namespace KnowledgeMining.Data
{
    public interface IRolesRepository
    {
        Task<IEnumerable<Role>> GetAllUserRolesAsync();
        Task<Role> GetRoleByIdAsync(int id);
        Task<int> CreateRoleAsync(Role Role);

        Task<Role> UpdateRoleAsync(Role Role);

        Task<int?> DeleteRoleAsync(int id);
    }
}
