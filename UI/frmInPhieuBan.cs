using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using CuahangNongduoc.BusinessObject;
using CuahangNongduoc.Controller;

namespace CuahangNongduoc
{
    /// <summary>
    /// PM-21: In Phiếu Bán Hàng — không phụ thuộc ReportViewer DLL.
    /// Design: form trắng + thanh nút thống nhất; in qua PrintPreview (PhieuIn).
    /// </summary>
    public partial class frmInPhieuBan : Form
    {
        private PhieuBan m_PhieuBan;

        // Tham số in (ticket #5)
        private DateTimePicker dtTu, dtDen;
        private TextBox txtMaPhieu, txtKhachHang;
        private ListBox lstPhieu;
        private Label lblThongBao;
        private Button btnIn, btnDong, btnTim;

        public frmInPhieuBan()
        {
            InitializeComponent();
            XayGiaoDien();
        }

        public frmInPhieuBan(PhieuBan ph)
        {
            InitializeComponent();
            m_PhieuBan = ph;
            XayGiaoDien();
        }

        void XayGiaoDien()
        {
            this.Text = "In Phiếu Bán Hàng";
            this.BackColor = Color.White;
            this.Font = new Font("Tahoma", 9f);

            // Thanh công cụ trên
            Panel tool = new Panel();
            tool.Dock = DockStyle.Top;
            tool.Height = 40;
            tool.BackColor = Color.FromArgb(240, 240, 240);

            btnIn = new Button();
            btnIn.Text = "In";
            btnIn.Width = 80;
            btnIn.Height = 28;
            btnIn.Left = 8;
            btnIn.Top = 6;
            btnIn.Click += btnIn_Click;

            btnTim = new Button();
            btnTim.Text = "Tìm";
            btnTim.Width = 80;
            btnTim.Height = 28;
            btnTim.Left = 96;
            btnTim.Top = 6;
            btnTim.Click += btnTim_Click;

            btnDong = new Button();
            btnDong.Text = "Đóng";
            btnDong.Width = 80;
            btnDong.Height = 28;
            btnDong.Left = 184;
            btnDong.Top = 6;
            btnDong.Click += delegate { this.Close(); };

            tool.Controls.Add(btnIn);
            tool.Controls.Add(btnTim);
            tool.Controls.Add(btnDong);

            // Panel tham số
            GroupBox grp = new GroupBox();
            grp.Text = "Tham số in";
            grp.Dock = DockStyle.Top;
            grp.Height = 100;
            grp.Padding = new Padding(8);

            Label l1 = new Label { Text = "Từ ngày:", Left = 12, Top = 24, AutoSize = true };
            dtTu = new DateTimePicker { Left = 80, Top = 20, Width = 120, Format = DateTimePickerFormat.Short };
            dtTu.Value = DateTime.Today.AddMonths(-1);

            Label l2 = new Label { Text = "Đến ngày:", Left = 220, Top = 24, AutoSize = true };
            dtDen = new DateTimePicker { Left = 290, Top = 20, Width = 120, Format = DateTimePickerFormat.Short };
            dtDen.Value = DateTime.Today;

            Label l3 = new Label { Text = "Mã phiếu:", Left = 12, Top = 56, AutoSize = true };
            txtMaPhieu = new TextBox { Left = 80, Top = 52, Width = 120 };

            Label l4 = new Label { Text = "Khách hàng:", Left = 220, Top = 56, AutoSize = true };
            txtKhachHang = new TextBox { Left = 300, Top = 52, Width = 160 };

            grp.Controls.Add(l1);
            grp.Controls.Add(dtTu);
            grp.Controls.Add(l2);
            grp.Controls.Add(dtDen);
            grp.Controls.Add(l3);
            grp.Controls.Add(txtMaPhieu);
            grp.Controls.Add(l4);
            grp.Controls.Add(txtKhachHang);

            lblThongBao = new Label();
            lblThongBao.Dock = DockStyle.Bottom;
            lblThongBao.Height = 28;
            lblThongBao.TextAlign = ContentAlignment.MiddleLeft;
            lblThongBao.Padding = new Padding(8, 0, 0, 0);
            lblThongBao.ForeColor = Color.DarkRed;

            lstPhieu = new ListBox();
            lstPhieu.Dock = DockStyle.Fill;
            lstPhieu.Font = new Font("Tahoma", 9.5f);
            lstPhieu.DisplayMember = "Text";

            this.Controls.Add(lstPhieu);
            this.Controls.Add(lblThongBao);
            this.Controls.Add(grp);
            this.Controls.Add(tool);
        }

