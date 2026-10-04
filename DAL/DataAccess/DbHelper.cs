using System.Configuration;
using System.Data.SqlClient;

namespace CuahangNongduoc.DAL.DataAccess
{
    public static class DbHelper
    {
        private static readonly string connectionString =
            ConfigurationManager.ConnectionStrings["CuahangNongDuoc"].ConnectionString;

        public static SqlConnection GetConnection()
        {
            return new SqlConnection(connectionString);
        }
    }
}