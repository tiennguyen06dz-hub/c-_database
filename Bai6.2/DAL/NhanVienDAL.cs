using System;
using System.Data;
using Microsoft.Data.SqlClient;
using Bai6_2.DTO;

namespace Bai6_2.DAL;

public class NhanVienDAL
{
    public DataTable TimKiem(string tuKhoa)
    {
        using var conn = DatabaseHelper.GetConnection();

        const string sql = """
            SELECT nv.MaNV, nv.HoTen, nv.NgaySinh,
                   nv.CCCD, nv.MaPB, pb.TenPB,
                   nv.LuongCoBan, nv.HeSoLuong
            FROM NhanVien nv
            LEFT JOIN PhongBan pb ON nv.MaPB = pb.MaPB
            WHERE nv.HoTen LIKE @TuKhoa
               OR nv.CCCD LIKE @TuKhoa
               OR pb.TenPB LIKE @TuKhoa
            ORDER BY nv.MaNV
            """;

        using var cmd = new SqlCommand(sql, conn);
        cmd.Parameters.Add("@TuKhoa", SqlDbType.NVarChar, 200)
            .Value = "%" + tuKhoa + "%";

        using var adapter = new SqlDataAdapter(cmd);
        var dt = new DataTable();
        adapter.Fill(dt);
        return dt;
    }

    private static void GanThamSo(
        SqlCommand cmd, NhanVienDTO nv)
    {
        cmd.Parameters.Add("@MaNV", SqlDbType.Int)
            .Value = nv.MaNV;

        cmd.Parameters.Add("@HoTen", SqlDbType.NVarChar, 100)
            .Value = nv.HoTen;

        cmd.Parameters.Add("@NgaySinh", SqlDbType.DateTime)
            .Value = nv.NgaySinh;

        cmd.Parameters.Add("@CCCD", SqlDbType.VarChar, 20)
            .Value = string.IsNullOrWhiteSpace(nv.CCCD)
                ? DBNull.Value : nv.CCCD;

        cmd.Parameters.Add("@MaPB", SqlDbType.Int)
            .Value = nv.MaPB.HasValue
                ? nv.MaPB.Value : DBNull.Value;

        var luong = cmd.Parameters.Add(
            "@LuongCoBan", SqlDbType.Decimal);
        luong.Precision = 18;
        luong.Scale = 2;
        luong.Value = nv.LuongCoBan;

        cmd.Parameters.Add("@HeSoLuong", SqlDbType.Float)
            .Value = nv.HeSoLuong;
    }

    public void Them(NhanVienDTO nv)
    {
        using var conn = DatabaseHelper.GetConnection();
        using var cmd = new SqlCommand("""
            INSERT INTO NhanVien
            (HoTen, NgaySinh, CCCD, MaPB, LuongCoBan, HeSoLuong)
            VALUES
            (@HoTen, @NgaySinh, @CCCD, @MaPB,
             @LuongCoBan, @HeSoLuong)
            """, conn);

        GanThamSo(cmd, nv);
        conn.Open();
        cmd.ExecuteNonQuery();
    }

    public void Sua(NhanVienDTO nv)
    {
        using var conn = DatabaseHelper.GetConnection();
        using var cmd = new SqlCommand("""
            UPDATE NhanVien
            SET HoTen=@HoTen,
                NgaySinh=@NgaySinh,
                CCCD=@CCCD,
                MaPB=@MaPB,
                LuongCoBan=@LuongCoBan,
                HeSoLuong=@HeSoLuong
            WHERE MaNV=@MaNV
            """, conn);

        GanThamSo(cmd, nv);
        conn.Open();

        if (cmd.ExecuteNonQuery() != 1)
            throw new Exception("Không tìm thấy nhân viên.");
    }

    public void Xoa(int maNV)
    {
        using var conn = DatabaseHelper.GetConnection();
        using var cmd = new SqlCommand(
            "DELETE FROM NhanVien WHERE MaNV=@MaNV", conn);

        cmd.Parameters.Add("@MaNV", SqlDbType.Int)
            .Value = maNV;

        conn.Open();

        if (cmd.ExecuteNonQuery() != 1)
            throw new Exception("Không tìm thấy nhân viên.");
    }
}