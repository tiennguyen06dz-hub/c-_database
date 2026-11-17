
using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using Bai6._1.DTO;

namespace Bai6._1.DAL
{
    public class BanHangDAL
    {
        public int ThanhToan(
            int maNV, int? maKH, List<GioHangItem> gioHang)
        {
            using var conn = DatabaseHelper.GetConnection();
            conn.Open();

            using var trans = conn.BeginTransaction(
                IsolationLevel.ReadCommitted);

            try
            {
                decimal tongTien = 0;
                int maHD;

                // 1. Tạo hóa đơn
                using (var cmd = new SqlCommand(@"
                    INSERT INTO HoaDon(MaNV, MaKH, TongTien)
                    OUTPUT INSERTED.MaHD
                    VALUES(@MaNV, @MaKH, 0)", conn, trans))
                {
                    cmd.Parameters.Add("@MaNV", SqlDbType.Int)
                        .Value = maNV;
                    cmd.Parameters.Add("@MaKH", SqlDbType.Int)
                        .Value = maKH.HasValue
                            ? maKH.Value : DBNull.Value;

                    maHD = Convert.ToInt32(cmd.ExecuteScalar());
                }

                foreach (var item in gioHang)
                {
                    decimal giaThucTe;

                    // 2. Đọc giá hiện tại từ database
                    using (var cmd = new SqlCommand(@"
                        SELECT GiaBan
                        FROM SanPham
                        WHERE MaSP=@MaSP", conn, trans))
                    {
                        cmd.Parameters.Add("@MaSP",
                            SqlDbType.VarChar, 20).Value = item.MaSP;

                        object result = cmd.ExecuteScalar();

                        if (result == null || result == DBNull.Value)
                            throw new Exception(
                                "Sản phẩm không tồn tại: " + item.MaSP);

                        giaThucTe = Convert.ToDecimal(result);
                    }

                    // 3. Trừ tồn kho an toàn
                    using (var cmd = new SqlCommand(@"
                        UPDATE SanPham
                        SET SoLuongTon = SoLuongTon - @SoLuong
                        WHERE MaSP=@MaSP
                          AND SoLuongTon >= @SoLuong",
                        conn, trans))
                    {
                        cmd.Parameters.Add("@MaSP",
                            SqlDbType.VarChar, 20).Value = item.MaSP;
                        cmd.Parameters.Add("@SoLuong",
                            SqlDbType.Int).Value = item.SoLuong;

                        if (cmd.ExecuteNonQuery() != 1)
                            throw new Exception(
                                "Không đủ tồn kho: " + item.MaSP);
                    }

                    // 4. Lưu chi tiết hóa đơn
                    using (var cmd = new SqlCommand(@"
                        INSERT INTO ChiTietHoaDon
                        (MaHD, MaSP, SoLuong, DonGia)
                        VALUES(@MaHD, @MaSP, @SoLuong, @DonGia)",
                        conn, trans))
                    {
                        cmd.Parameters.Add("@MaHD",
                            SqlDbType.Int).Value = maHD;
                        cmd.Parameters.Add("@MaSP",
                            SqlDbType.VarChar, 20).Value = item.MaSP;
                        cmd.Parameters.Add("@SoLuong",
                            SqlDbType.Int).Value = item.SoLuong;

                        var p = cmd.Parameters.Add(
                            "@DonGia", SqlDbType.Decimal);
                        p.Precision = 18;
                        p.Scale = 2;
                        p.Value = giaThucTe;

                        cmd.ExecuteNonQuery();
                    }

                    tongTien += item.SoLuong * giaThucTe;
                }

                // 5. Cập nhật tổng tiền
                using (var cmd = new SqlCommand(@"
                    UPDATE HoaDon SET TongTien=@TongTien
                    WHERE MaHD=@MaHD", conn, trans))
                {
                    var p = cmd.Parameters.Add(
                        "@TongTien", SqlDbType.Decimal);
                    p.Precision = 18;
                    p.Scale = 2;
                    p.Value = tongTien;

                    cmd.Parameters.Add("@MaHD",
                        SqlDbType.Int).Value = maHD;
                    cmd.ExecuteNonQuery();
                }

                // 6. Cộng điểm tích lũy
                if (maKH.HasValue)
                {
                    int diem = checked((int)decimal.Truncate(
                        tongTien / 100000m));

                    using var cmd = new SqlCommand(@"
                        UPDATE KhachHang
                        SET DiemTichLuy = DiemTichLuy + @Diem
                        WHERE MaKH=@MaKH", conn, trans);

                    cmd.Parameters.Add("@Diem",
                        SqlDbType.Int).Value = diem;
                    cmd.Parameters.Add("@MaKH",
                        SqlDbType.Int).Value = maKH.Value;

                    if (cmd.ExecuteNonQuery() != 1)
                        throw new Exception(
                            "Không tìm thấy khách hàng.");
                }

                // 7. Hoàn tất giao dịch
                trans.Commit();
                return maHD;
            }
            catch
            {
                trans.Rollback();
                throw;
            }
        }
    }
}
