
using System;
using System.Data;
using Microsoft.Data.SqlClient;
using Bai6._1.DTO;

namespace Bai6._1.DAL
{
    public class DanhMucDAL
    {
        public DataTable LayKhachHang()
        {
            return LayBang(
                "SELECT MaKH, TenKH, DienThoai, DiemTichLuy FROM KhachHang ORDER BY MaKH");
        }

        public DataTable LayNhanVien()
        {
            return LayBang(
                "SELECT MaNV, TenNV, SoDienThoai, ChucVu FROM NhanVien ORDER BY MaNV");
        }

        private DataTable LayBang(string sql)
        {
            using var conn = DatabaseHelper.GetConnection();
            using var cmd = new SqlCommand(sql, conn);
            using var adapter = new SqlDataAdapter(cmd);
            var dt = new DataTable();
            adapter.Fill(dt);
            return dt;
        }

        public void LuuKhachHang(KhachHangDTO kh)
        {
            string sql = kh.MaKH == 0
                ? @"INSERT INTO KhachHang(TenKH, DienThoai)
                    VALUES(@Ten, @SDT)"
                : @"UPDATE KhachHang SET TenKH=@Ten,
                    DienThoai=@SDT WHERE MaKH=@Ma";

            using var conn = DatabaseHelper.GetConnection();
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.Add("@Ten", SqlDbType.NVarChar, 100)
                .Value = kh.TenKH;
            cmd.Parameters.Add("@SDT", SqlDbType.VarChar, 15)
                .Value = string.IsNullOrWhiteSpace(kh.DienThoai)
                    ? DBNull.Value : kh.DienThoai;
            cmd.Parameters.Add("@Ma", SqlDbType.Int).Value = kh.MaKH;
            conn.Open();
            if (cmd.ExecuteNonQuery() == 0)
                throw new Exception("Không tìm thấy khách hàng.");
        }

        public void XoaKhachHang(int ma)
        {
            Xoa("DELETE FROM KhachHang WHERE MaKH=@Ma", ma);
        }

        public void LuuNhanVien(NhanVienDTO nv)
        {
            string sql = nv.MaNV == 0
                ? @"INSERT INTO NhanVien(TenNV, SoDienThoai, ChucVu)
                    VALUES(@Ten, @SDT, @ChucVu)"
                : @"UPDATE NhanVien SET TenNV=@Ten,
                    SoDienThoai=@SDT, ChucVu=@ChucVu
                    WHERE MaNV=@Ma";

            using var conn = DatabaseHelper.GetConnection();
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.Add("@Ten", SqlDbType.NVarChar, 100)
                .Value = nv.TenNV;
            cmd.Parameters.Add("@SDT", SqlDbType.VarChar, 15)
                .Value = string.IsNullOrWhiteSpace(nv.SoDienThoai)
                    ? DBNull.Value : nv.SoDienThoai;
            cmd.Parameters.Add("@ChucVu", SqlDbType.NVarChar, 50)
                .Value = string.IsNullOrWhiteSpace(nv.ChucVu)
                    ? DBNull.Value : nv.ChucVu;
            cmd.Parameters.Add("@Ma", SqlDbType.Int).Value = nv.MaNV;
            conn.Open();
            if (cmd.ExecuteNonQuery() == 0)
                throw new Exception("Không tìm thấy nhân viên.");
        }

        public void XoaNhanVien(int ma)
        {
            Xoa("DELETE FROM NhanVien WHERE MaNV=@Ma", ma);
        }

        private void Xoa(string sql, int ma)
        {
            using var conn = DatabaseHelper.GetConnection();
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.Add("@Ma", SqlDbType.Int).Value = ma;
            conn.Open();
            if (cmd.ExecuteNonQuery() == 0)
                throw new Exception("Không tìm thấy bản ghi.");
        }
    }
}
