using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace CuahangNongduoc
{
    /// <summary>
    /// Khung vẽ biểu đồ cột / đường / tròn dùng chung.
    /// REFACTOR #63 Form Template Method – BieuDoBan (frmSoLuongBan) và BieuDoDoanhThu (frmDoanhThu)
    /// trước đây mỗi lớp có một OnPaint ~85 dòng gần giống nhau. Nay OnPaint (khuôn mẫu) nằm ở đây;
    /// lớp con chỉ cung cấp các bước khác nhau: gom dữ liệu, câu báo khi trống, tiêu đề.
    /// </summary>
    public abstract class BieuDoCot : Panel
    {
        public const int KIEU_COT = 0;
        public const int KIEU_DUONG = 1;
        public const int KIEU_TRON = 2;

        /// <summary>0 = cột, 1 = đường, 2 = tròn.</summary>
        public int Kieu;

        /// <summary>Bảng dữ liệu nguồn.</summary>
        public DataTable Bang;

        private static readonly Color[] s_Mau = new Color[] {
            Color.FromArgb(46, 125, 90), Color.FromArgb(33, 115, 185), Color.DarkOrange,
            Color.MediumPurple, Color.IndianRed, Color.Teal, Color.Goldenrod, Color.DimGray
        };

        protected BieuDoCot()
        {
            this.DoubleBuffered = true;
            this.Resize += delegate { Invalidate(); };
        }

        // ---- Các bước lớp con cung cấp ----

        /// <summary>Gom bảng nguồn thành các cặp (nhãn, giá trị) theo thứ tự vẽ.</summary>
        protected abstract List<KeyValuePair<string, long>> GomDuLieu(DataTable bang);

        /// <summary>Câu hiện khi không có dữ liệu.</summary>
        protected abstract string ThongBaoKhiTrong { get; }

        /// <summary>Tiêu đề vẽ ở góc trên (rỗng = không vẽ).</summary>
        protected virtual string TieuDe
        {
            get { return ""; }
        }

        // ---- Khuôn mẫu ----

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.Clear(Color.White);

            if (Bang == null || Bang.Rows.Count == 0)
            {
                using (Font f = new Font("Segoe UI", 9f))
                    g.DrawString(ThongBaoKhiTrong, f, Brushes.Gray, 12, 12);
                return;
            }

            List<KeyValuePair<string, long>> duLieu = GomDuLieu(Bang);
            if (duLieu.Count == 0) return;

            int lechTren = 16;
            if (TieuDe.Length > 0)
            {
                using (Font f = new Font("Segoe UI", 8f, FontStyle.Bold))
                    g.DrawString(TieuDe, f, Brushes.DarkGreen, 8, 4);
                lechTren = 20;
            }
            Rectangle vung = new Rectangle(36, lechTren, Math.Max(20, Width - 52), Math.Max(20, Height - 32 - lechTren));

            if (Kieu == KIEU_TRON)
                VeTron(g, vung, duLieu);
            else
                VeCotHoacDuong(g, vung, duLieu);
        }

        private static void VeTron(Graphics g, Rectangle vung, List<KeyValuePair<string, long>> duLieu)
        {
            float tong = 0;
            foreach (KeyValuePair<string, long> kv in duLieu) tong += kv.Value;
            if (tong <= 0) tong = 1;
            float goc = -90;
            int canh = Math.Min(vung.Width, vung.Height) - 10;
            Rectangle tron = new Rectangle(vung.X + 10, vung.Y, canh, canh);
            for (int i = 0; i < duLieu.Count; i++)
            {
                float quet = (float)(duLieu[i].Value * 360.0 / tong);
                using (SolidBrush b = new SolidBrush(s_Mau[i % s_Mau.Length]))
                    g.FillPie(b, tron, goc, quet);
                goc += quet;
            }
        }

        private void VeCotHoacDuong(Graphics g, Rectangle vung, List<KeyValuePair<string, long>> duLieu)
        {
            long max = 1;
            foreach (KeyValuePair<string, long> kv in duLieu) if (kv.Value > max) max = kv.Value;

            int n = duLieu.Count;
            float bw = Math.Max(6, (vung.Width - 8) / (float)n);
            using (Pen truc = new Pen(Color.Silver))
                g.DrawLine(truc, vung.Left, vung.Bottom, vung.Right, vung.Bottom);

            List<PointF> diem = new List<PointF>();
            for (int i = 0; i < n; i++)
            {
                float h = (float)duLieu[i].Value / max * (vung.Height - 10);
                float x = vung.Left + i * bw;
                if (Kieu == KIEU_COT)
                {
                    using (SolidBrush b = new SolidBrush(s_Mau[i % s_Mau.Length]))
                        g.FillRectangle(b, x + 2, vung.Bottom - h, Math.Max(4, bw - 6), h);
                }
                diem.Add(new PointF(x + bw / 2f, vung.Bottom - h));
            }
            if (Kieu == KIEU_DUONG && diem.Count > 1)
            {
                using (Pen p = new Pen(Color.FromArgb(33, 115, 185), 2f))
                    g.DrawLines(p, diem.ToArray());
                foreach (PointF pt in diem)
                    g.FillEllipse(Brushes.DarkOrange, pt.X - 3, pt.Y - 3, 6, 6);
            }
        }
    }
}
