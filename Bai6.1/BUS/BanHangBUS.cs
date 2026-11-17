
using System;
using System.Collections.Generic;
using System.Linq;
using Bai6._1.DAL;
using Bai6._1.DTO;

namespace Bai6._1.BUS
{
    public class BanHangBUS
    {
        private readonly BanHangDAL dal = new BanHangDAL();

        public decimal TinhTongTien(List<GioHangItem> gio)
        {
            return gio.Sum(x => x.ThanhTien);
        }

        public int TinhDiem(List<GioHangItem> gio)
        {
            return checked((int)decimal.Truncate(
                TinhTongTien(gio) / 100000m));
        }

        public int ThanhToan(
            int maNV, int? maKH, List<GioHangItem> gio)
        {
            if (maNV <= 0)
                throw new Exception("Vui lòng chọn nhân viên.");

            if (gio == null || gio.Count == 0)
                throw new Exception("Giỏ hàng đang trống.");

            if (gio.Any(x =>
                string.IsNullOrWhiteSpace(x.MaSP) ||
                x.SoLuong <= 0 ||
                x.DonGia <= 0))
                throw new Exception("Giỏ hàng không hợp lệ.");

            if (gio.GroupBy(x => x.MaSP)
                .Any(g => g.Count() > 1))
                throw new Exception("Giỏ hàng có mã sản phẩm trùng.");

            return dal.ThanhToan(maNV, maKH, gio);
        }
    }
}
