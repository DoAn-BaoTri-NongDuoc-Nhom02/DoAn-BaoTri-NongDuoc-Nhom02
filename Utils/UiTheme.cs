using System;
using System.Drawing;
using System.Windows.Forms;

namespace CuahangNongduoc
{
    /// <summary>
    /// Giao diện thống nhất cho toàn bộ form WinForms.
    /// Gọi UiTheme.Apply(this) trong Form_Load (sau InitializeComponent / bind dữ liệu).
    /// </summary>
    public static class UiTheme
    {
        // Bảng màu
        public static readonly Color NenForm = Color.FromArgb(245, 250, 247);
        public static readonly Color NenToolbar = Color.FromArgb(200, 235, 210);
        public static readonly Color NenHeader = Color.FromArgb(46, 125, 90);
        public static readonly Color ChuHeader = Color.White;
        public static readonly Color LuoiNen = Color.White;
        public static readonly Color LuoiDongChan = Color.FromArgb(236, 248, 240);
        public static readonly Color LuoiHeader = Color.FromArgb(56, 142, 100);
        public static readonly Color LuoiChon = Color.FromArgb(129, 199, 132);
        public static readonly Color ChuChinh = Color.FromArgb(33, 37, 41);
        public static readonly Color CanhBao = Color.FromArgb(198, 40, 40);
        public static readonly Color ThanhCong = Color.FromArgb(27, 94, 32);

        public static readonly Font FontChinh = new Font("Segoe UI", 9.5f, FontStyle.Regular);
        public static readonly Font FontTieuDe = new Font("Segoe UI Semibold", 11f, FontStyle.Bold);
        public static readonly Font FontHeaderLuoi = new Font("Segoe UI Semibold", 9.5f, FontStyle.Bold);

        /// <summary>Áp dụng theme cho cả form (gọi 1 lần trong Load).</summary>
        public static void Apply(Form form)
        {
            if (form == null) return;
            try
            {
                form.Font = FontChinh;
                form.BackColor = NenForm;
                form.ForeColor = ChuChinh;
                form.StartPosition = FormStartPosition.CenterScreen;
                // Double-buffer giảm nháy
                typeof(Control).GetProperty("DoubleBuffered",
                    System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)
                    ?.SetValue(form, true, null);

                foreach (Control c in form.Controls)
                    StyleControl(c);
            }
            catch { /* không làm hỏng form nếu thiếu control */ }
        }

