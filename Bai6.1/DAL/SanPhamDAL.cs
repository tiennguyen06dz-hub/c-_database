
using System;
using System.Data;
using Microsoft.Data.SqlClient;
using Bai6._1.DTO;

namespace Bai6._1.DAL
{
    public class SanPhamDAL
    {
        // 1. Lấy danh sách và tìm kiếm sản phẩm
        public DataTable LayDanhSach(string tuKhoa = "")
        {
            using var conn = DatabaseHelper.GetConnection();

            string sql = @"
                SELECT MaSP, TenSP, GiaBan, SoLuongTon
                FROM SanPham
                WHERE MaSP LIKE @TuKhoa
                   OR TenSP LIKE @TuKhoa
                ORDER BY MaSP";

            using var cmd = new SqlCommand(sql, conn);

            cmd.Parameters.Add(
                "@TuKhoa", SqlDbType.NVarChar, 200
            ).Value = "%" + tuKhoa + "%";

            DataTable dt = new DataTable();

            using var adapter = new SqlDataAdapter(cmd);
            adapter.Fill(dt);

            return dt;
        }

        // 2. Thêm sản phẩm
        public void Them(SanPhamDTO sp)
        {
            using var conn = DatabaseHelper.GetConnection();

            string sql = @"
                INSERT INTO SanPham
                (MaSP, TenSP, GiaBan, SoLuongTon)
                VALUES
                (@MaSP, @TenSP, @GiaBan, @SoLuongTon)";

            using var cmd = new SqlCommand(sql, conn);

            GanThamSo(cmd, sp);

            conn.Open();
            cmd.ExecuteNonQuery();
        }

        // 3. Sửa sản phẩm
        public void Sua(SanPhamDTO sp)
        {
            using var conn = DatabaseHelper.GetConnection();

            string sql = @"
                UPDATE SanPham
                SET TenSP = @TenSP,
                    GiaBan = @GiaBan,
                    SoLuongTon = @SoLuongTon
                WHERE MaSP = @MaSP";

            using var cmd = new SqlCommand(sql, conn);

            GanThamSo(cmd, sp);

            conn.Open();

            int ketQua = cmd.ExecuteNonQuery();

            if (ketQua == 0)
                throw new Exception(
                    "Không tìm thấy sản phẩm cần sửa!");
        }

        // 4. Xóa sản phẩm
        public void Xoa(string maSP)
        {
            using var conn = DatabaseHelper.GetConnection();

            string sql =
                "DELETE FROM SanPham WHERE MaSP = @MaSP";

            using var cmd = new SqlCommand(sql, conn);

            cmd.Parameters.Add(
                "@MaSP", SqlDbType.VarChar, 20
            ).Value = maSP;

            conn.Open();

            int ketQua = cmd.ExecuteNonQuery();

            if (ketQua == 0)
                throw new Exception(
                    "Không tìm thấy sản phẩm cần xóa!");
        }

        // 5. Hàm dùng chung để truyền SqlParameter
        private void GanThamSo(
            SqlCommand cmd, SanPhamDTO sp)
        {
            cmd.Parameters.Add(
                "@MaSP", SqlDbType.VarChar, 20
            ).Value = sp.MaSP;

            cmd.Parameters.Add(
                "@TenSP", SqlDbType.NVarChar, 150
            ).Value = sp.TenSP;

            var gia = cmd.Parameters.Add(
                "@GiaBan", SqlDbType.Decimal);

            gia.Precision = 18;
            gia.Scale = 2;
            gia.Value = sp.GiaBan;

            cmd.Parameters.Add(
                "@SoLuongTon", SqlDbType.Int
            ).Value = sp.SoLuongTon;
        }
    }
}
