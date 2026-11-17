
using System;
using System.Data;
using Microsoft.Data.SqlClient;

namespace Bai6._1.DAL
{
    public class ThongKeDAL
    {
        public decimal DoanhThuNgay(DateTime ngay)
        {
            return LayDoanhThu(ngay.Date, ngay.Date.AddDays(1));
        }

        public decimal DoanhThuThang(int nam, int thang)
        {
            DateTime dauThang = new DateTime(nam, thang, 1);
            return LayDoanhThu(dauThang, dauThang.AddMonths(1));
        }

        private decimal LayDoanhThu(DateTime tu, DateTime den)
        {
            using var conn = DatabaseHelper.GetConnection();
            using var cmd = new SqlCommand(@"
                SELECT ISNULL(SUM(TongTien), 0)
                FROM HoaDon
                WHERE NgayLap >= @Tu AND NgayLap < @Den", conn);

            cmd.Parameters.Add("@Tu", SqlDbType.DateTime).Value = tu;
            cmd.Parameters.Add("@Den", SqlDbType.DateTime).Value = den;

            conn.Open();
            return Convert.ToDecimal(cmd.ExecuteScalar());
        }

        public DataTable Top5SanPham()
        {
            using var conn = DatabaseHelper.GetConnection();
            using var cmd = new SqlCommand(@"
                SELECT TOP 5
                    sp.MaSP,
                    sp.TenSP,
                    SUM(ct.SoLuong) AS TongSoLuongBan,
                    SUM(ct.SoLuong * ct.DonGia) AS DoanhThu
                FROM ChiTietHoaDon ct
                INNER JOIN SanPham sp ON ct.MaSP = sp.MaSP
                GROUP BY sp.MaSP, sp.TenSP
                ORDER BY TongSoLuongBan DESC, sp.MaSP", conn);

            var dt = new DataTable();
            dt.Columns.Add("MaSP", typeof(string));
            dt.Columns.Add("TenSP", typeof(string));
            dt.Columns.Add("TongSoLuongBan", typeof(long));
            dt.Columns.Add("DoanhThu", typeof(decimal));

            conn.Open();

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                dt.Rows.Add(
                    reader.GetString(0),
                    reader.GetString(1),
                    Convert.ToInt64(reader.GetValue(2)),
                    reader.GetDecimal(3));
            }

            return dt;
        }
    }
}
