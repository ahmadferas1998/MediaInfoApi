using System.Data;
using Dapper;
using KnowledgeMining.Models;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using KnowledgeMining.DTOs;
using KnowledgeMining.Helpers;
using System.Runtime.CompilerServices;

namespace KnowledgeMining.Data
{
    public class UserRepository : IUserRepository
    {
        private readonly IDbConnection _dbConnection;
        private readonly ILogger<UserRepository> _logger;

        public UserRepository(IDbConnection dbConnection, ILogger<UserRepository> logger)
        {
            _dbConnection = dbConnection;
            _logger = logger;
        }

        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            try
            {
                return await _dbConnection.QueryAsync<UserDto>("GetAllUsers", commandType: CommandType.StoredProcedure);
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving all users.");
                throw new DataAccessException("An error occurred while retrieving all users.", ex);
            }
        }

        public async Task<User> GetUserByIdAsync(int id)
        {
            try
            {
                return await _dbConnection.QuerySingleOrDefaultAsync<User>("GetUserById", new { Id = id }, commandType: CommandType.StoredProcedure);
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, $"An error occurred while retrieving user with ID {id}.");
                throw new DataAccessException($"An error occurred while retrieving user with ID {id}.", ex);
            }
        }

        public async Task<int> CreateUserAsync(User user)
        {
            try
            {
                var EncreptionPassword = EncryptionTool.encrypt(user.Password);
                var sqlParams = new { Username = user.Username, Password = EncreptionPassword, RoleId = user.RoleId , firstName = user.firstName , lastName = user.lastName, GenderId=user.GenderId , Email=user.Email };
                return await _dbConnection.QuerySingleAsync<int>("CreateUser", sqlParams, commandType: CommandType.StoredProcedure);
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "An error occurred while creating a new user.");
                throw new DataAccessException("An error occurred while creating a new user.", ex);
            }
        }

        public async Task<User> UpdateUserAsync(User user)
        {
            try
            {
                var EncreptionPassword = EncryptionTool.encrypt(user.Password);
                var sqlParams = new { Id=user.Id, Username = user.Username, Password = EncreptionPassword, RoleId = user.RoleId, firstName = user.firstName, lastName = user.lastName, GenderId = user.GenderId, Email = user.Email };

                var updatedUser = await _dbConnection.QuerySingleOrDefaultAsync<User>(
                    "UpdateUser",
                    sqlParams,
                    commandType: CommandType.StoredProcedure
                );

                return updatedUser;
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, $"An error occurred while updating user with ID {user.Id}.");
                throw new DataAccessException($"An error occurred while updating user with ID {user.Id}.", ex);
            }
        }

        public async Task<int?> DeleteUserAsync(int id)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Id", id, DbType.Int32, ParameterDirection.Input);
                parameters.Add("@DeletedId", dbType: DbType.Int32, direction: ParameterDirection.Output);

                await _dbConnection.ExecuteAsync("DeleteUser", parameters, commandType: CommandType.StoredProcedure);

                var deletedId = parameters.Get<int?>("@DeletedId");

                return deletedId;
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, $"An error occurred while deleting user with ID {id}.");
                throw new DataAccessException($"An error occurred while deleting user with ID {id}.", ex);
            }
        }


        public async Task<User> GetUserByUsernameAndPasswordAsync(string username, string password)
        {
            try
            {
                var result = await _dbConnection.QuerySingleOrDefaultAsync<User>(
                    "GetUserByUsernameAndPassword",
                    new { Username = username},
                    commandType: CommandType.StoredProcedure
                );

                if (result.Id == 0 || result.Id == null)
                {
                    return new User { Id = 0, Username = null, Password = null };
                }

                var decryptedPassword = EncryptionTool.Decrypt(result.Password);

                if (decryptedPassword == password)
                {
                    return result; 
                }
                else
                {
                    return new User {Id=0,Username=null,Password=null};
                }
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, $"An error occurred while retrieving user with username {username}.");
                throw new DataAccessException($"An error occurred while retrieving user with username {username}.", ex);
            }
        }

        public async Task<string> GetUserRolesAsync(int userId)
        {
            try
            {
                var roles = await _dbConnection.QueryAsync<string>(
                    "GetUserRoles",
                    new { UserId = userId },
                    commandType: CommandType.StoredProcedure
                );

                var result = string.Join(", ", roles);

                return result;
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, $"An error occurred while retrieving roles for user with ID {userId}.");
                throw new DataAccessException($"An error occurred while retrieving roles for user with ID {userId}.", ex);
            }
        }

    }


}