        private void frmInPhieuBan_Load(object sender, EventArgs e)
        {
            this.Text = "In Phiếu Bán Hàng";

            // PM-93: null-safe
            if (m_PhieuBan == null)
            {
                lblThongBao.Text = "Chưa có phiếu — chọn tham số rồi bấm Tìm, hoặc mở từ danh sách/phiếu bán.";
                MessageBox.Show(
                    "Không có dữ liệu phiếu bán để in.\n\nBạn có thể tìm theo ngày / mã phiếu / khách hàng rồi In.",
                    this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Có phiếu truyền vào: hiện tóm tắt + sẵn sàng In
            if (txtMaPhieu.Text.Trim() == "")
                txtMaPhieu.Text = m_PhieuBan.Id ?? "";

            string kh = "";
            try
            {
                if (m_PhieuBan.KhachHang != null)
                    kh = m_PhieuBan.KhachHang.HoTen ?? m_PhieuBan.KhachHang.Id ?? "";
            }
            catch { }

            lstPhieu.Items.Clear();
            lstPhieu.Items.Add(new PhieuItem(m_PhieuBan,
                string.Format("{0}  |  {1:dd/MM/yyyy}  |  {2}  |  Tổng: {3:n0}",
                    m_PhieuBan.Id,
                    m_PhieuBan.NgayBan,
                    kh,
                    m_PhieuBan.TongTien)));
            lstPhieu.SelectedIndex = 0;
            lblThongBao.Text = "Đã nạp phiếu. Bấm In để xem trước / in.";
            lblThongBao.ForeColor = Color.DarkGreen;
        }

        void btnTim_Click(object sender, EventArgs e)
        {
            try
            {
                lstPhieu.Items.Clear();
                string ma = (txtMaPhieu.Text ?? "").Trim();
                string khKey = (txtKhachHang.Text ?? "").Trim().ToLower();

                PhieuBanController ctrl = new PhieuBanController();

                // Ưu tiên theo mã phiếu
                if (ma.Length > 0)
                {
                    PhieuBan ph = ctrl.LayPhieuBan(ma);
                    if (ph == null)
                    {
                        lblThongBao.ForeColor = Color.DarkRed;
                        lblThongBao.Text = "Không tìm thấy phiếu: " + ma;
                        MessageBox.Show("Không tìm thấy phiếu bán \"" + ma + "\".", this.Text,
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    ThemItem(ph);
                    lblThongBao.ForeColor = Color.DarkGreen;
                    lblThongBao.Text = "Tìm thấy 1 phiếu theo mã.";
                    lstPhieu.SelectedIndex = 0;
                    return;
                }

                // Lọc theo khoảng ngày từ danh sách sỉ + lẻ (nếu controller hỗ trợ)
                int dem = 0;
                dem += NapTuBang(ctrl, true, khKey);  // bán sỉ
                dem += NapTuBang(ctrl, false, khKey); // bán lẻ

                if (dem == 0)
                {
                    lblThongBao.ForeColor = Color.DarkRed;
                    lblThongBao.Text = "Không có phiếu phù hợp tham số.";
                    MessageBox.Show(
                        "Không tìm thấy phiếu bán theo tham số đã chọn.\nKiểm tra lại ngày / khách hàng.",
                        this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    lblThongBao.ForeColor = Color.DarkGreen;
                    lblThongBao.Text = "Tìm thấy " + dem + " phiếu. Chọn một dòng rồi bấm In.";
                    if (lstPhieu.Items.Count > 0) lstPhieu.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tìm phiếu:\n" + ex.Message, this.Text,
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        int NapTuBang(PhieuBanController ctrl, bool banSi, string khKey)
        {
            int dem = 0;
            try
            {
                // Dùng DataTable qua Hienthi — binding tạm
                BindingNavigator nav = new BindingNavigator();
                DataGridView dgv = new DataGridView();
                if (banSi)
                    ctrl.HienthiPhieuBanSi(nav, dgv);
                else
                    ctrl.HienthiPhieuBanLe(nav, dgv);

                BindingSource bs = nav.BindingSource;
                if (bs == null) return 0;

                foreach (object o in bs)
                {
                    System.Data.DataRowView rv = o as System.Data.DataRowView;
                    if (rv == null) continue;
                    try
                    {
                        DateTime ngay = Convert.ToDateTime(rv["NGAY_BAN"]);
                        if (ngay.Date < dtTu.Value.Date || ngay.Date > dtDen.Value.Date)
                            continue;

                        string id = Convert.ToString(rv["ID"]);
                        PhieuBan ph = ctrl.LayPhieuBan(id);
                        if (ph == null) continue;

                        if (khKey.Length > 0)
                        {
                            string ten = "";
                            try
                            {
                                if (ph.KhachHang != null)
                                    ten = (ph.KhachHang.HoTen ?? "") + " " + (ph.KhachHang.Id ?? "");
                            }
                            catch { }
                            if (ten.ToLower().IndexOf(khKey) < 0) continue;
                        }

                        ThemItem(ph);
                        dem++;
                    }
                    catch { }
                }
            }
            catch { }
            return dem;
        }

        void ThemItem(PhieuBan ph)
        {
            string kh = "";
            try
            {
                if (ph.KhachHang != null)
                    kh = ph.KhachHang.HoTen ?? ph.KhachHang.Id ?? "";
            }
            catch { }
            lstPhieu.Items.Add(new PhieuItem(ph,
                string.Format("{0}  |  {1:dd/MM/yyyy}  |  {2}  |  Tổng: {3:n0}",
                    ph.Id, ph.NgayBan, kh, ph.TongTien)));
        }

        void btnIn_Click(object sender, EventArgs e)
        {
            PhieuBan ph = null;
            if (lstPhieu.SelectedItem is PhieuItem)
                ph = ((PhieuItem)lstPhieu.SelectedItem).Phieu;
            else
                ph = m_PhieuBan;

            if (ph == null)
            {
                MessageBox.Show(
                    "Chưa có phiếu để in.\nChọn phiếu trên danh sách hoặc tìm theo tham số trước.",
                    this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // Nạp lại từ DB theo mã — tránh DataRow đã xóa / ChiTiet hỏng
                string ma = ph.Id;
                if (ma != null && ma.Length > 0)
                {
                    try
                    {
                        PhieuBanController c = new PhieuBanController();
                        PhieuBan fresh = c.LayPhieuBan(ma);
                        if (fresh != null) ph = fresh;
                    }
                    catch { }
                }
                InPhieu(ph);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi in phiếu:\n" + ex.Message, this.Text,
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        void InPhieu(PhieuBan ph)
        {
            string cuaHang = "Cửa hàng Nông dược";
            try
            {
                CuaHang ch = ThamSo.LayCuaHang();
                if (ch != null && !string.IsNullOrEmpty(ch.TenCuaHang))
                    cuaHang = ch.TenCuaHang;
            }
            catch { }

            string tenKH = "";
            try
            {
                if (ph.KhachHang != null)
                    tenKH = ph.KhachHang.HoTen ?? ph.KhachHang.Id ?? "";
            }
            catch { }

            List<string> dong = new List<string>();
            dong.Add("Mã phiếu|" + (ph.Id ?? ""));
            dong.Add("Ngày bán|" + ph.NgayBan.ToString("dd/MM/yyyy"));
            dong.Add("Khách hàng / Đại lý|" + tenKH);
            dong.Add("Tổng tiền|" + string.Format("{0:n0} đ", ph.TongTien));
            dong.Add("Đã trả|" + string.Format("{0:n0} đ", ph.DaTra));
            dong.Add("Còn nợ|" + string.Format("{0:n0} đ", ph.ConNo));

            // Chi tiết SP
            try
            {
                if (ph.ChiTiet != null && ph.ChiTiet.Count > 0)
                {
                    dong.Add("— Chi tiết —|");
                    int stt = 1;
                    foreach (ChiTietPhieuBan ct in ph.ChiTiet)
                    {
                        if (ct == null) continue;
                        string ma = "";
                        try
                        {
                            if (ct.MaSanPham != null) ma = ct.MaSanPham.Id ?? "";
                        }
                        catch { }
                        try
                        {
                            dong.Add(string.Format("{0}. {1}|SL {2} × {3:n0} = {4:n0}",
                                stt, ma, ct.SoLuong, ct.DonGia, ct.ThanhTien));
                        }
                        catch { }
                        stt++;
                    }
                }
            }
            catch { }

            try
            {
                Num2Str num = new Num2Str();
                dong.Add("Bằng chữ|" + num.NumberToString(ph.TongTien.ToString()));
            }
            catch
            {
                dong.Add("Bằng chữ|");
            }

            PhieuIn.Mo("IN PHIẾU BÁN HÀNG", cuaHang, dong.ToArray(), this);
        }

        class PhieuItem
        {
            public PhieuBan Phieu;
            public string Text;
            public PhieuItem(PhieuBan p, string text) { Phieu = p; Text = text; }
            public override string ToString() { return Text; }
        }
    }
}
