
using System.Configuration;
using Microsoft.Data.SqlClient;

namespace Bai6_2.DAL
{
    public static class DatabaseHelper
    {
        private static readonly string ConnectionString =
            ConfigurationManager
                .ConnectionStrings["HRMDB"]?
                .ConnectionString
            ?? throw new Exception(
                "Không tìm thấy chuỗi kết nối HRMDB.");

        public static SqlConnection GetConnection()
        {
            return new SqlConnection(ConnectionString);
        }
    }
}
