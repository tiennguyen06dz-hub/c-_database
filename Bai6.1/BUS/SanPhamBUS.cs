
using System;
using System.Data;
using Bai6._1.DAL;
using Bai6._1.DTO;

namespace Bai6._1.BUS
{
    public class SanPhamBUS
    {
        private readonly SanPhamDAL dal = new SanPhamDAL();

        public DataTable LayDanhSach(string tuKhoa = "")
        {
            return dal.LayDanhSach(tuKhoa);
        }

        public void Them(SanPhamDTO sp)
        {
            KiemTra(sp);
            dal.Them(sp);
        }

        public void Sua(SanPhamDTO sp)
        {
            KiemTra(sp);
            dal.Sua(sp);
        }

        public void Xoa(string maSP)
        {
            if (string.IsNullOrWhiteSpace(maSP))
                throw new Exception("Vui lòng chọn sản phẩm!");

            dal.Xoa(maSP);
        }

        private void KiemTra(SanPhamDTO sp)
        {
            if (string.IsNullOrWhiteSpace(sp.MaSP))
                throw new Exception("Mã sản phẩm không được trống!");

            if (string.IsNullOrWhiteSpace(sp.TenSP))
                throw new Exception("Tên sản phẩm không được trống!");

            if (sp.GiaBan <= 0)
                throw new Exception("Giá bán phải lớn hơn 0!");

            if (sp.SoLuongTon < 0)
                throw new Exception("Số lượng tồn không hợp lệ!");
        }
    }
}
