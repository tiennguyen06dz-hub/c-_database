
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Bai6._1.BUS;
using Bai6._1.DAL;
using Bai6._1.DTO;

namespace Bai6._1.GUI
{
    public class RetailForm : Form
    {
        private readonly SanPhamBUS spBUS = new();
        private readonly DanhMucBUS dmBUS = new();
        private readonly BanHangBUS bhBUS = new();
        private readonly ThongKeDAL tkDAL = new();

        private readonly List<GioHangItem> gioHang = new();

        private DataGridView dgvSP = new();
        private DataGridView dgvKH = new();
        private DataGridView dgvNV = new();
        private DataGridView dgvGio = new();
        private DataGridView dgvTop = new();

        private TextBox txtMaSP = new();
        private TextBox txtTenSP = new();
        private TextBox txtGia = new();
        private TextBox txtTon = new();
        private TextBox txtTim = new();

        private TextBox txtMaKH = new();
        private TextBox txtTenKH = new();
        private TextBox txtSDTKH = new();

        private TextBox txtMaNV = new();
        private TextBox txtTenNV = new();
        private TextBox txtSDTNV = new();
        private TextBox txtChucVu = new();

        private ComboBox cboSP = new();
        private ComboBox cboKH = new();
        private ComboBox cboNV = new();
        private NumericUpDown numSL = new();

        private Label lblTong = new();
        private Label lblDiem = new();
        private Label lblNgay = new();
        private Label lblThang = new();

        private DateTimePicker dtpNgay = new();
        private DateTimePicker dtpThang = new();

        public RetailForm()
        {
            Text = "HỆ THỐNG QUẢN LÝ BÁN HÀNG - SALESDB";
            Size = new Size(1150, 750);
            MinimumSize = new Size(950, 650);
            StartPosition = FormStartPosition.CenterScreen;
            Font = new Font("Segoe UI", 10);

            var tabs = new TabControl { Dock = DockStyle.Fill };
            Controls.Add(tabs);

            TaoTabSanPham(tabs);
            TaoTabKhachHang(tabs);
            TaoTabNhanVien(tabs);
            TaoTabBanHang(tabs);
            TaoTabThongKe(tabs);

            try
            {
                TaiTatCa();
            }
            catch (Exception ex)
            {
                BaoLoi(ex);
            }
        }

        private TextBox ONhap(
            Control parent, string nhan, int x, int y,
            int width = 200)
        {
            parent.Controls.Add(new Label
            {
                Text = nhan,
                Location = new Point(x, y),
                AutoSize = true
            });

            var txt = new TextBox
            {
                Location = new Point(x, y + 25),
                Width = width
            };

            parent.Controls.Add(txt);
            return txt;
        }

        private Button Nut(
            Control parent, string text, int x, int y,
            Action action, int width = 115)
        {
            var btn = new Button
            {
                Text = text,
                Location = new Point(x, y),
                Size = new Size(width, 38),
                BackColor = Color.FromArgb(35, 105, 180),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };

            btn.Click += (s, e) =>
            {
                try
                {
                    action();
                }
                catch (Exception ex)
                {
                    BaoLoi(ex);
                }
            };

            parent.Controls.Add(btn);
            return btn;
        }

        private DataGridView Bang(Control parent, int y)
        {
            var dgv = new DataGridView
            {
                Location = new Point(20, y),
                Width = 1050,
                Height = 360,
                Anchor = AnchorStyles.Top |
                         AnchorStyles.Bottom |
                         AnchorStyles.Left |
                         AnchorStyles.Right,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                SelectionMode =
                    DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoSizeColumnsMode =
                    DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White
            };

            parent.Controls.Add(dgv);
            return dgv;
        }

        private TabPage Tab(TabControl tabs, string ten)
        {
            var page = new TabPage(ten)
            {
                AutoScroll = true,
                BackColor = Color.White,
                Padding = new Padding(10)
            };
            tabs.TabPages.Add(page);
            return page;
        }

        private static string Gia(decimal n)
        {
            return n.ToString("N0") + " VNĐ";
        }

