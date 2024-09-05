using Dapper;
using KnowledgeMining.DTOs;
using KnowledgeMining.Helpers;
using KnowledgeMining.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace KnowledgeMining.Data
{
    public class RolesRepository:IRolesRepository
    {
        private readonly IDbConnection _dbConnection;
        private readonly ILogger<RolesRepository> _logger;
        public RolesRepository(IDbConnection dbConnection, ILogger<RolesRepository> logger)
        {
            _dbConnection = dbConnection;
            _logger = logger;
        }


        public async Task<IEnumerable<Role>> GetAllUserRolesAsync()
        {
            try
            {
                return await _dbConnection.QueryAsync<Role>("GetAllUserRoles", commandType: CommandType.StoredProcedure);
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving all users.");
                throw new DataAccessException("An error occurred while retrieving all users.", ex);
            }
        }
        public async Task<Role> GetRoleByIdAsync(int id)
        {
            try
            {
                return await _dbConnection.QuerySingleOrDefaultAsync<Role>("GetUserRolesById", new { Id = id }, commandType: CommandType.StoredProcedure);
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, $"An error occurred while retrieving Role with ID {id}.");
                throw new DataAccessException($"An error occurred while retrieving Role with ID {id}.", ex);
            }
        }

        public async Task<int> CreateRoleAsync(Role Role)
        {
            try
            {
                var sqlParams = new { RoleAr = Role.RoleAr, RoleEn = Role.RoleEn };
                return await _dbConnection.QuerySingleAsync<int>("CreateUserRoles", sqlParams, commandType: CommandType.StoredProcedure);
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "An error occurred while creating a new Role.");
                throw new DataAccessException("An error occurred while creating a new Role.", ex);
            }
        }

        public async Task<Role> UpdateRoleAsync(Role Role)
        {
            try
            {
                var sqlParams = new { RoleAr = Role.RoleAr, RoleEn = Role.RoleEn, Id = Role.Id };
                var updatedRole = await _dbConnection.QuerySingleOrDefaultAsync<Role>(
                    "UpdateUserRoles",
                    sqlParams,
                    commandType: CommandType.StoredProcedure
                );

                return updatedRole;
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, $"An error occurred while updating Role with ID {Role.Id}.");
                throw new DataAccessException($"An error occurred while updating Role with ID {Role.Id}.", ex);
            }
        }

        public async Task<int?> DeleteRoleAsync(int id)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Id", id, DbType.Int32, ParameterDirection.Input);
                parameters.Add("@DeletedId", dbType: DbType.Int32, direction: ParameterDirection.Output);

                await _dbConnection.ExecuteAsync("DeleteUserRoles", parameters, commandType: CommandType.StoredProcedure);

                var deletedId = parameters.Get<int?>("@DeletedId");

                return deletedId;
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, $"An error occurred while deleting Role with ID {id}.");
                throw new DataAccessException($"An error occurred while deleting Role with ID {id}.", ex);
            }
        }
    }
}
