using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Windows.Forms;
using CuahangNongduoc.BusinessObject;
using CuahangNongduoc.Controller;
using CuahangNongduoc.DataLayer;

namespace CuahangNongduoc
{
    /// <summary>
    /// PM-17: In Phieu Nhap – KHONG dung ReportViewer (khong can them DLL).
    /// Dung PhieuIn (PrintDocument) giong Phieu chi / Phieu thu.
    /// </summary>
    public partial class frmInPhieuNhap : Form
    {
        private PhieuNhap m_PhieuNhap;
        private readonly PhieuNhapController ctrlPN = new PhieuNhapController();
        private readonly NhaCungCapController ctrlNCC = new NhaCungCapController();

        private Panel panelLoc;
        private ComboBox cmbMaPhieu;
        private ComboBox cmbNCC;
        private DateTimePicker dtNgay;
        private CheckBox chkLocNgay;
        private Button btnXemIn;
        private Label lblHuongDan;

        /// <summary>In 1 phieu da co (tu Nhap hang / Danh sach).</summary>
        public frmInPhieuNhap(PhieuNhap ph)
        {
            m_PhieuNhap = ph;
            InitializeComponent();
        }

        /// <summary>Mo form loc: ma / ngay / NCC roi in.</summary>
        public frmInPhieuNhap()
        {
            m_PhieuNhap = null;
            InitializeComponent();
            ThemPanelLoc();
        }

        private void ThemPanelLoc()
        {
            this.ClientSize = new Size(780, 100);
            panelLoc = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(232, 245, 233),
                Padding = new Padding(8)
            };

            Label l1 = new Label { Text = "Mã phiếu:", AutoSize = true, Location = new Point(10, 18) };
            cmbMaPhieu = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDown,
                Location = new Point(75, 14),
                Width = 120
            };

