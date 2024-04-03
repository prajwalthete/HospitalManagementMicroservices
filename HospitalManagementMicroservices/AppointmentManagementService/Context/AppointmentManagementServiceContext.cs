using Microsoft.Data.SqlClient;
using System.Data;

namespace AppointmentManagementService.Context
{
    public class AppointmentManagementServiceContext
    {

        private readonly IConfiguration _configuration;

        private readonly string _connectionString;

        public AppointmentManagementServiceContext(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("AppointmentManagementServiceConnection");
        }

        public IDbConnection CreateConnection() => new SqlConnection(_connectionString);


    }
}
