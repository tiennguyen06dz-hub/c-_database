using System;
using System.Data;
using Bai6_2.DAL;

namespace Bai6_2.BUS;

public class BangLuongBUS
{
    private readonly BangLuongDAL dal = new();

    public DataTable XemLuong(string thangNam)
        => dal.XemLuong(thangNam);

    public DataTable XemBangLuongDaChot(string thangNam)
        => dal.XemBangLuongDaChot(thangNam);

    public int ChotLuong(string thangNam)
    {
        if (string.IsNullOrWhiteSpace(thangNam))
            throw new Exception("Chưa chọn tháng.");

        return dal.ChotLuong(thangNam);
    }
}