        /// <summary>Chỉ làm đẹp lưới (khi form phức tạp, gọi riêng).</summary>
        public static void StyleGrid(DataGridView dgv)
        {
            if (dgv == null) return;
            dgv.BackgroundColor = LuoiNen;
            dgv.BorderStyle = BorderStyle.None;
            dgv.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgv.GridColor = Color.FromArgb(220, 230, 225);
            dgv.RowHeadersVisible = false;
            dgv.EnableHeadersVisualStyles = false;
            dgv.ColumnHeadersDefaultCellStyle.BackColor = LuoiHeader;
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = ChuHeader;
            dgv.ColumnHeadersDefaultCellStyle.Font = FontHeaderLuoi;
            dgv.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgv.ColumnHeadersHeight = 36;
            dgv.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dgv.DefaultCellStyle.Font = FontChinh;
            dgv.DefaultCellStyle.SelectionBackColor = LuoiChon;
            dgv.DefaultCellStyle.SelectionForeColor = ChuChinh;
            dgv.DefaultCellStyle.Padding = new Padding(4, 2, 4, 2);
            dgv.AlternatingRowsDefaultCellStyle.BackColor = LuoiDongChan;
            dgv.RowTemplate.Height = 30;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.MultiSelect = false;
            dgv.AllowUserToResizeRows = false;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        public static void StyleNavigator(BindingNavigator nav)
        {
            if (nav == null) return;
            nav.BackColor = NenToolbar;
            nav.Font = FontChinh;
            nav.GripStyle = ToolStripGripStyle.Hidden;
            nav.ImageScalingSize = new Size(24, 24);
            try { nav.CountItemFormat = "/ {0} bản ghi"; } catch { }
            foreach (ToolStripItem it in nav.Items)
            {
                it.Font = FontChinh;
                // Nút có cả icon + chữ dễ nhận
                if (it is ToolStripButton btn && btn.DisplayStyle == ToolStripItemDisplayStyle.Image)
                {
                    if (!string.IsNullOrEmpty(btn.Text) && btn.Text.Length > 1
                        && !btn.Text.StartsWith("Move") && btn.Text != "Delete" && btn.Text != "Add new")
                    {
                        btn.DisplayStyle = ToolStripItemDisplayStyle.ImageAndText;
                        btn.TextImageRelation = TextImageRelation.ImageAboveText;
                    }
                }
            }
        }

        public static void StyleToolStrip(ToolStrip ts)
        {
            if (ts == null) return;
            ts.BackColor = NenToolbar;
            ts.Font = FontChinh;
            ts.ImageScalingSize = new Size(24, 24);
            ts.GripStyle = ToolStripGripStyle.Hidden;
        }

        public static void StyleStatusStrip(StatusStrip ss)
        {
            if (ss == null) return;
            ss.BackColor = Color.FromArgb(232, 245, 233);
            ss.Font = FontChinh;
        }

        public static void StyleGroupBox(GroupBox gb)
        {
            if (gb == null) return;
            gb.Font = new Font("Segoe UI Semibold", 9.5f, FontStyle.Bold);
            gb.ForeColor = NenHeader;
            gb.BackColor = Color.FromArgb(240, 248, 243);
            foreach (Control c in gb.Controls)
                StyleControl(c);
        }

        public static void StyleButton(Button btn)
        {
            if (btn == null) return;
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderColor = Color.FromArgb(76, 175, 80);
            btn.FlatAppearance.BorderSize = 1;
            btn.BackColor = Color.FromArgb(232, 245, 233);
            btn.ForeColor = ChuChinh;
            btn.Font = FontChinh;
            btn.Cursor = Cursors.Hand;
            btn.Height = Math.Max(btn.Height, 30);
        }

        private static void StyleControl(Control c)
        {
            if (c is DataGridView dgv) StyleGrid(dgv);
            else if (c is BindingNavigator bn) StyleNavigator(bn);
            else if (c is ToolStrip ts && !(c is StatusStrip) && !(c is MenuStrip)) StyleToolStrip(ts);
            else if (c is StatusStrip ss) StyleStatusStrip(ss);
            else if (c is GroupBox gb) StyleGroupBox(gb);
            else if (c is Button btn) StyleButton(btn);
            else if (c is Label lbl)
            {
                lbl.ForeColor = ChuChinh;
                if (lbl.Font.Bold) lbl.ForeColor = NenHeader;
            }
            else if (c is TextBox || c is ComboBox || c is NumericUpDown || c is DateTimePicker)
            {
                c.Font = FontChinh;
            }
            else if (c is Panel || c is SplitContainer || c is TabControl || c is TabPage)
            {
                foreach (Control child in c.Controls)
                    StyleControl(child);
            }

            // Đệ quy container còn lại
            if (c.HasChildren && !(c is DataGridView) && !(c is GroupBox) && !(c is ToolStrip))
            {
                foreach (Control child in c.Controls)
                    StyleControl(child);
            }
        }

        /// <summary>Gán icon + chữ cho ToolStripButton nếu còn trống.</summary>
        public static void SetNavButton(ToolStripButton btn, string text, Image icon)
        {
            if (btn == null) return;
            btn.Text = text;
            if (icon != null) btn.Image = icon;
            btn.DisplayStyle = ToolStripItemDisplayStyle.ImageAndText;
            btn.TextImageRelation = TextImageRelation.ImageAboveText;
            btn.AutoSize = true;
        }
    }
}
