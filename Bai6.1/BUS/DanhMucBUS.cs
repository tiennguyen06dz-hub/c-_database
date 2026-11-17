
using System;
using System.Data;
using Bai6._1.DAL;
using Bai6._1.DTO;

namespace Bai6._1.BUS
{
    public class DanhMucBUS
    {
        private readonly DanhMucDAL dal = new DanhMucDAL();

        public DataTable LayKhachHang() => dal.LayKhachHang();
        public DataTable LayNhanVien() => dal.LayNhanVien();

        public void LuuKhachHang(KhachHangDTO kh)
        {
            if (string.IsNullOrWhiteSpace(kh.TenKH))
                throw new Exception("Chưa nhập tên khách hàng.");
            if (kh.TenKH.Length > 100 ||
                kh.DienThoai.Length > 15)
                throw new Exception("Thông tin khách hàng quá dài.");

            dal.LuuKhachHang(kh);
        }

        public void XoaKhachHang(int ma)
        {
            if (ma <= 0)
                throw new Exception("Chưa chọn khách hàng.");
            dal.XoaKhachHang(ma);
        }

        public void LuuNhanVien(NhanVienDTO nv)
        {
            if (string.IsNullOrWhiteSpace(nv.TenNV))
                throw new Exception("Chưa nhập tên nhân viên.");
            if (nv.TenNV.Length > 100 ||
                nv.SoDienThoai.Length > 15 ||
                nv.ChucVu.Length > 50)
                throw new Exception("Thông tin nhân viên quá dài.");

            dal.LuuNhanVien(nv);
        }

        public void XoaNhanVien(int ma)
        {
            if (ma <= 0)
                throw new Exception("Chưa chọn nhân viên.");
            dal.XoaNhanVien(ma);
        }
    }
}
