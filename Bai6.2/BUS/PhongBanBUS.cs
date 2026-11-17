using System.Data;
using Bai6_2.DAL;

namespace Bai6_2.BUS;

public class PhongBanBUS
{
    private readonly PhongBanDAL dal = new();

    public DataTable LayDanhSach()
        => dal.LayDanhSach();
}