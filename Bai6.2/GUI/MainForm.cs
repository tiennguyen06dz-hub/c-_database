
using System;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using Bai6_2.BUS;
using Bai6_2.DTO;

namespace Bai6_2.GUI
{
    public class MainForm : Form
    {
        // =========================
        // BUSINESS
        // =========================
        private readonly NhanVienBUS nvBUS = new();
        private readonly PhongBanBUS pbBUS = new();
        private readonly ChamCongBUS ccBUS = new();
        private readonly BangLuongBUS luongBUS = new();

        // =========================
        // COLORS
        // =========================
        private readonly Color bg =
            Color.FromArgb(246, 248, 251);

        private readonly Color primary =
            Color.FromArgb(47, 82, 120);

        private readonly Color textColor =
            Color.FromArgb(37, 48, 62);

        private readonly Color borderColor =
            Color.FromArgb(224, 229, 235);

        // =========================
        // MAIN CONTROLS
        // =========================
        private Panel contentPanel = new();
        private Label pageTitle = new();

        private DataGridView dgvNV = new();
        private DataGridView dgvCC = new();
        private DataGridView dgvLuong = new();

        private TextBox txtTim = new();
        private TextBox txtMaNV = new();
        private TextBox txtHoTen = new();
        private TextBox txtCCCD = new();
        private TextBox txtLuong = new();
        private TextBox txtHeSo = new();

        private ComboBox cboPB = new();

        private DateTimePicker dtpSinh = new();
        private DateTimePicker dtpCC = new();
        private DateTimePicker dtpLuong = new();

        private Button btnLuuCC = new();

        private Label lblTrangThaiCC = new();
        private Label lblTongLuong = new();

        private string thangCCDaNap = "";

        // =========================
        // CONSTRUCTOR
        // =========================
        public MainForm()
        {
            Text = "HRM & PAYROLL SYSTEM";
            StartPosition = FormStartPosition.CenterScreen;

            Size = new Size(1350, 820);
            MinimumSize = new Size(1080, 680);

            BackColor = bg;
            Font = new Font("Segoe UI", 10F);
            AutoScaleMode = AutoScaleMode.Dpi;

            TaoGiaoDienChinh();
            HienThiNhanVien();
        }

        // =========================
        // COMMON HELPERS
        // =========================
        private void XuLy(Action action)
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.ToString(),
                    "Chi tiết lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void ThongBao(string message)
        {
            MessageBox.Show(
                message,
                "Thông báo",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        private Button TaoButton(
            string text,
            EventHandler click,
            int minWidth = 115)
        {
            Button btn = new()
            {
                Text = text,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                MinimumSize = new Size(minWidth, 44),
                Height = 44,
                Padding = new Padding(15, 0, 15, 0),
                Margin = new Padding(0, 3, 10, 7),
                BackColor = primary,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10F),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                UseVisualStyleBackColor = false
            };

            btn.FlatAppearance.BorderSize = 0;
            btn.Click += click;

            return btn;
        }

        private FlowLayoutPanel TaoThanhCongCu()
        {
            return new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                WrapContents = true,
                FlowDirection = FlowDirection.LeftToRight,
                Padding = new Padding(0, 4, 0, 8),
                Margin = Padding.Empty
            };
        }

        private TableLayoutPanel TaoTruong(
            string labelText,
            Control input)
        {
            TableLayoutPanel panel = new()
            {
                Dock = DockStyle.Fill,
                Height = 86,
                MinimumSize = new Size(0, 86),
                ColumnCount = 1,
                RowCount = 2,
                Padding = new Padding(3, 2, 10, 5),
                Margin = Padding.Empty
            };

            panel.RowStyles.Add(
                new RowStyle(SizeType.Absolute, 32));

            panel.RowStyles.Add(
                new RowStyle(SizeType.Percent, 100));

            Label label = new()
            {
                Text = labelText,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                ForeColor = textColor,
                Font = new Font("Segoe UI", 10F),
                AutoSize = false
            };

            input.Dock = DockStyle.Top;
            input.Font = new Font("Segoe UI", 10F);
            input.Margin = new Padding(0, 4, 0, 0);

            if (input is TextBox tb)
            {
                tb.BorderStyle = BorderStyle.FixedSingle;
            }

            panel.Controls.Add(label, 0, 0);
            panel.Controls.Add(input, 0, 1);

            return panel;
        }

