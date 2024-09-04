using System.Data;
using Dapper;
using Npgsql;

namespace WebCrawler.Controllers
{
    public static class DataAccess
    {
        public static List<T> LoadData<T, U>(string sql, U parameters, string connectionString)
        {
            using (IDbConnection connection = new NpgsqlConnection(connectionString))
            {
                List<T> row = connection.Query<T>(sql, parameters).ToList();
                return row;
            }
        }

        public static void SaveData<T>(string sql, T parameters, string connectionString)
        {
            using (IDbConnection connection = new NpgsqlConnection(connectionString))
            {
                connection.Execute(sql, parameters);
            }
        }
    }
}