        private void BaoLoi(Exception ex)
        {
            MessageBox.Show(
                ex.Message,
                "Thông báo",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
        }

        private void ThongBao(string text)
        {
            MessageBox.Show(text, "Thành công",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        private bool XacNhanXoa()
        {
            return MessageBox.Show(
                "Bạn có chắc muốn xóa bản ghi này?",
                "Xác nhận",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question) == DialogResult.Yes;
        }

        // ============================
        // TAB QUẢN LÝ SẢN PHẨM
        // ============================

        private void TaoTabSanPham(TabControl tabs)
        {
            var p = Tab(tabs, "Sản phẩm");

            txtMaSP = ONhap(p, "Mã sản phẩm", 20, 20);
            txtTenSP = ONhap(p, "Tên sản phẩm", 250, 20, 280);
            txtGia = ONhap(p, "Giá bán", 560, 20);
            txtTon = ONhap(p, "Số lượng tồn", 790, 20);

            Nut(p, "Thêm", 20, 105, ThemSP);
            Nut(p, "Sửa", 145, 105, SuaSP);
            Nut(p, "Xóa", 270, 105, XoaSP);
            Nut(p, "Làm mới", 395, 105, () =>
            {
                txtMaSP.Clear();
                txtTenSP.Clear();
                txtGia.Clear();
                txtTon.Clear();
                TaiSanPham();
            });

            txtTim = ONhap(p, "Tìm mã / tên sản phẩm",
                20, 165, 380);

            Nut(p, "Tìm kiếm", 420, 188,
                () => dgvSP.DataSource =
                    spBUS.LayDanhSach(txtTim.Text));

            dgvSP = Bang(p, 255);

            dgvSP.SelectionChanged += (s, e) =>
            {
                if (dgvSP.CurrentRow?.DataBoundItem
                    is DataRowView row)
                {
                    txtMaSP.Text = row["MaSP"].ToString();
                    txtTenSP.Text = row["TenSP"].ToString();
                    txtGia.Text = row["GiaBan"].ToString();
                    txtTon.Text = row["SoLuongTon"].ToString();
                }
            };
        }

        private SanPhamDTO DocSP()
        {
            if (!decimal.TryParse(txtGia.Text, out decimal gia))
                throw new Exception("Giá bán không hợp lệ.");

            if (!int.TryParse(txtTon.Text, out int ton))
                throw new Exception("Số lượng tồn không hợp lệ.");

            return new SanPhamDTO
            {
                MaSP = txtMaSP.Text.Trim(),
                TenSP = txtTenSP.Text.Trim(),
                GiaBan = gia,
                SoLuongTon = ton
            };
        }

        private void ThemSP()
        {
            spBUS.Them(DocSP());
            TaiSanPham();
            ThongBao("Đã thêm sản phẩm.");
        }

        private void SuaSP()
        {
            spBUS.Sua(DocSP());
            TaiSanPham();
            ThongBao("Đã sửa sản phẩm.");
        }

        private void XoaSP()
        {
            if (!XacNhanXoa()) return;
            spBUS.Xoa(txtMaSP.Text.Trim());
            TaiSanPham();
            ThongBao("Đã xóa sản phẩm.");
        }

        private void TaiSanPham()
        {
            dgvSP.DataSource = spBUS.LayDanhSach();
            TaiComboSanPham();
        }

        // ============================
        // TAB QUẢN LÝ KHÁCH HÀNG
        // ============================

        private void TaoTabKhachHang(TabControl tabs)
        {
            var p = Tab(tabs, "Khách hàng");

            txtMaKH = ONhap(p, "Mã KH (để trống khi thêm)",
                20, 20);
            txtMaKH.ReadOnly = true;

            txtTenKH = ONhap(p, "Tên khách hàng", 250, 20, 280);
            txtSDTKH = ONhap(p, "Điện thoại", 560, 20);

            Nut(p, "Thêm", 20, 105, () => LuuKH(true));
            Nut(p, "Sửa", 145, 105, () => LuuKH(false));
            Nut(p, "Xóa", 270, 105, () =>
            {
                if (!XacNhanXoa()) return;
                dmBUS.XoaKhachHang(int.Parse(txtMaKH.Text));
                TaiKhachHang();
                ThongBao("Đã xóa khách hàng.");
            });
            Nut(p, "Làm mới", 395, 105, () =>
            {
                txtMaKH.Clear();
                txtTenKH.Clear();
                txtSDTKH.Clear();
                TaiKhachHang();
            });

            dgvKH = Bang(p, 180);

            dgvKH.SelectionChanged += (s, e) =>
            {
                if (dgvKH.CurrentRow?.DataBoundItem
                    is DataRowView row)
                {
                    txtMaKH.Text = row["MaKH"].ToString();
                    txtTenKH.Text = row["TenKH"].ToString();
                    txtSDTKH.Text = row["DienThoai"].ToString();
                }
            };
        }

        private void LuuKH(bool them)
        {
            int ma = 0;
            if (!them && !int.TryParse(txtMaKH.Text, out ma))
                throw new Exception("Vui lòng chọn khách hàng.");

            dmBUS.LuuKhachHang(new KhachHangDTO
            {
                MaKH = ma,
                TenKH = txtTenKH.Text.Trim(),
                DienThoai = txtSDTKH.Text.Trim()
            });

            TaiKhachHang();
            ThongBao(them ? "Đã thêm khách hàng." :
                           "Đã sửa khách hàng.");
        }

        private void TaiKhachHang()
        {
            dgvKH.DataSource = dmBUS.LayKhachHang();
            TaiComboKhachHang();
        }

        // ============================
        // TAB QUẢN LÝ NHÂN VIÊN
        // ============================

        private void TaoTabNhanVien(TabControl tabs)
        {
            var p = Tab(tabs, "Nhân viên");

            txtMaNV = ONhap(p, "Mã NV (tự tăng)", 20, 20);
            txtMaNV.ReadOnly = true;

            txtTenNV = ONhap(p, "Tên nhân viên", 250, 20);
            txtSDTNV = ONhap(p, "Điện thoại", 480, 20);
            txtChucVu = ONhap(p, "Chức vụ", 710, 20);

            Nut(p, "Thêm", 20, 105, () => LuuNV(true));
            Nut(p, "Sửa", 145, 105, () => LuuNV(false));
            Nut(p, "Xóa", 270, 105, () =>
            {
                if (!XacNhanXoa()) return;
                dmBUS.XoaNhanVien(int.Parse(txtMaNV.Text));
                TaiNhanVien();
                ThongBao("Đã xóa nhân viên.");
            });
            Nut(p, "Làm mới", 395, 105, () =>
            {
                txtMaNV.Clear();
                txtTenNV.Clear();
                txtSDTNV.Clear();
                txtChucVu.Clear();
                TaiNhanVien();
            });

            dgvNV = Bang(p, 180);

            dgvNV.SelectionChanged += (s, e) =>
            {
                if (dgvNV.CurrentRow?.DataBoundItem
                    is DataRowView row)
                {
                    txtMaNV.Text = row["MaNV"].ToString();
                    txtTenNV.Text = row["TenNV"].ToString();
                    txtSDTNV.Text = row["SoDienThoai"].ToString();
                    txtChucVu.Text = row["ChucVu"].ToString();
                }
            };
        }

        private void LuuNV(bool them)
        {
            int ma = 0;
            if (!them && !int.TryParse(txtMaNV.Text, out ma))
                throw new Exception("Vui lòng chọn nhân viên.");

            dmBUS.LuuNhanVien(new NhanVienDTO
            {
                MaNV = ma,
                TenNV = txtTenNV.Text.Trim(),
                SoDienThoai = txtSDTNV.Text.Trim(),
                ChucVu = txtChucVu.Text.Trim()
            });

            TaiNhanVien();
            ThongBao(them ? "Đã thêm nhân viên." :
                           "Đã sửa nhân viên.");
        }

        private void TaiNhanVien()
        {
            dgvNV.DataSource = dmBUS.LayNhanVien();
            TaiComboNhanVien();
        }

        // ============================
        // TAB BÁN HÀNG
        // ============================

        private void TaoTabBanHang(TabControl tabs)
        {
            var p = Tab(tabs, "Bán hàng");

            p.Controls.Add(new Label
            {
                Text = "Nhân viên bán hàng",
                Location = new Point(20, 20),
                AutoSize = true
            });
            cboNV = new ComboBox
            {
                Location = new Point(20, 45),
                Width = 240,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            p.Controls.Add(cboNV);

            p.Controls.Add(new Label
            {
                Text = "Khách hàng",
                Location = new Point(290, 20),
                AutoSize = true
            });
            cboKH = new ComboBox
            {
                Location = new Point(290, 45),
                Width = 260,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            p.Controls.Add(cboKH);

            p.Controls.Add(new Label
            {
                Text = "Sản phẩm / Mã vạch",
                Location = new Point(20, 100),
                AutoSize = true
            });
            cboSP = new ComboBox
            {
                Location = new Point(20, 125),
                Width = 400,
                DropDownStyle = ComboBoxStyle.DropDown
            };
            cboSP.AutoCompleteMode =
                AutoCompleteMode.SuggestAppend;
            cboSP.AutoCompleteSource =
                AutoCompleteSource.ListItems;
            p.Controls.Add(cboSP);

            p.Controls.Add(new Label
            {
                Text = "Số lượng",
                Location = new Point(450, 100),
                AutoSize = true
            });

            numSL = new NumericUpDown
            {
                Location = new Point(450, 125),
                Width = 100,
                Minimum = 1,
                Maximum = 1000000,
                Value = 1
            };
            p.Controls.Add(numSL);

            Nut(p, "Thêm giỏ hàng", 580, 120,
                ThemGio, 160);
            Nut(p, "Xóa khỏi giỏ", 750, 120,
                XoaKhoiGio, 150);

            dgvGio = Bang(p, 185);
            dgvGio.Height = 280;

            lblTong = new Label
            {
                Text = "Tổng tiền: 0 VNĐ",
                Location = new Point(20, 490),
                AutoSize = true,
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.DarkRed
            };
            p.Controls.Add(lblTong);

            lblDiem = new Label
            {
                Text = "Điểm tích lũy: 0",
                Location = new Point(20, 530),
                AutoSize = true
            };
            p.Controls.Add(lblDiem);

            Nut(p, "THANH TOÁN", 780, 495,
                ThanhToan, 190);
        }

        private void TaiComboSanPham()
        {
            var dt = spBUS.LayDanhSach();
            dt.Columns.Add("HienThi", typeof(string),
                "MaSP + ' - ' + TenSP");

            cboSP.DataSource = dt;
            cboSP.DisplayMember = "HienThi";
            cboSP.ValueMember = "MaSP";
        }

        private void TaiComboKhachHang()
        {
            var dt = dmBUS.LayKhachHang();
            var row = dt.NewRow();
            row["MaKH"] = 0;
            row["TenKH"] = "Khách lẻ";
            row["DiemTichLuy"] = 0;
            dt.Rows.InsertAt(row, 0);

            cboKH.DataSource = dt;
            cboKH.DisplayMember = "TenKH";
            cboKH.ValueMember = "MaKH";
            cboKH.SelectedValue = 0;
        }

        private void TaiComboNhanVien()
        {
            var dt = dmBUS.LayNhanVien();
            cboNV.DataSource = dt;
            cboNV.DisplayMember = "TenNV";
            cboNV.ValueMember = "MaNV";
        }

        private void ThemGio()
        {
            if (cboSP.SelectedItem is not DataRowView row)
                throw new Exception("Vui lòng chọn sản phẩm hợp lệ.");

            string ma = row["MaSP"].ToString()!;
            string ten = row["TenSP"].ToString()!;
            decimal gia = Convert.ToDecimal(row["GiaBan"]);
            int ton = Convert.ToInt32(row["SoLuongTon"]);
            int sl = (int)numSL.Value;

            var cu = gioHang.FirstOrDefault(x => x.MaSP == ma);
            int daThem = cu?.SoLuong ?? 0;

            if (sl > ton - daThem)
                throw new Exception("Số lượng vượt quá tồn kho.");

            if (cu != null)
                cu.SoLuong += sl;
            else
                gioHang.Add(new GioHangItem
                {
                    MaSP = ma,
                    TenSP = ten,
                    SoLuong = sl,
                    DonGia = gia
                });

            CapNhatGio();
        }

        private void XoaKhoiGio()
        {
            if (dgvGio.CurrentRow?.DataBoundItem
                is GioHangItem item)
            {
                gioHang.Remove(item);
                CapNhatGio();
            }
        }

        private void CapNhatGio()
        {
            dgvGio.DataSource = null;
            dgvGio.DataSource = gioHang.ToList();

            lblTong.Text =
                "Tổng tiền: " + Gia(bhBUS.TinhTongTien(gioHang));

            lblDiem.Text =
                "Điểm tích lũy dự kiến: " +
                bhBUS.TinhDiem(gioHang);
        }

        private void ThanhToan()
        {
            if (cboNV.SelectedValue == null)
                throw new Exception("Chưa chọn nhân viên.");

            int maNV = Convert.ToInt32(cboNV.SelectedValue);
            int kh = Convert.ToInt32(cboKH.SelectedValue);
            int? maKH = kh == 0 ? null : kh;

            if (MessageBox.Show(
                "Xác nhận thanh toán đơn hàng?",
                "Thanh toán",
                MessageBoxButtons.YesNo) != DialogResult.Yes)
                return;

            int maHD = bhBUS.ThanhToan(
                maNV, maKH, gioHang);

            ThongBao("Thanh toán thành công!\nMã hóa đơn: " +
                maHD);

            gioHang.Clear();
            CapNhatGio();
            TaiTatCa();
        }

        // ============================
        // TAB THỐNG KÊ
        // ============================

        private void TaoTabThongKe(TabControl tabs)
        {
            var p = Tab(tabs, "Thống kê");

            p.Controls.Add(new Label
            {
                Text = "Ngày thống kê",
                Location = new Point(20, 20),
                AutoSize = true
            });

            dtpNgay = new DateTimePicker
            {
                Location = new Point(20, 50),
                Width = 200,
                Format = DateTimePickerFormat.Short
            };
            p.Controls.Add(dtpNgay);

            Nut(p, "Xem doanh thu", 240, 45,
                TaiThongKe, 160);

            lblNgay = new Label
            {
                Location = new Point(20, 105),
                AutoSize = true,
                Font = new Font("Segoe UI", 13, FontStyle.Bold)
            };
            p.Controls.Add(lblNgay);

            p.Controls.Add(new Label
            {
                Text = "Tháng thống kê",
                Location = new Point(20, 150),
                AutoSize = true
            });

            dtpThang = new DateTimePicker
            {
                Location = new Point(20, 180),
                Width = 200,
                Format = DateTimePickerFormat.Custom,
                CustomFormat = "MM/yyyy",
                ShowUpDown = true
            };
            p.Controls.Add(dtpThang);

            lblThang = new Label
            {
                Location = new Point(240, 180),
                AutoSize = true,
                Font = new Font("Segoe UI", 13, FontStyle.Bold)
            };
            p.Controls.Add(lblThang);

            p.Controls.Add(new Label
            {
                Text = "TOP 5 SẢN PHẨM BÁN CHẠY",
                Location = new Point(20, 240),
                AutoSize = true,
                Font = new Font("Segoe UI", 14, FontStyle.Bold)
            });

            dgvTop = Bang(p, 285);
            dgvTop.Height = 300;
        }

        private void TaiThongKe()
        {
            lblNgay.Text = "Doanh thu ngày: " +
                Gia(tkDAL.DoanhThuNgay(dtpNgay.Value));

            lblThang.Text = "Doanh thu tháng: " +
                Gia(tkDAL.DoanhThuThang(
                    dtpThang.Value.Year,
                    dtpThang.Value.Month));

            dgvTop.DataSource = tkDAL.Top5SanPham();
        }

        private void TaiTatCa()
        {
            TaiSanPham();
            TaiKhachHang();
            TaiNhanVien();
            TaiThongKe();
        }
    }
}