        private DataGridView TaoDataGridView()
        {
            DataGridView dgv = new()
            {
                Dock = DockStyle.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                RowHeadersVisible = false,

                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AllowUserToResizeRows = false,

                MultiSelect = false,
                SelectionMode =
                    DataGridViewSelectionMode.FullRowSelect,

                AutoSizeColumnsMode =
                    DataGridViewAutoSizeColumnsMode.None,

                AutoSizeRowsMode =
                    DataGridViewAutoSizeRowsMode.None,

                ScrollBars = ScrollBars.Both,

                ColumnHeadersHeight = 45,
                ColumnHeadersHeightSizeMode =
                    DataGridViewColumnHeadersHeightSizeMode
                        .DisableResizing,

                RowTemplate = { Height = 38 },

                EnableHeadersVisualStyles = false,
                GridColor = borderColor,

                Font = new Font("Segoe UI", 10F)
            };

            dgv.ColumnHeadersDefaultCellStyle.BackColor =
                Color.FromArgb(238, 242, 247);

            dgv.ColumnHeadersDefaultCellStyle.ForeColor =
                textColor;

            dgv.ColumnHeadersDefaultCellStyle.Font =
                new Font("Segoe UI", 10F, FontStyle.Bold);

            dgv.ColumnHeadersDefaultCellStyle.Alignment =
                DataGridViewContentAlignment.MiddleLeft;

            dgv.ColumnHeadersDefaultCellStyle.WrapMode =
                DataGridViewTriState.False;

            dgv.DefaultCellStyle.Padding =
                new Padding(7, 0, 7, 0);

            dgv.DefaultCellStyle.SelectionBackColor =
                Color.FromArgb(215, 228, 242);

            dgv.DefaultCellStyle.SelectionForeColor =
                textColor;

            dgv.AlternatingRowsDefaultCellStyle.BackColor =
                Color.FromArgb(248, 250, 252);

            return dgv;
        }

        private void DinhDangCot(
            DataGridView dgv,
            string name,
            string header,
            int width)
        {
            if (!dgv.Columns.Contains(name))
                return;

            var col = dgv.Columns[name];

            if (col == null)
                return;

            col.HeaderText = header;
            col.Width = width;
            col.MinimumWidth = Math.Min(width, 70);
            col.SortMode =
                DataGridViewColumnSortMode.Automatic;
        }

        // =========================
        // MAIN LAYOUT
        // =========================
        private void TaoGiaoDienChinh()
        {
            TableLayoutPanel root = new()
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                BackColor = bg
            };

            root.ColumnStyles.Add(
                new ColumnStyle(SizeType.Absolute, 225));

