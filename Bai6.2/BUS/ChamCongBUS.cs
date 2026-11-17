using System;
using System.Data;
using Bai6_2.DAL;

namespace Bai6_2.BUS;

public class ChamCongBUS
{
    private readonly ChamCongDAL dal = new();

    public DataTable NapChamCong(string thangNam)
    {
        KiemTraThang(thangNam);
        return dal.NapChamCong(thangNam);
    }

    public void LuuChamCong(string thangNam)
    {
        KiemTraThang(thangNam);
        dal.LuuChamCong(thangNam);
    }

    public bool DaChot(string thangNam)
        => dal.DaChot(thangNam);

    private static void KiemTraThang(string thangNam)
    {
        if (!DateTime.TryParseExact(
            thangNam, "MM/yyyy",
            System.Globalization.CultureInfo.InvariantCulture,
            System.Globalization.DateTimeStyles.None,
            out _))
            throw new Exception("Tháng/năm không hợp lệ.");
    }
}