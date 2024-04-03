using Microsoft.Data.SqlClient;
using System.Data;

namespace DoctorManagementService.Context
{
    public class DoctorManagementServiceContext
    {
        private readonly IConfiguration _configuration;

        private readonly string _connectionString;

        public DoctorManagementServiceContext(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("DoctorManagementServiceConnection");
        }

        public IDbConnection CreateConnection() => new SqlConnection(_connectionString);
    }
}
