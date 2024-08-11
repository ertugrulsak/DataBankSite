using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using InternProject.Abstract;

namespace InternProject.Services
{
    public class DatabaseService : IDatabaseService
    {
        private readonly IConfiguration _configuration;

        public DatabaseService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<List<string>> GetDatabasesAsync(string serverName, string userName, string password, bool isSecure)
        {
            var connectionString = isSecure ?
                $"Server={serverName};User Id={userName};Password={password};TrustServerCertificate=True;" :
                $"Server={serverName};Trusted_Connection=True;TrustServerCertificate=True;Integrated Security=True;";

            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                var command = new SqlCommand("SELECT name FROM sys.databases;", connection);
                var reader = await command.ExecuteReaderAsync();

                var databases = new List<string>();
                while (await reader.ReadAsync())
                {
                    databases.Add(reader.GetString(0));
                }

                return databases;
            }
        }

        public async Task<List<string>> GetDatabasesAsync(bool isSecure)
        {
            var connectionString = isSecure ?
                _configuration.GetConnectionString("SecureConnection") :
                _configuration.GetConnectionString("TrustedConnection");

            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                var command = new SqlCommand(@"
                    SELECT name 
                    FROM sys.databases
                    WHERE name NOT IN ('master', 'tempdb', 'model', 'msdb')", connection);
                var reader = await command.ExecuteReaderAsync();

                var databases = new List<string>();
                while (await reader.ReadAsync())
                {
                    databases.Add(reader.GetString(0));
                }

                return databases;
            }
        }

