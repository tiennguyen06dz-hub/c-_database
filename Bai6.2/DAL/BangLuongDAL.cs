using System;
using System.Data;
using Microsoft.Data.SqlClient;

namespace Bai6_2.DAL;

public class BangLuongDAL
{
    public DataTable XemLuong(string thangNam)
    {
        using var conn = DatabaseHelper.GetConnection();

        using var cmd = new SqlCommand("""
            SELECT nv.MaNV,
                   nv.HoTen,
                   cc.ThangNam,
                   nv.LuongCoBan,
                   nv.HeSoLuong,
                   cc.SoNgayCong,
                   cc.SoGioOT,
                   CAST(
                       (nv.LuongCoBan * nv.HeSoLuong / 22.0)
                       * cc.SoNgayCong
                       + cc.SoGioOT * 50000
                       AS DECIMAL(18,2)
                   ) AS ThucLinh
            FROM BangChamCong cc
            INNER JOIN NhanVien nv ON cc.MaNV=nv.MaNV
            WHERE cc.ThangNam=@ThangNam
            ORDER BY nv.MaNV
            """, conn);

        cmd.Parameters.Add("@ThangNam",
            SqlDbType.VarChar, 7).Value = thangNam;

        using var adapter = new SqlDataAdapter(cmd);
        var dt = new DataTable();
        adapter.Fill(dt);
        return dt;
    }

    public DataTable XemBangLuongDaChot(string thangNam)
    {
        using var conn = DatabaseHelper.GetConnection();

        using var cmd = new SqlCommand("""
            SELECT bl.MaLuong, bl.MaNV,
                   nv.HoTen, bl.ThangNam, bl.ThucLinh
            FROM BangLuong bl
            INNER JOIN NhanVien nv ON bl.MaNV=nv.MaNV
            WHERE bl.ThangNam=@ThangNam
            ORDER BY bl.MaNV
            """, conn);

        cmd.Parameters.Add("@ThangNam",
            SqlDbType.VarChar, 7).Value = thangNam;

        using var adapter = new SqlDataAdapter(cmd);
        var dt = new DataTable();
        adapter.Fill(dt);
        return dt;
    }

    public int ChotLuong(string thangNam)
    {
        using var conn = DatabaseHelper.GetConnection();
        conn.Open();

        using var trans = conn.BeginTransaction(
            IsolationLevel.Serializable);

        try
        {
            using (var check = new SqlCommand("""
                SELECT COUNT(*)
                FROM BangLuong WITH (UPDLOCK, HOLDLOCK)
                WHERE ThangNam=@ThangNam
                """, conn, trans))
            {
                check.Parameters.Add("@ThangNam",
                    SqlDbType.VarChar, 7).Value = thangNam;

                if (Convert.ToInt32(check.ExecuteScalar()) > 0)
                    throw new Exception(
                        "Tháng này đã được chốt lương.");
            }

            using var cmd = new SqlCommand("""
                INSERT INTO BangLuong
                (MaNV, ThangNam, ThucLinh)
                SELECT nv.MaNV,
                       cc.ThangNam,
                       CAST(
                           (nv.LuongCoBan * nv.HeSoLuong / 22.0)
                           * cc.SoNgayCong
                           + cc.SoGioOT * 50000
                           AS DECIMAL(18,2)
                       )
                FROM BangChamCong cc WITH (UPDLOCK, HOLDLOCK)
                INNER JOIN NhanVien nv
                    ON nv.MaNV=cc.MaNV
                WHERE cc.ThangNam=@ThangNam
                """, conn, trans);

            cmd.Parameters.Add("@ThangNam",
                SqlDbType.VarChar, 7).Value = thangNam;

            int soNhanVien = cmd.ExecuteNonQuery();

            if (soNhanVien == 0)
                throw new Exception(
                    "Không có dữ liệu chấm công để chốt.");

            trans.Commit();
            return soNhanVien;
        }
        catch
        {
            trans.Rollback();
            throw;
        }
    }
}