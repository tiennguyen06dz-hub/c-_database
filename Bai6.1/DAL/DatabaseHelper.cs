
using Microsoft.Data.SqlClient;

namespace Bai6._1.DAL
{
    public class DatabaseHelper
    {
        private static string connectionString =
            @"Server=DESKTOP-NCDMVRV\TIEN;
              Database=SalesDB;
              Trusted_Connection=True;
              TrustServerCertificate=True;";

        public static SqlConnection GetConnection()
        {
            return new SqlConnection(connectionString);
        }
    }
}