            Label l2 = new Label { Text = "NCC:", AutoSize = true, Location = new Point(210, 18) };
            cmbNCC = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Location = new Point(250, 14),
                Width = 180
            };

            chkLocNgay = new CheckBox
            {
                Text = "Ngày",
                AutoSize = true,
                Location = new Point(445, 16),
                Checked = false
            };
            dtNgay = new DateTimePicker
            {
                Format = DateTimePickerFormat.Custom,
                CustomFormat = "dd/MM/yyyy",
                Location = new Point(505, 14),
                Width = 110,
                Enabled = false,
                Value = DateTime.Today
            };
            chkLocNgay.CheckedChanged += delegate { dtNgay.Enabled = chkLocNgay.Checked; };

            btnXemIn = new Button
            {
                Text = "Xem / In",
                Location = new Point(630, 12),
                Size = new Size(100, 28),
                BackColor = Color.FromArgb(46, 125, 50),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnXemIn.Click += btnXemIn_Click;

            lblHuongDan = new Label
            {
                Text = "Chọn mã phiếu (hoặc NCC / ngày) → Xem / In. Không cần ReportViewer.",
                AutoSize = true,
                Location = new Point(10, 55),
                ForeColor = Color.FromArgb(80, 80, 80)
            };

            panelLoc.Controls.Add(l1);
            panelLoc.Controls.Add(cmbMaPhieu);
            panelLoc.Controls.Add(l2);
            panelLoc.Controls.Add(cmbNCC);
            panelLoc.Controls.Add(chkLocNgay);
            panelLoc.Controls.Add(dtNgay);
            panelLoc.Controls.Add(btnXemIn);
            panelLoc.Controls.Add(lblHuongDan);
            this.Controls.Add(panelLoc);

            NapDanhSachLoc();
        }

        private void NapDanhSachLoc()
        {
            try
            {
                DataService d = new DataService();
                d.Load(new OleDbCommand("SELECT ID FROM PHIEU_NHAP ORDER BY NGAY_NHAP DESC"));
                cmbMaPhieu.Items.Clear();
                foreach (DataRow r in d.Rows)
                {
                    string id = Convert.ToString(r["ID"]);
                    if (!string.IsNullOrEmpty(id))
                        cmbMaPhieu.Items.Add(id);
                }
            }
            catch { }

            try { ctrlNCC.HienthiAutoComboBox(cmbNCC); }
            catch { }
        }

        private void frmInPhieuNhap_Load(object sender, EventArgs e)
        {
            this.Text = "In Phiếu Nhập Hàng";

            // Co phieu truyen vao → in luon
            if (m_PhieuNhap != null)
            {
                if (!InPhieu(m_PhieuNhap))
                {
                    /* da MessageBox trong InPhieu */
                }
                this.BeginInvoke(new MethodInvoker(delegate { this.Close(); }));
                return;
            }

            // Che do loc: neu khong co panel → bao loi
            if (panelLoc == null)
            {
                MessageBox.Show(
                    "Không có dữ liệu phiếu nhập để in.\n" +
                    "Hãy chọn phiếu từ Nhập hàng / Danh sách, hoặc mở form in có bộ lọc.",
                    this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.BeginInvoke(new MethodInvoker(delegate { this.Close(); }));
            }
        }

        private void btnXemIn_Click(object sender, EventArgs e)
        {
            string ma = (cmbMaPhieu.Text ?? "").Trim();
            if (string.IsNullOrEmpty(ma) && cmbMaPhieu.SelectedItem != null)
                ma = Convert.ToString(cmbMaPhieu.SelectedItem);
            if (string.IsNullOrEmpty(ma))
                ma = TimMaPhieuTheoLoc();

            if (string.IsNullOrEmpty(ma))
            {
                MessageBox.Show(
                    "Chưa chọn phiếu nhập.\nChọn mã phiếu (hoặc NCC / ngày) rồi bấm Xem / In.",
                    this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            PhieuNhap ph = null;
            try { ph = ctrlPN.LayPhieuNhap(ma); }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi đọc phiếu \"" + ma + "\":\n" + ex.Message,
                    this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (ph == null)
            {
                MessageBox.Show(
                    "Không tìm thấy phiếu nhập \"" + ma + "\".",
                    this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            InPhieu(ph);
        }

        private string TimMaPhieuTheoLoc()
        {
            try
            {
                string idNcc = null;
                try
                {
                    if (cmbNCC.SelectedValue != null)
                        idNcc = Convert.ToString(cmbNCC.SelectedValue);
                }
                catch { }

                string sql = "SELECT ID FROM PHIEU_NHAP WHERE 1=1";
                if (!string.IsNullOrEmpty(idNcc))
                    sql += " AND ID_NHA_CUNG_CAP = '" + idNcc.Replace("'", "''") + "'";
                if (chkLocNgay != null && chkLocNgay.Checked)
                {
                    DateTime d = dtNgay.Value.Date;
                    sql += " AND NGAY_NHAP >= #" + d.ToString("MM/dd/yyyy") +
                           "# AND NGAY_NHAP < #" + d.AddDays(1).ToString("MM/dd/yyyy") + "#";
                }
                sql += " ORDER BY NGAY_NHAP DESC";

                DataService ds = new DataService();
                ds.Load(new OleDbCommand(sql));
                if (ds.Rows.Count > 0)
                    return Convert.ToString(ds.Rows[0]["ID"]);
            }
            catch { }
            return null;
        }

        private bool InPhieu(PhieuNhap ph)
        {
            if (ph == null)
            {
                MessageBox.Show("Không có dữ liệu phiếu nhập để in.",
                    this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            // Dam bao chi tiet
            if (ph.ChiTiet == null)
            {
                try
                {
                    MaSanPhamController ctrlSP = new MaSanPhamController();
                    ph.ChiTiet = ctrlSP.ChiTietPhieuNhap(ph.Id);
                }
                catch { }
                if (ph.ChiTiet == null)
                    ph.ChiTiet = new List<MaSanPham>();
            }

            string cuaHang = "CỬA HÀNG NÔNG DƯỢC";
            try
            {
                CuaHang ch = ThamSo.LayCuaHang();
                if (ch != null && !string.IsNullOrEmpty(ch.TenCuaHang) && ch.TenCuaHang.IndexOf('?') < 0)
                    cuaHang = ch.TenCuaHang.ToUpper();
            }
            catch { }

            string tenNcc = "";
            try
            {
                if (ph.NhaCungCap != null)
                    tenNcc = ph.NhaCungCap.HoTen ?? "";
            }
            catch { }

            List<string> dong = new List<string>();
            dong.Add("Mã phiếu|" + (ph.Id ?? ""));
            dong.Add("Nhà cung cấp|" + tenNcc);
            try { dong.Add("Ngày nhập|" + ph.NgayNhap.ToString("dd/MM/yyyy")); }
            catch { dong.Add("Ngày nhập|"); }
            dong.Add("Tổng tiền|" + ph.TongTien.ToString("N0") + " đ");
            try { dong.Add("Đã trả|" + ph.DaTra.ToString("N0") + " đ"); } catch { }
            try { dong.Add("Còn nợ|" + ph.ConNo.ToString("N0") + " đ"); } catch { }

            int stt = 0;
            foreach (MaSanPham ct in ph.ChiTiet)
            {
                if (ct == null) continue;
                stt++;
                string ten = "";
                try
                {
                    if (!string.IsNullOrEmpty(ct.TenSanPham)) ten = ct.TenSanPham;
                    else if (ct.SanPham != null) ten = ct.SanPham.TenSanPham ?? "";
                }
                catch { }
                dong.Add("SP " + stt + "|" + ten
                    + " | Mã " + (ct.Id ?? "")
                    + " | SL " + ct.SoLuong
                    + " | " + ct.GiaNhap.ToString("N0") + " đ");
            }

            try
            {
                PhieuIn.Mo("PHIẾU NHẬP HÀNG", cuaHang, dong.ToArray(), this);
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không in được:\n" + ex.Message,
                    this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
        }
    }
}
