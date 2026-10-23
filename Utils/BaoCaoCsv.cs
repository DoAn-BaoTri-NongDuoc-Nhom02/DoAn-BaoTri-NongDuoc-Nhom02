using System;
using System.Data;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace CuahangNongduoc
{
    public static class BaoCaoCsv
    {
        public static void Xuat(DataGridView grid, string tenMacDinh, IWin32Window owner)
        {
            if (grid == null)
            {
                MessageBox.Show("Không có bảng dữ liệu để xuất.", "Xuất file",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "CSV Excel (*.csv)|*.csv|Tất cả (*.*)|*.*";
            dlg.FileName = tenMacDinh;
            if (dlg.ShowDialog(owner) != DialogResult.OK) return;

            StringBuilder sb = new StringBuilder();
            bool first = true;
            for (int c = 0; c < grid.Columns.Count; c++)
            {
                if (!grid.Columns[c].Visible) continue;
                if (!first) sb.Append(",");
                first = false;
                sb.Append(O(grid.Columns[c].HeaderText));
            }
            sb.AppendLine();

            foreach (DataGridViewRow row in grid.Rows)
            {
                if (row.IsNewRow) continue;
                first = true;
                for (int c = 0; c < grid.Columns.Count; c++)
                {
                    if (!grid.Columns[c].Visible) continue;
                    if (!first) sb.Append(",");
                    first = false;
                    object v = row.Cells[c].FormattedValue;
                    if (v == null) v = row.Cells[c].Value;
                    sb.Append(O(v == null ? "" : Convert.ToString(v)));
                }
                sb.AppendLine();
            }
            File.WriteAllText(dlg.FileName, sb.ToString(), Encoding.UTF8);
            MessageBox.Show("Đã xuất file:\n" + dlg.FileName, "Xuất file",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public static void Xuat(DataTable table, string tenMacDinh, IWin32Window owner)
        {
            if (table == null)
            {
                MessageBox.Show("Không có dữ liệu để xuất.", "Xuất file",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "CSV Excel (*.csv)|*.csv|Tất cả (*.*)|*.*";
            dlg.FileName = tenMacDinh;
            if (dlg.ShowDialog(owner) != DialogResult.OK) return;

            StringBuilder sb = new StringBuilder();
            for (int c = 0; c < table.Columns.Count; c++)
            {
                if (c > 0) sb.Append(",");
                sb.Append(O(table.Columns[c].ColumnName));
            }
            sb.AppendLine();
            foreach (DataRow row in table.Rows)
            {
                for (int c = 0; c < table.Columns.Count; c++)
                {
                    if (c > 0) sb.Append(",");
                    object v = row[c];
                    sb.Append(O(v == null || v == DBNull.Value ? "" : Convert.ToString(v)));
                }
                sb.AppendLine();
            }
            File.WriteAllText(dlg.FileName, sb.ToString(), Encoding.UTF8);
            MessageBox.Show("Đã xuất file:\n" + dlg.FileName, "Xuất file",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        static string O(string s)
        {
            if (s == null) s = "";
            if (s.IndexOfAny(new char[] { ',', '"', '\n', '\r' }) >= 0)
                return "\"" + s.Replace("\"", "\"\"") + "\"";
            return s;
        }
    }
}
