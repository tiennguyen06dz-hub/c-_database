using System;
using System.Data;
using Bai6_2.DAL;
using Bai6_2.DTO;

namespace Bai6_2.BUS;

public class NhanVienBUS
{
    private readonly NhanVienDAL dal = new();

    public DataTable TimKiem(string tuKhoa)
        => dal.TimKiem(tuKhoa);

    private static void KiemTra(NhanVienDTO nv)
    {
        if (string.IsNullOrWhiteSpace(nv.HoTen))
            throw new Exception("Họ tên không được để trống.");

        if (nv.HoTen.Length > 100 ||
            nv.CCCD.Length > 20)
            throw new Exception("Thông tin vượt độ dài cho phép.");

        if (nv.LuongCoBan <= 0)
            throw new Exception("Lương cơ bản phải lớn hơn 0.");

        if (double.IsNaN(nv.HeSoLuong) ||
            double.IsInfinity(nv.HeSoLuong) ||
            nv.HeSoLuong <= 0)
            throw new Exception("Hệ số lương không hợp lệ.");
    }

    public void Them(NhanVienDTO nv)
    {
        KiemTra(nv);
        dal.Them(nv);
    }

    public void Sua(NhanVienDTO nv)
    {
        if (nv.MaNV <= 0)
            throw new Exception("Chưa chọn nhân viên.");

        KiemTra(nv);
        dal.Sua(nv);
    }

    public void Xoa(int maNV)
    {
        if (maNV <= 0)
            throw new Exception("Chưa chọn nhân viên.");

        dal.Xoa(maNV);
    }
}