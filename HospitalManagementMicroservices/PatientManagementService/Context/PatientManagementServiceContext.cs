using Microsoft.Data.SqlClient;
using System.Data;

namespace PatientManagementService.Context;

public class PatientManagementServiceContext
{
    private readonly IConfiguration _configuration;

    private readonly string _connectionString;

    public PatientManagementServiceContext(IConfiguration configuration)
    {
        _configuration = configuration;
        _connectionString = _configuration.GetConnectionString("PatientManagementServiceConnection");
    }

    public IDbConnection CreateConnection() => new SqlConnection(_connectionString);


}

