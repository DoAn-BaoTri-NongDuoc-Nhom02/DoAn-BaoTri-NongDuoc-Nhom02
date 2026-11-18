using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using CuahangNongduoc.BanHang;
using CuahangNongduoc.BLL.Service;
using CuahangNongduoc.BusinessObject;
using CuahangNongduoc.ThongKe;

namespace CuahangNongduoc
{
    /// <summary>
    /// Thống kê theo khoảng ngày (yêu cầu 4): tồn kho, chi phí vận chuyển + dịch vụ phụ,
    /// hóa đơn giảm giá / khuyến mãi, giảm giá theo nhân viên. Xem trên lưới hoặc xuất CSV để in.
    /// Mỗi thống kê là một Template Method, bọc trong Proxy phân quyền, xuất qua Bridge (lưới / CSV).
    /// Form cũng là một bên nghe (Observer) sự kiện "hóa đơn đã lưu" để tự làm mới.
    /// </summary>
    // PATTERN #19 Observer (nơi dùng) – form là một bên nghe (IQuanSatHoaDon), tự làm mới khi có hóa đơn mới.
    public class frmThongKe : Form, IQuanSatHoaDon
    {
        private readonly DateTimePicker dtTuNgay = new DateTimePicker();
        private readonly DateTimePicker dtDenNgay = new DateTimePicker();
        private readonly ComboBox cboLoai = new ComboBox();
        private readonly Button btnXem = new Button();
        private readonly Button btnXuatCsv = new Button();
        private readonly Label lblTieuDe = new Label();
        private readonly DataGridView luoi = new DataGridView();
        private bool m_DaXem;

        public frmThongKe()
        {
            this.Text = "Thống kê theo ngày";
            this.ClientSize = new Size(900, 520);
            this.StartPosition = FormStartPosition.CenterParent;

            Panel thanhLoc = new Panel();
            thanhLoc.Dock = DockStyle.Top;
            thanhLoc.Height = 72;
            this.Controls.Add(thanhLoc);

            ThemNhan(thanhLoc, "Loại thống kê:", 12, 12);
            cboLoai.DropDownStyle = ComboBoxStyle.DropDownList;
            cboLoai.SetBounds(110, 8, 330, 24);
            cboLoai.Items.AddRange(new object[] {
                "Tồn kho hàng hóa",
                "Chi phí vận chuyển và dịch vụ phụ",
                "Hóa đơn giảm giá, khuyến mãi",
                "Hóa đơn giảm giá theo nhân viên" });
            cboLoai.SelectedIndex = 0;
            thanhLoc.Controls.Add(cboLoai);

            ThemNhan(thanhLoc, "Từ ngày:", 12, 44);
            dtTuNgay.Format = DateTimePickerFormat.Short;
            dtTuNgay.SetBounds(110, 40, 120, 24);
            dtTuNgay.Value = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            thanhLoc.Controls.Add(dtTuNgay);

            ThemNhan(thanhLoc, "Đến ngày:", 246, 44);
            dtDenNgay.Format = DateTimePickerFormat.Short;
            dtDenNgay.SetBounds(320, 40, 120, 24);
            dtDenNgay.Value = DateTime.Today;
            thanhLoc.Controls.Add(dtDenNgay);

            btnXem.Text = "Xem";
            btnXem.SetBounds(460, 8, 100, 56);
            btnXem.Click += delegate { XuatRa(new XuatRaLuoi(luoi, lblTieuDe)); };
            thanhLoc.Controls.Add(btnXem);

            btnXuatCsv.Text = "Xuất CSV / In";
            btnXuatCsv.SetBounds(570, 8, 120, 56);
            btnXuatCsv.Click += delegate { XuatRa(new XuatRaCsv(this)); };
            thanhLoc.Controls.Add(btnXuatCsv);

            lblTieuDe.Dock = DockStyle.Top;
            lblTieuDe.Height = 28;
            lblTieuDe.Font = new Font(this.Font.FontFamily, 11f, FontStyle.Bold);
            lblTieuDe.TextAlign = ContentAlignment.MiddleLeft;

            luoi.Dock = DockStyle.Fill;
            luoi.ReadOnly = true;
            luoi.AllowUserToAddRows = false;
            luoi.AllowUserToDeleteRows = false;
            luoi.RowHeadersVisible = false;
            luoi.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            this.Controls.Add(luoi);
            this.Controls.Add(lblTieuDe);
            thanhLoc.BringToFront();
            lblTieuDe.BringToFront();
            luoi.BringToFront();

            this.Load += delegate
            {
                UiTheme.Apply(this);
                SuKienHoaDon.DangKy(this);
            };
            this.FormClosed += delegate { SuKienHoaDon.HuyDangKy(this); };
        }

        private static void ThemNhan(Control chua, String chu, int x, int y)
        {
            Label l = new Label();
            l.Text = chu;
            l.AutoSize = true;
            l.Location = new Point(x, y);
            chua.Controls.Add(l);
        }

        /// <summary>Thống kê đang chọn, bọc trong proxy phân quyền.</summary>
        private IThongKe ThongKeDangChon()
        {
            ThongKeTheoKhoangNgay that;
            switch (cboLoai.SelectedIndex)
            {
                case 1: that = new ThongKeChiPhiVanChuyenDichVu(); break;
                case 2: that = new ThongKeGiamGiaKhuyenMai(); break;
                case 3: that = new ThongKeGiamGiaTheoNhanVien(); break;
                default: that = new ThongKeTonKho(); break;
            }
            // PATTERN #22 Template Method (nơi dùng) – mỗi lớp thống kê chỉ viết bước TruyVan, khung ThucHien dùng chung.
            // PATTERN #12 Proxy (nơi dùng) – bọc thống kê thật trong proxy kiểm tra quyền.
            return new ThongKeBaoVe(that);
        }

        private void XuatRa(IKenhXuat kenh)
        {
            KhoangNgay khoang;
            try
            {
                khoang = new KhoangNgay(dtTuNgay.Value, dtDenNgay.Value);
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                Cursor = Cursors.WaitCursor;
                IThongKe thongKe = ThongKeDangChon();
                // PATTERN #7 Bridge (nơi dùng) – cùng một thống kê, đổi kênh xuất (lưới / CSV) mà không cần lớp mới.
                thongKe.KenhXuat = kenh;
                thongKe.Xuat(khoang);
                m_DaXem = true;
            }
            catch (NghiepVuException ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        /// <summary>Observer: có hóa đơn mới lưu thì xem lại thống kê đang hiện.</summary>
        public void HoaDonDaLuu(PhieuBan phieu)
        {
            if (m_DaXem && !IsDisposed)
                XuatRa(new XuatRaLuoi(luoi, lblTieuDe));
        }
    }
}
