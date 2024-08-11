using System.Data;
using Microsoft.Data.SqlClient;

namespace InternProject.Abstract
{
    public interface IDatabaseService
    {
        Task<List<string>> GetDatabasesAsync(string serverName, string userName, string password, bool isSecure);
        Task<List<string>> GetDatabasesAsync(bool isSecure);
        Task<List<string>> GetTablesAsync(string databaseName, bool isSecure);
        Task<List<string>> GetViewsAsync(string databaseName, bool isSecure);
        Task<List<string>> GetFunctionsAsync(string databaseName, bool isSecure);
        Task<List<string>> GetStoredProceduresAsync(string databaseName, bool isSecure);
        Task<DataTable> GetTableDataAsync(string databaseName, string tableName, bool isSecure);
        Task<DataTable> GetViewDataAsync(string databaseName, string viewName, bool isSecure);
        Task<DataTable> GetStoredProcedureDataAsync(string databaseName, string procedureName, bool isSecure, Dictionary<string, object>? parameters = null);
        Task<DataTable> GetFunctionDataAsync(string databaseName, string functionName, bool isSecure, Dictionary<string, object>? parameters = null);
        Task<List<SqlParameter>> GetStoredProcedureParametersAsync(string databaseName, string procedureName, bool isSecure);
        Task<List<SqlParameter>> GetFunctionParametersAsync(string databaseName, string functionName, bool isSecure);
    }
}
  