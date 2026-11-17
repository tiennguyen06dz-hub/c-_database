
namespace Bai6._1.DTO
{
    public class GioHangItem
    {
        public string MaSP { get; set; } = "";
        public string TenSP { get; set; } = "";
        public int SoLuong { get; set; }
        public decimal DonGia { get; set; }

        public decimal ThanhTien => SoLuong * DonGia;
    }
}
