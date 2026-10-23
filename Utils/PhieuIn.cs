using System;
using System.Drawing;
using System.Drawing.Printing;
using System.Windows.Forms;

namespace CuahangNongduoc
{
    public static class PhieuIn
    {
        public static void Mo(string tieuDe, string cuaHang, string[] dong, IWin32Window owner)
        {
            PrintDocument doc = new PrintDocument();
            doc.DocumentName = tieuDe;
            doc.PrintPage += delegate(object s, PrintPageEventArgs ev)
            {
                Graphics g = ev.Graphics;
                Rectangle page = ev.MarginBounds;
                int x = page.Left;
                int y = page.Top;
                int w = page.Width;
                using (Pen vien = new Pen(Color.Black, 1.4f))
                using (Font fCua = new Font("Tahoma", 12f, FontStyle.Bold))
                using (Font fTieu = new Font("Tahoma", 20f, FontStyle.Bold))
                using (Font f = new Font("Tahoma", 13f))
                using (Font fNho = new Font("Tahoma", 11f, FontStyle.Italic))
                {
                    StringFormat giua = new StringFormat();
                    giua.Alignment = StringAlignment.Center;
                    g.DrawString(cuaHang, fCua, Brushes.Black, new RectangleF(x, y, w, 22), giua);
                    y += 26;
                    g.DrawString(tieuDe, fTieu, Brushes.Black, new RectangleF(x, y, w, 36), giua);
                    y += 40;
                    g.DrawLine(vien, x + 80, y, x + w - 80, y);
                    y += 16;

                    int tableTop = y;
                    int rowH = 32;
                    int labelW = 180;
                    g.DrawRectangle(vien, x, tableTop, w, rowH * dong.Length);
                    for (int i = 0; i < dong.Length; i++)
                    {
                        int ry = tableTop + i * rowH;
                        if (i > 0) g.DrawLine(vien, x, ry, x + w, ry);
                        g.DrawLine(vien, x + labelW, ry, x + labelW, ry + rowH);
                        string[] kv = (dong[i] ?? "").Split(new char[] { '|' }, 2);
                        string lb = kv.Length > 0 ? kv[0] : "";
                        string vl = kv.Length > 1 ? kv[1] : "";
                        RectangleF r1 = new RectangleF(x + 8, ry + 6, labelW - 12, rowH - 8);
                        RectangleF r2 = new RectangleF(x + labelW + 8, ry + 6, w - labelW - 16, rowH - 8);
                        g.DrawString(lb, f, Brushes.Black, r1);
                        using (Font fb = new Font("Tahoma", 13f, FontStyle.Bold))
                            g.DrawString(vl, lb.IndexOf("tiền", StringComparison.OrdinalIgnoreCase) >= 0 ? fb : f, Brushes.Black, r2);
                    }
                    y = tableTop + rowH * dong.Length + 28;
                    g.DrawString("Ngày in: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm"), fNho, Brushes.Black, x, y);
                    y += 50;
                    int cot = w / 3;
                    g.DrawString("Người nộp", f, Brushes.Black, new RectangleF(x, y, cot, 24), giua);
                    g.DrawString("Người lập phiếu", f, Brushes.Black, new RectangleF(x + cot, y, cot, 24), giua);
                    g.DrawString("Thủ quỹ", f, Brushes.Black, new RectangleF(x + cot * 2, y, cot, 24), giua);
                    g.DrawString("(Ký, ghi rõ họ tên)", fNho, Brushes.Black, new RectangleF(x, y + 22, cot, 20), giua);
                    g.DrawString("(Ký, ghi rõ họ tên)", fNho, Brushes.Black, new RectangleF(x + cot, y + 22, cot, 20), giua);
                    g.DrawString("(Ký, ghi rõ họ tên)", fNho, Brushes.Black, new RectangleF(x + cot * 2, y + 22, cot, 20), giua);
                }
            };
            using (PrintPreviewDialog pv = new PrintPreviewDialog())
            {
                pv.Document = doc;
                pv.Text = tieuDe;
                pv.Width = 900;
                pv.Height = 700;
                pv.ShowDialog(owner);
            }
        }
    }
}