            root.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 100));

            Controls.Add(root);

            // Sidebar
            TableLayoutPanel sidebar = new()
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Padding = new Padding(14, 16, 14, 14),
                ColumnCount = 1,
                RowCount = 5
            };

            sidebar.RowStyles.Add(
                new RowStyle(SizeType.Absolute, 85));

            sidebar.RowStyles.Add(
                new RowStyle(SizeType.Absolute, 60));

            sidebar.RowStyles.Add(
                new RowStyle(SizeType.Absolute, 60));

            sidebar.RowStyles.Add(
                new RowStyle(SizeType.Absolute, 60));

            sidebar.RowStyles.Add(
                new RowStyle(SizeType.Percent, 100));

            Label logo = new()
            {
                Text = "HRM SYSTEM",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Font = new Font(
                    "Segoe UI", 15F, FontStyle.Bold),
                ForeColor = primary
            };

            sidebar.Controls.Add(logo, 0, 0);

            Button TaoMenu(string text, Action action)
            {
                Button btn = new()
                {
                    Text = text,
                    Dock = DockStyle.Fill,
                    TextAlign = ContentAlignment.MiddleLeft,
                    FlatStyle = FlatStyle.Flat,
                    BackColor = Color.White,
                    ForeColor = textColor,
                    Font = new Font("Segoe UI", 11F),
                    Padding = new Padding(12, 0, 0, 0),
                    Margin = new Padding(0, 4, 0, 4),
                    Cursor = Cursors.Hand
                };

                btn.FlatAppearance.BorderSize = 0;

                btn.Click += (s, e) =>
                    XuLy(action);

                return btn;
            }

            sidebar.Controls.Add(
                TaoMenu("Nhân viên", HienThiNhanVien), 0, 1);

            sidebar.Controls.Add(
                TaoMenu("Chấm công", HienThiChamCong), 0, 2);

            sidebar.Controls.Add(
                TaoMenu("Bảng lương", HienThiBangLuong), 0, 3);

            root.Controls.Add(sidebar, 0, 0);

            // Main area
            TableLayoutPanel main = new()
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(22, 16, 22, 20),
                ColumnCount = 1,
                RowCount = 2
            };

            main.RowStyles.Add(
                new RowStyle(SizeType.Absolute, 75));

            main.RowStyles.Add(
                new RowStyle(SizeType.Percent, 100));

            pageTitle = new Label
            {
                Dock = DockStyle.Fill,
                Font = new Font(
                    "Segoe UI", 20F, FontStyle.Bold),
                ForeColor = textColor,
                TextAlign = ContentAlignment.MiddleLeft
            };

            contentPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Padding = new Padding(18)
            };

            main.Controls.Add(pageTitle, 0, 0);
            main.Controls.Add(contentPanel, 0, 1);

            root.Controls.Add(main, 1, 0);
        }

        private TableLayoutPanel TaoTrang(string title)
        {
            pageTitle.Text = title;

            contentPanel.Controls.Clear();

            TableLayoutPanel page = new()
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 3,
                BackColor = Color.White
            };

            page.RowStyles.Add(
                new RowStyle(SizeType.AutoSize));

            page.RowStyles.Add(
                new RowStyle(SizeType.AutoSize));

            page.RowStyles.Add(
                new RowStyle(SizeType.Percent, 100));

            contentPanel.Controls.Add(page);

            return page;
        }

        // =========================
        // EMPLOYEE PAGE
        // =========================
        private void HienThiNhanVien()
        {
            var page = TaoTrang(
                "Quản lý hồ sơ nhân viên");

            // Input fields
            TableLayoutPanel fields = new()
            {
                Dock = DockStyle.Fill,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                ColumnCount = 4,
                RowCount = 2,
                Padding = new Padding(0, 0, 0, 12)
            };

            for (int i = 0; i < 4; i++)
            {
                fields.ColumnStyles.Add(
                    new ColumnStyle(SizeType.Percent, 25));
            }

            fields.RowStyles.Add(
                new RowStyle(SizeType.Absolute, 90));

            fields.RowStyles.Add(
                new RowStyle(SizeType.Absolute, 90));

            txtMaNV = new TextBox
            {
                ReadOnly = true,
                BackColor = Color.FromArgb(245, 247, 249)
            };

            txtHoTen = new TextBox();
            txtCCCD = new TextBox();
            txtLuong = new TextBox();

            txtHeSo = new TextBox
            {
                Text = "1"
            };

            dtpSinh = new DateTimePicker
            {
                Format = DateTimePickerFormat.Custom,
                CustomFormat = "dd/MM/yyyy"
            };

            cboPB = new ComboBox
            {
                DropDownStyle =
                    ComboBoxStyle.DropDownList
            };

            fields.Controls.Add(
                TaoTruong("Mã nhân viên", txtMaNV), 0, 0);

            fields.Controls.Add(
                TaoTruong("Họ và tên", txtHoTen), 1, 0);

            fields.Controls.Add(
                TaoTruong("Ngày sinh", dtpSinh), 2, 0);

            fields.Controls.Add(
                TaoTruong("CCCD", txtCCCD), 3, 0);

            fields.Controls.Add(
                TaoTruong("Phòng ban", cboPB), 0, 1);

            fields.Controls.Add(
                TaoTruong("Lương cơ bản", txtLuong), 1, 1);

            fields.Controls.Add(
                TaoTruong("Hệ số lương", txtHeSo), 2, 1);

            page.Controls.Add(fields, 0, 0);

            // Action buttons
            FlowLayoutPanel actions = TaoThanhCongCu();

            txtTim = new TextBox
            {
                Width = 240,
                PlaceholderText =
                    "Tên, CCCD hoặc phòng ban",
                Margin = new Padding(0, 8, 10, 8)
            };

            actions.Controls.Add(txtTim);

            actions.Controls.Add(
                TaoButton("Tìm kiếm",
                    (s, e) => XuLy(TaiNhanVien)));

            actions.Controls.Add(
                TaoButton("Thêm",
                    (s, e) =>
                        XuLy(() => LuuNhanVien(true))));

            actions.Controls.Add(
                TaoButton("Sửa / Chuyển PB",
                    (s, e) =>
                        XuLy(() => LuuNhanVien(false)),
                    165));

            actions.Controls.Add(
                TaoButton("Xóa",
                    (s, e) => XuLy(XoaNhanVien)));

            actions.Controls.Add(
                TaoButton("Làm mới",
                    (s, e) => XuLy(LamMoiNhanVien)));

            page.Controls.Add(actions, 0, 1);

            // Employee table
            dgvNV = TaoDataGridView();
            dgvNV.ReadOnly = true;

            page.Controls.Add(dgvNV, 0, 2);

            dgvNV.SelectionChanged +=
                DgvNV_SelectionChanged;

            XuLy(() =>
            {
                cboPB.DisplayMember = "TenPB";
                cboPB.ValueMember = "MaPB";
                cboPB.DataSource = pbBUS.LayDanhSach();

                TaiNhanVien();
            });
        }

        private void TaiNhanVien()
        {
            dgvNV.DataSource =
                nvBUS.TimKiem(txtTim.Text.Trim());

            DinhDangCot(
                dgvNV, "MaNV", "Mã NV", 85);

            DinhDangCot(
                dgvNV, "HoTen", "Họ và tên", 200);

            DinhDangCot(
                dgvNV, "NgaySinh", "Ngày sinh", 135);

            DinhDangCot(
                dgvNV, "CCCD", "CCCD", 165);

            DinhDangCot(
                dgvNV, "MaPB", "Mã PB", 85);

            DinhDangCot(
                dgvNV, "TenPB", "Phòng ban", 150);

            DinhDangCot(
                dgvNV, "LuongCoBan", "Lương cơ bản", 160);

            DinhDangCot(
                dgvNV, "HeSoLuong", "Hệ số", 100);

            if (dgvNV.Columns.Contains("LuongCoBan"))
            {
                dgvNV.Columns["LuongCoBan"]!
                    .DefaultCellStyle.Format = "N0";
            }

            if (dgvNV.Columns.Contains("NgaySinh"))
            {
                dgvNV.Columns["NgaySinh"]!
                    .DefaultCellStyle.Format =
                        "dd/MM/yyyy";
            }
        }

        private void DgvNV_SelectionChanged(
            object? sender,
            EventArgs e)
        {
            if (dgvNV.CurrentRow?.DataBoundItem
                is not DataRowView row)
                return;

            if (txtMaNV.IsDisposed ||
                txtHoTen.IsDisposed ||
                cboPB.IsDisposed)
                return;

            txtMaNV.Text =
                row["MaNV"]?.ToString() ?? "";

            txtHoTen.Text =
                row["HoTen"]?.ToString() ?? "";

            txtCCCD.Text =
                row["CCCD"]?.ToString() ?? "";

            txtLuong.Text =
                row["LuongCoBan"]?.ToString() ?? "";

            txtHeSo.Text =
                row["HeSoLuong"]?.ToString() ?? "";

            if (row["NgaySinh"] != DBNull.Value)
            {
                DateTime ngay =
                    Convert.ToDateTime(row["NgaySinh"]);

                if (ngay >= dtpSinh.MinDate &&
                    ngay <= dtpSinh.MaxDate)
                {
                    dtpSinh.Value = ngay;
                }
            }

            if (row["MaPB"] != DBNull.Value &&
                cboPB.DataSource != null)
            {
                cboPB.SelectedValue =
                    Convert.ToInt32(row["MaPB"]);
            }
        }

        private NhanVienDTO DocNhanVien(bool them)
        {
            if (!decimal.TryParse(
                txtLuong.Text,
                out decimal luong))
            {
                throw new Exception(
                    "Lương cơ bản không hợp lệ.");
            }

            if (!double.TryParse(
                txtHeSo.Text,
                out double heSo))
            {
                throw new Exception(
                    "Hệ số lương không hợp lệ.");
            }

            int maNV = 0;

            if (!them &&
                !int.TryParse(txtMaNV.Text, out maNV))
            {
                throw new Exception(
                    "Vui lòng chọn nhân viên.");
            }

            return new NhanVienDTO
            {
                MaNV = maNV,
                HoTen = txtHoTen.Text.Trim(),
                CCCD = txtCCCD.Text.Trim(),
                NgaySinh = dtpSinh.Value.Date,

                MaPB = cboPB.SelectedValue is int maPB
                    ? maPB
                    : null,

                LuongCoBan = luong,
                HeSoLuong = heSo
            };
        }

        private void LuuNhanVien(bool them)
        {
            NhanVienDTO nv = DocNhanVien(them);

            if (them)
                nvBUS.Them(nv);
            else
                nvBUS.Sua(nv);

            TaiNhanVien();

            ThongBao(them
                ? "Thêm nhân viên thành công."
                : "Cập nhật nhân viên thành công.");
        }

        private void XoaNhanVien()
        {
            if (!int.TryParse(
                txtMaNV.Text,
                out int maNV))
            {
                throw new Exception(
                    "Vui lòng chọn nhân viên.");
            }

            DialogResult result = MessageBox.Show(
                "Bạn có chắc muốn xóa nhân viên này?",
                "Xác nhận xóa",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result != DialogResult.Yes)
                return;

            nvBUS.Xoa(maNV);

            LamMoiNhanVien();

            ThongBao("Xóa nhân viên thành công.");
        }

        private void LamMoiNhanVien()
        {
            txtMaNV.Clear();
            txtHoTen.Clear();
            txtCCCD.Clear();
            txtLuong.Clear();
            txtHeSo.Text = "1";
            txtTim.Clear();

            dtpSinh.Value = DateTime.Today;

            if (cboPB.Items.Count > 0)
                cboPB.SelectedIndex = 0;

            TaiNhanVien();
        }

        // =========================
        // ATTENDANCE PAGE
        // =========================
        private void HienThiChamCong()
        {
            var page = TaoTrang(
                "Chấm công hàng tháng");

            FlowLayoutPanel tools = TaoThanhCongCu();

            dtpCC = new DateTimePicker
            {
                Width = 155,
                Format = DateTimePickerFormat.Custom,
                CustomFormat = "MM/yyyy",
                ShowUpDown = true,
                Margin = new Padding(0, 8, 12, 8)
            };

            tools.Controls.Add(dtpCC);

            tools.Controls.Add(
                TaoButton("Nạp chấm công",
                    (s, e) => XuLy(NapChamCong),
                    155));

            btnLuuCC = TaoButton(
                "Lưu chấm công",
                (s, e) => XuLy(LuuChamCong),
                155);

            btnLuuCC.Enabled = false;
            tools.Controls.Add(btnLuuCC);

            page.Controls.Add(tools, 0, 0);

            lblTrangThaiCC = new Label
            {
                Text =
                    "Chọn tháng/năm và nhấn Nạp chấm công.",
                Dock = DockStyle.Fill,
                AutoSize = true,
                Padding = new Padding(0, 10, 0, 14),
                ForeColor = textColor
            };

            page.Controls.Add(lblTrangThaiCC, 0, 1);

            dgvCC = TaoDataGridView();

            dgvCC.EditMode =
                DataGridViewEditMode.EditOnEnter;

            dgvCC.DataError += (s, e) =>
            {
                e.ThrowException = false;

                MessageBox.Show(
                    "Dữ liệu nhập không hợp lệ. " +
                    "Vui lòng nhập số.",
                    "Lỗi nhập liệu",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            };

            page.Controls.Add(dgvCC, 0, 2);

            thangCCDaNap = "";
        }

        private string ThangChamCong()
        {
            return dtpCC.Value.ToString(
                "MM/yyyy",
                CultureInfo.InvariantCulture);
        }

        private void NapChamCong()
        {
            string thang = ThangChamCong();

            DataTable dt =
                ccBUS.NapChamCong(thang);

            dgvCC.DataSource = dt;

            bool daChot = ccBUS.DaChot(thang);

            dgvCC.ReadOnly = daChot;
            btnLuuCC.Enabled = !daChot;

            thangCCDaNap = thang;

            DinhDangCot(
                dgvCC, "MaCong", "Mã công", 110);

            DinhDangCot(
                dgvCC, "MaNV", "Mã nhân viên", 140);

            DinhDangCot(
                dgvCC, "ThangNam", "Tháng/năm", 135);

            DinhDangCot(
                dgvCC, "SoNgayCong", "Số ngày công", 165);

            DinhDangCot(
                dgvCC, "SoGioOT", "Số giờ OT", 145);

            foreach (DataGridViewColumn col
                in dgvCC.Columns)
            {
                col.ReadOnly =
                    daChot ||
                    (col.Name != "SoNgayCong" &&
                     col.Name != "SoGioOT");
            }

            lblTrangThaiCC.Text = daChot
                ? "Tháng đã chốt lương. " +
                  "Dữ liệu chấm công chỉ được xem."
                : "Chỉ chỉnh sửa Số ngày công và " +
                  "Số giờ OT trực tiếp trên bảng.";
        }

        private void LuuChamCong()
        {
            if (thangCCDaNap != ThangChamCong())
            {
                throw new Exception(
                    "Bạn đã thay đổi tháng/năm. " +
                    "Vui lòng nạp lại chấm công.");
            }

            if (!dgvCC.EndEdit())
            {
                throw new Exception(
                    "Có ô nhập chưa hợp lệ.");
            }

            if (dgvCC.DataSource is DataTable dt)
            {
                if (BindingContext[dt]
                    is CurrencyManager manager)
                {
                    manager.EndCurrentEdit();
                }
            }

            ccBUS.LuuChamCong(thangCCDaNap);

            ThongBao(
                "Lưu chấm công thành công.");

            NapChamCong();
        }

        // =========================
        // PAYROLL PAGE
        // =========================
        private void HienThiBangLuong()
        {
            var page = TaoTrang(
                "Tính lương và chốt lương");

            FlowLayoutPanel tools = TaoThanhCongCu();

            dtpLuong = new DateTimePicker
            {
                Width = 155,
                Format = DateTimePickerFormat.Custom,
                CustomFormat = "MM/yyyy",
                ShowUpDown = true,
                Margin = new Padding(0, 8, 12, 8)
            };

            tools.Controls.Add(dtpLuong);

            tools.Controls.Add(
                TaoButton("Xem bảng lương",
                    (s, e) => XuLy(TaiBangLuong),
                    160));

            tools.Controls.Add(
                TaoButton("Chốt lương tháng",
                    (s, e) => XuLy(ChotLuong),
                    175));

            page.Controls.Add(tools, 0, 0);

            lblTongLuong = new Label
            {
                Text = "Tổng lương: 0 VNĐ",
                Dock = DockStyle.Fill,
                AutoSize = true,
                Padding = new Padding(0, 12, 0, 16),
                Font = new Font(
                    "Segoe UI", 13F, FontStyle.Bold),
                ForeColor = primary
            };

            page.Controls.Add(lblTongLuong, 0, 1);

            dgvLuong = TaoDataGridView();
            dgvLuong.ReadOnly = true;

            page.Controls.Add(dgvLuong, 0, 2);

            XuLy(TaiBangLuong);
        }

        private string ThangBangLuong()
        {
            return dtpLuong.Value.ToString(
                "MM/yyyy",
                CultureInfo.InvariantCulture);
        }

        private void TaiBangLuong()
        {
            string thang = ThangBangLuong();

            bool daChot = ccBUS.DaChot(thang);

            DataTable dt = daChot
                ? luongBUS.XemBangLuongDaChot(thang)
                : luongBUS.XemLuong(thang);

            dgvLuong.DataSource = dt;

            DinhDangCot(
                dgvLuong, "MaLuong", "Mã lương", 110);

            DinhDangCot(
                dgvLuong, "MaNV", "Mã nhân viên", 130);

            DinhDangCot(
                dgvLuong, "HoTen", "Họ và tên", 205);

            DinhDangCot(
                dgvLuong, "ThangNam", "Tháng/năm", 125);

            DinhDangCot(
                dgvLuong, "LuongCoBan",
                "Lương cơ bản", 160);

            DinhDangCot(
                dgvLuong, "HeSoLuong",
                "Hệ số", 95);

            DinhDangCot(
                dgvLuong, "SoNgayCong",
                "Ngày công", 120);

            DinhDangCot(
                dgvLuong, "SoGioOT",
                "Giờ OT", 110);

            DinhDangCot(
                dgvLuong, "ThucLinh",
                "Thực lĩnh", 175);

            if (dgvLuong.Columns.Contains("ThucLinh"))
            {
                dgvLuong.Columns["ThucLinh"]!
                    .DefaultCellStyle.Format = "N2";
            }

            if (dgvLuong.Columns.Contains("LuongCoBan"))
            {
                dgvLuong.Columns["LuongCoBan"]!
                    .DefaultCellStyle.Format = "N0";
            }

            decimal tong = 0;

            foreach (DataRow row in dt.Rows)
            {
                if (row["ThucLinh"] != DBNull.Value)
                {
                    tong += Convert.ToDecimal(
                        row["ThucLinh"]);
                }
            }

            string trangThai = daChot
                ? "Đã chốt"
                : "Dự kiến";

            lblTongLuong.Text =
                $"{trangThai} | Tổng lương: " +
                tong.ToString(
                    "N0",
                    new CultureInfo("vi-VN")) +
                " VNĐ";
        }

        private void ChotLuong()
        {
            string thang = ThangBangLuong();

            DialogResult result = MessageBox.Show(
                $"Bạn có chắc muốn chốt lương " +
                $"tháng {thang}?\n\n" +
                "Sau khi chốt, dữ liệu chấm công " +
                "của tháng sẽ bị khóa.",
                "Xác nhận chốt lương",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result != DialogResult.Yes)
                return;

            int soNV = luongBUS.ChotLuong(thang);

            ThongBao(
                $"Đã chốt lương thành công " +
                $"cho {soNV} nhân viên.");

            TaiBangLuong();
        }
    }
}