        public async Task<List<string>> GetTablesAsync(string databaseName, bool isSecure)
        {
            var connectionString = isSecure ?
                _configuration.GetConnectionString("SecureConnection") :
                _configuration.GetConnectionString("TrustedConnection");

            connectionString += $"Database={databaseName};";

            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                var command = new SqlCommand("SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE';", connection);
                var reader = await command.ExecuteReaderAsync();

                var tables = new List<string>();
                while (await reader.ReadAsync())
                {
                    tables.Add(reader.GetString(0));
                }

                return tables;
            }
        }

        public async Task<List<string>> GetViewsAsync(string databaseName, bool isSecure)
        {
            var connectionString = isSecure ?
                _configuration.GetConnectionString("SecureConnection") :
                _configuration.GetConnectionString("TrustedConnection");

            connectionString += $"Database={databaseName};";

            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                var command = new SqlCommand(@"
                    SELECT TABLE_NAME FROM INFORMATION_SCHEMA.VIEWS;", connection);
                var reader = await command.ExecuteReaderAsync();

                var views = new List<string>();
                while (await reader.ReadAsync())
                {
                    views.Add(reader.GetString(0));
                }

                return views;
            }
        }

        public async Task<List<string>> GetFunctionsAsync(string databaseName, bool isSecure)
        {
            var connectionString = isSecure ?
                _configuration.GetConnectionString("SecureConnection") :
                _configuration.GetConnectionString("TrustedConnection");

            connectionString += $"Database={databaseName};";

            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                var command = new SqlCommand(@"
                    SELECT SPECIFIC_NAME 
                    FROM INFORMATION_SCHEMA.ROUTINES 
                    WHERE ROUTINE_TYPE='FUNCTION'", connection);
                var reader = await command.ExecuteReaderAsync();

                var functions = new List<string>();
                while (await reader.ReadAsync())
                {
                    functions.Add(reader.GetString(0));
                }

                return functions;
            }
        }

        public async Task<List<string>> GetStoredProceduresAsync(string databaseName, bool isSecure)
        {
            var connectionString = isSecure ?
                _configuration.GetConnectionString("SecureConnection") :
                _configuration.GetConnectionString("TrustedConnection");

            connectionString += $"Database={databaseName};";

            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                var command = new SqlCommand(@"
                    SELECT name 
                    FROM sys.procedures 
                    WHERE schema_id = SCHEMA_ID('dbo')
                      AND name NOT IN ('sp_upgraddiagrams', 'sp_helpdiagrams', 'sp_helpdiagramdefinition', 
                                       'sp_creatediagram', 'sp_renamediagram', 'sp_alterdiagram', 'sp_dropdiagram');", connection);
                var reader = await command.ExecuteReaderAsync();

                var procedures = new List<string>();
                while (await reader.ReadAsync())
                {
                    procedures.Add(reader.GetString(0));
                }

                return procedures;
            }
        }

        public async Task<DataTable> GetTableDataAsync(string databaseName, string tableName, bool isSecure)
        {
            var connectionString = isSecure ?
                _configuration.GetConnectionString("SecureConnection") :
                _configuration.GetConnectionString("TrustedConnection");

            connectionString += $"Database={databaseName};";

            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                var command = new SqlCommand($"SELECT * FROM {tableName};", connection);
                var reader = await command.ExecuteReaderAsync();

                var dataTable = new DataTable();
                dataTable.Load(reader);

                return dataTable;
            }
        }

        public async Task<DataTable> GetViewDataAsync(string databaseName, string viewName, bool isSecure)
        {
            var connectionString = isSecure ?
                _configuration.GetConnectionString("SecureConnection") :
                _configuration.GetConnectionString("TrustedConnection");

            connectionString += $"Database={databaseName};";

            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                var command = new SqlCommand($"SELECT * FROM {viewName};", connection);
                var reader = await command.ExecuteReaderAsync();

                var dataTable = new DataTable();
                dataTable.Load(reader);

                return dataTable;
            }
        }

        public async Task<DataTable> GetStoredProcedureDataAsync(string databaseName, string procedureName, bool isSecure, Dictionary<string, object>? parameters = null)
        {
            var connectionString = isSecure ?
                _configuration.GetConnectionString("SecureConnection") :
                _configuration.GetConnectionString("TrustedConnection");

            connectionString += $"Database={databaseName};";

            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                var command = new SqlCommand(procedureName, connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                if (parameters != null)
                {
                    foreach (var param in parameters)
                    {
                        command.Parameters.AddWithValue(param.Key, param.Value);
                    }
                }

                var reader = await command.ExecuteReaderAsync();

                var dataTable = new DataTable();
                dataTable.Load(reader);

                return dataTable;
            }
        }

        public async Task<List<SqlParameter>> GetStoredProcedureParametersAsync(string databaseName, string procedureName, bool isSecure)
        {
            var connectionString = isSecure ?
                _configuration.GetConnectionString("SecureConnection") :
                _configuration.GetConnectionString("TrustedConnection");

            connectionString += $"Database={databaseName};";

            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                var command = new SqlCommand(procedureName, connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                SqlCommandBuilder.DeriveParameters(command);

                return command.Parameters.Cast<SqlParameter>()
                    .Where(p => p.Direction != ParameterDirection.ReturnValue)
                    .ToList();
            }
        }

        public async Task<List<SqlParameter>> GetFunctionParametersAsync(string databaseName, string functionName, bool isSecure)
        {
            var connectionString = isSecure ?
                _configuration.GetConnectionString("SecureConnection") :
                _configuration.GetConnectionString("TrustedConnection");

            connectionString += $"Database={databaseName};";

            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                var command = new SqlCommand(@"
                    SELECT name, system_type_id 
                    FROM sys.parameters 
                    WHERE object_id = OBJECT_ID(@functionName);", connection);
                command.Parameters.AddWithValue("@functionName", functionName);
                var reader = await command.ExecuteReaderAsync();

                var parameters = new List<SqlParameter>();
                while (await reader.ReadAsync())
                {
                    var paramName = reader.GetString(reader.GetOrdinal("name"));
                    var paramTypeId = reader.GetByte(reader.GetOrdinal("system_type_id"));
                    var paramType = GetSqlDbType(paramTypeId);
                    if (!string.IsNullOrEmpty(paramName))
                    {
                        var param = new SqlParameter(paramName, paramType)
                        {
                            Direction = ParameterDirection.Input
                        };
                        parameters.Add(param);
                    }
                }

                return parameters;
            }
        }

        public async Task<DataTable> GetFunctionDataAsync(string databaseName, string functionName, bool isSecure, Dictionary<string, object>? parameters = null)
        {
            var connectionString = isSecure ?
                _configuration.GetConnectionString("SecureConnection") :
                _configuration.GetConnectionString("TrustedConnection");

            connectionString += $"Database={databaseName};";

            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                var paramList = parameters != null ? string.Join(", ", parameters.Keys.Select(k => "" + k)) : string.Empty;
                var command = new SqlCommand($"SELECT   dbo.{functionName}({paramList});", connection);

                if (parameters != null)
                {
                    foreach (var param in parameters)
                    {
                        command.Parameters.AddWithValue(param.Key, param.Value);
                    }
                }

                var reader = await command.ExecuteReaderAsync();

                var dataTable = new DataTable();
                dataTable.Load(reader);

                return dataTable;
            }
        }

        private SqlDbType GetSqlDbType(byte typeId)
        {
            switch (typeId)
            {
                case 56: return SqlDbType.Int;
                case 60: return SqlDbType.Money;
                case 61: return SqlDbType.DateTime;
                case 104: return SqlDbType.Bit;
                case 167: return SqlDbType.VarChar;
                case 231: return SqlDbType.NVarChar;              
                default: throw new ArgumentOutOfRangeException(nameof(typeId), $"Unknown type ID: {typeId}");
            }
        }
    }
}
