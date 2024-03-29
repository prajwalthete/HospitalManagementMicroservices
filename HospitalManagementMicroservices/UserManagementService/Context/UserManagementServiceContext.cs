using Microsoft.Data.SqlClient;
using System.Data;

namespace UserManagementService.Context
{
    public class UserManagementServiceContext
    {
        private readonly IConfiguration _configuration;

        private readonly string _connectionString;

        public UserManagementServiceContext(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("UserManagementServiceConnection");
        }

        public IDbConnection CreateConnection() => new SqlConnection(_connectionString);


    }
}
