using System.Data;
using Microsoft.Data.SqlClient;

namespace Bai6_2.DAL;

public class PhongBanDAL
{
    public DataTable LayDanhSach()
    {
        using var conn = DatabaseHelper.GetConnection();
        using var cmd = new SqlCommand(
            "SELECT MaPB, TenPB, Mota FROM PhongBan ORDER BY TenPB",
            conn);

        using var adapter = new SqlDataAdapter(cmd);
        var dt = new DataTable();
        adapter.Fill(dt);
        return dt;
    }
}