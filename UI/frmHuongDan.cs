using System;
using System.Drawing;
using System.Windows.Forms;

namespace CuahangNongduoc
{
    public class frmHuongDan : Form
    {
        public frmHuongDan()
        {
            this.Text = "Hướng dẫn sử dụng";
            this.Size = new Size(720, 520);
            this.StartPosition = FormStartPosition.CenterParent;
            this.MaximizeBox = true;
            this.BackColor = Color.FromArgb(236, 248, 236);

            TextBox box = new TextBox
            {
                Multiline = true,
                ReadOnly = true,
                ScrollBars = ScrollBars.Vertical,
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10f),
                BorderStyle = BorderStyle.None,
                BackColor = Color.FromArgb(245, 250, 247)
            };
            box.Text =
@"HƯỚNG DẪN SỬ DỤNG – CỬA HÀNG NÔNG DƯỢC
========================================

1. ĐĂNG NHẬP
   - Dùng tài khoản Admin / nhân viên kho / bán hàng / kế toán.

2. QUẢN LÝ DANH MỤC
   - Sản phẩm: thêm/sửa/xóa, đơn vị tính, giá nhập – bán.
   - Khách hàng / Đại lý: thông tin, hạn mức công nợ, trạng thái.
   - Nhà cung cấp: thông tin NCC dùng khi nhập hàng.

3. NHẬP HÀNG
   - Tạo phiếu nhập → chọn NCC → chọn sản phẩm, số lượng, giá.
   - Bấm «Thêm vào» để thêm dòng chi tiết → Lưu.
   - Lưới dưới chỉ hiện khi phiếu đã có dòng MA_SAN_PHAM.

4. BÁN SỈ / BÁN LẺ
   - Mở danh sách phiếu → Thêm mới / double-click để sửa.
   - Công nợ = Tổng tiền – Đã trả.

5. PHIẾU THU / PHIẾU CHI
   - Thu: thu tiền khách/đại lý.
   - Chi: chi phí theo lý do chi.

6. BÁO CÁO
   - Số lượng tồn: tồn theo từng sản phẩm.
   - Số lượng bán / SP hết hạn: nếu máy thiếu ReportViewer sẽ hiện bảng dữ liệu.
   - Dư nợ: chọn tháng/năm → bấm «Tổng hợp».

7. TRỢ GIÚP
   - Menu Trợ giúp → Hướng dẫn sử dụng (F1).
   - Thông tin liên hệ: cửa hàng mẫu / bộ môn.

Ghi chú: In phiếu cần Microsoft Report Viewer trên máy.
";
            Button dong = new Button
            {
                Text = "Đóng",
                Dock = DockStyle.Bottom,
                Height = 36
            };
            dong.Click += (s, e) => this.Close();
            this.Controls.Add(box);
            this.Controls.Add(dong);
        }
    }
}
