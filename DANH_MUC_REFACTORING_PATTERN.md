# Danh mục Refactoring & Design Pattern – Cửa hàng Nông dược (Nhóm 02)

File này được sinh tự động từ các nhãn trong mã nguồn. Mỗi chỗ sửa trong code có một dòng chú thích bắt đầu bằng
`REFACTOR #<số> <tên kỹ thuật>` hoặc `PATTERN #<số> <tên mẫu>`, theo sau là ý nghĩa của thay đổi.

**Cách tìm trong Visual Studio:** nhấn `Ctrl+Shift+F`, gõ `REFACTOR #12` (hoặc `PATTERN #10`) rồi Find All, nhấp đúp vào kết quả để nhảy tới dòng đó.
Muốn thấy tất cả trong cửa sổ **Task List** (View → Task List): vào Tools → Options → Environment → Task List, thêm hai token `REFACTOR` và `PATTERN`.

Cột "Loại": **Cài đặt** = nơi viết mẫu / kỹ thuật; **Nơi dùng** = chỗ form hoặc lớp khác gọi tới nó.

## A. 66 kỹ thuật Refactoring (theo catalog refactoring.guru)

| # | Kỹ thuật | Vị trí (file : dòng) | Ý nghĩa của thay đổi |
| --- | --- | --- | --- |
| 1 | Extract Method | `UI/frmMain.cs` : 42 | thay cho 19 khối if/else giống hệt nhau trong các hàm mnu..._Click. |
| 2 | Inline Method | `UI/frmDanhsachPhieuBanSi.cs` : 485 | handler toolPrint_Click (chỉ gọi lại toolIn_Click) đã bị bỏ. |
|  |  | `UI/frmDunoKhachhang.cs` : 416 | handler btnTongHop_Click (chỉ gọi lại toolTongHop_Click) đã bị bỏ. |
|  |  | `UI/frmMain.cs` : 466 | handler itemQuanLyTaiKhoan_Click (chỉ gọi lại hàm này) đã bị bỏ; mục itemQuanLyTaiKhoan trong frmMain.Designer.cs nay gắn thẳng vào mnuQuanLyTaiKhoan_Click. |
| 3 | Extract Variable | `UI/frmNhapHang.cs` : 602 | tách biểu thức lồng nhau thành các biến có tên nói rõ ý nghĩa. |
| 4 | Inline Temp | `BLL/DonViTinhController.cs` : 22 | bỏ biến tạm "tbl" chỉ dùng đúng một lần, gán thẳng kết quả cho DataSource. |
| 5 | Replace Temp with Query | `UI/frmBanLe.cs` : 625 | trước đây là biến tạm tienHang cộng dồn ngay trong hàm tính tổng. |
|  |  | `UI/frmBanSi.cs` : 643 | trước đây là biến tạm tienHang cộng dồn ngay trong hàm tính tổng. |
| 6 | Split Temporary Variable | `Utils/Num2Str.cs` : 120 | trước đây một biến "s" lần lượt giữ 3 thứ khác nhau (phần chữ của số, chuỗi có dấu "âm", phần còn lại sau chữ hoa đầu). Nay mỗi thứ một biến. |
| 7 | Remove Assignments to Parameters | `Utils/Num2Str.cs` : 180 | không gán lại tham số "no" (trước đây bị gán 6 lần), mọi bước chuẩn hóa làm trên biến cục bộ chuoiSo. |
| 8 | Replace Method with Method Object | `BLL/TruyVanDoanhThu.cs` : 11 | trước đây là hàm frmDoanhThu.DocDoanhThu dài 114 dòng với nhiều biến cục bộ và câu SQL viết thẳng trong form. Nay các tham số thành trường của lớp, phần tính giá vốn và tổng chi là hàm riêng của lớp, form chỉ gọi ThucHien(). |
| 9 | Substitute Algorithm | `Utils/Num2Str.cs` : 157 | thay vòng lặp tự cắt từng nhóm 3 chữ số (và vá dấu "-,") bằng định dạng "#,##0" có sẵn của .NET; phần thập phân giữ nguyên. |
| 10 | Move Method | `Entities/ThongTinHoaDon.cs` : 76 | công thức trước đây nằm trong frmBanLe/frmBanSi. |
| 11 | Move Field | `Entities/DaiLy.cs` : 16 | tên các cấp đại lý trước đây là trường KhachHangController.NhanCapDaiLy (tầng điều khiển), nhưng là dữ liệu của đại lý nên chuyển về lớp DaiLy. |
| 12 | Extract Class | `DAL/DataAccess/NhatKyLoi.cs` : 9 | tách ra từ DataService để DataService chỉ lo việc truy vấn. |
| 13 | Inline Class | `DAL/Repositories/NguoiDungRepository.cs` : 12 | lớp DbHelper (chỉ có 1 hàm, chỉ lớp này dùng) được gộp vào đây. |
| 14 | Hide Delegate | `Entities/MaSanPham.cs` : 102 | bên ngoài không cần đi xuyên lo.SanPham.DonGiaNhap. |
| 15 | Remove Middle Man | `DAL/Repositories/ChiTietPhieuBanFactory.cs` : 46 | thay cho cặp NewRow()/Add() chỉ chuyển tiếp sang DataTable. |
|  |  | `DAL/Repositories/PhieuBanFactory.cs` : 83 | thay cho cặp NewRow()/Add() chỉ chuyển tiếp sang DataTable. |
| 16 | Introduce Foreign Method | `DAL/DataAccess/DuLieu.cs` : 8 | DataRow là lớp của .NET (không sửa được) và .NET 2.0 không có extension method, nên các hàm "lẽ ra thuộc DataRow" được gom ở đây thay cho các biểu thức "x == DBNull.Value ? ... : Convert..." lặp khắp nơi. |
| 17 | Introduce Local Extension | `Utils/TienUpDown.cs` : 8 | NumericUpDown là lớp của .NET không sửa được; trước đây chỗ nào cần nhập tiền cũng phải chỉnh lại Maximum / ThousandsSeparator. |
| 18 | Self Encapsulate Field | `DAL/DataAccess/DataService.cs` : 30 | kết nối đang dùng, luôn ở trạng thái đã mở. Trước đây các hàm Execute… dùng thẳng m_Connection nên bị NullReferenceException nếu chưa ai gọi OpenConnection() trước. |
| 19 | Replace Data Value with Object | `Entities/ThongTinLienLac.cs` : 6 | Số điện thoại, email, mã số thuế trước đây chỉ là chuỗi, còn luật kiểm tra nằm rải rác (KhachHangController và NhaCungCapController mỗi nơi một bản Regex giống nhau). Nay mỗi loại là một đối tượng giá trị: chỉ tạo được khi hợp lệ và tự giữ luật của mình. |
| 20 | Change Value to Reference | `BLL/KhachHangController.cs` : 129 | Trước đây mỗi lần LayKhachHang(id) lại đọc CSDL và tạo một đối tượng mới, nên danh sách phiếu bán sỉ truy vấn lại cho từng dòng và cùng một khách có nhiều bản sao khác nhau. Nay mỗi mã khách chỉ có MỘT đối tượng, lấy từ bộ đệm; Save() làm mới bộ đệm. |
| 21 | Change Reference to Value | `Entities/DonViTinh.cs` : 7 | đơn vị tính (Chai, Gói, Kg...) không có "danh tính" riêng, hai đối tượng cùng mã + tên là một. Bỏ setter (bất biến) và so sánh theo nội dung (Equals/GetHashCode), nên có thể dùng chung một đối tượng cho nhiều sản phẩm (xem Flyweight trong DonViTinhController). |
| 22 | Replace Array with Object | `BLL/BoLocCongNo.cs` : 7 | trước đây là mảng chuỗi { "Tất cả", "Đã thanh toán", "Còn nợ" } rồi so sánh chuỗi đã chọn ở 2 nơi trong mỗi form. |
| 23 | Duplicate Observed Data | `Entities/ThongTinHoaDon.cs` : 9 | trước đây các giá trị nghiệp vụ này là hàng loạt trường m_ChietKhau, m_PhiVC, m_VAT... nằm trong form frmBanLe / frmBanSi; nay form chỉ hiển thị và chép qua lại với đối tượng nghiệp vụ này. |
| 24 | Change Unidirectional Association to Bidirectional | `Entities/MaSanPham.cs` : 47 | giữ hai chiều liên kết lô <-> sản phẩm luôn khớp nhau |
|  |  | `Entities/SanPham.cs` : 68 | trước đây chỉ có chiều MaSanPham.SanPham (lô -> sản phẩm). Thêm chiều ngược SanPham.DanhSachLo để từ sản phẩm biết các lô của nó (tính tồn theo lô, lô sắp hết hạn). Chiều ngược được giữ đồng bộ trong setter MaSanPham.SanPham. |
| 25 | Change Bidirectional Association to Unidirectional | `Entities/ChiTietPhieuBan.cs` : 9 | Đã bỏ con trỏ ngược ChiTietPhieuBan.PhieuBan: không nơi nào gán hay đọc nó, chiều PhieuBan.ChiTiet -> ChiTietPhieuBan là đủ. |
| 26 | Replace Magic Number with Symbolic Constant | `Entities/NguoiDung.cs` : 9 | số 1 (tài khoản admin gốc, không được xóa) trước đây viết thẳng trong NguoiDungService.Xoa và câu SQL của NguoiDungRepository.Xoa. |
| 27 | Encapsulate Field | `DAL/DataAccess/DataService.cs` : 51 | chuỗi kết nối OleDb; trước đây là trường public static. Đổi chuỗi kết nối thì đóng kết nối cũ để lần sau mở lại bằng chuỗi mới. |
| 28 | Encapsulate Collection | `Entities/PhieuBan.cs` : 63 | Trước đây ChiTiet có setter công khai, nơi khác thay cả danh sách hoặc sửa thẳng bên trong. Nay bên ngoài chỉ đọc được bản chỉ-đọc; muốn thay đổi phải qua các hàm bên dưới. |
|  |  | `Entities/PhieuNhap.cs` : 60 | Trước đây ChiTiet có setter công khai, nơi khác thay cả danh sách hoặc sửa thẳng bên trong. Nay bên ngoài chỉ đọc được bản chỉ-đọc; muốn thay đổi phải qua các hàm bên dưới. |
| 29 | Replace Type Code with Class | `Entities/LoaiVaiTro.cs` : 7 | trước đây vai trò chỉ là số int (1, 2, 3, 4) rải khắp nơi; truyền nhầm một số bất kỳ vẫn biên dịch được. Nay chỉ có 4 đối tượng cố định. |
| 30 | Replace Type Code with Subclasses | `Entities/KhachHang.cs` : 47 | Trước đây loại khách là cờ bool LoaiKH (false = khách lẻ, true = đại lý) do bên ngoài gán. Nay loại khách do lớp con quyết định: DaiLy (khách lẻ là chính KhachHang); LoaiKH chỉ còn để đọc cho code cũ. |
| 31 | Replace Type Code with State/Strategy | `BLL/ChienLuocXuatKho.cs` : 8 | trước đây form bán hàng tự so sánh CauHinhKho.PhuongPhapXuatKho == ChiDinh (4 chỗ) rồi if/else gọi PhanLoChiDinh hay PhanLoFIFO. Nay mỗi phương pháp là một lớp; form chỉ hỏi chiến lược hiện hành. |
| 32 | Replace Subclass with Fields | `BLL/QuyenVaiTro.cs` : 84 | bốn lớp con QuyenBanHang, QuyenKho, QuyenKeToan, QuyenKhongCo chỉ khác nhau ở MỘT mảng hằng (danh sách chức năng), không khác hành vi. Gộp thành một lớp QuyenTheoDanhSach giữ danh sách đó trong trường; QuyenVaiTro.Cua() truyền danh sách cho từng vai trò. |
| 33 | Decompose Conditional | `UI/frmDanhsachPhieuNhap.cs` : 368 | điều kiện suy trạng thái phiếu nhập (trước đây viết thẳng trong vòng lặp của BoSungCotHienThi) tách thành các hàm có tên nói rõ ý nghĩa. |
| 34 | Consolidate Conditional Expression | `Utils/Num2Str.cs` : 187 | hai if cùng trả "không" gộp thành một điều kiện. |
| 35 | Consolidate Duplicate Conditional Fragments | `UI/frmBanLe.cs` : 257 | cả hai nhánh (thêm mới / cập nhật) trước đây đều tự gọi ctrlChiTiet.Save(); nay gọi một lần sau if/else. Phiếu luôn được ghi trước chi tiết (chi tiết tham chiếu tới phiếu). |
|  |  | `UI/frmBanSi.cs` : 276 | cả hai nhánh (thêm mới / cập nhật) trước đây đều tự gọi ctrlChiTiet.Save(); nay gọi một lần sau if/else. Phiếu luôn được ghi trước chi tiết (chi tiết tham chiếu tới phiếu). |
| 36 | Remove Control Flag | `UI/frmNhapHang.cs` : 951 | bỏ cờ "ok": lưu được thì return ngay, không được thì đi thẳng tới phần báo lỗi. |
| 37 | Replace Nested Conditional with Guard Clauses | `UI/frmDanhsachPhieuBanSi.cs` : 282 | Trước đây hai đoạn dưới nằm lồng tới 11 cấp if/try trong CapNhatCotAoPM23 và dùng reflection tìm thuộc tính "HanMucNo" (không tồn tại nên hạn mức không bao giờ hiện). Nay mỗi điều kiện không thỏa thì thoát sớm, và đọc thẳng DaiLy.HanMucCongNo. |
| 38 | Replace Conditional with Polymorphism | `BLL/QuyenVaiTro.cs` : 46 | frmMain.PhanQuyenTheoVaiTro trước đây là một switch 136 dòng bật/tắt từng menu theo số vai trò. Nay mỗi vai trò là một lớp con tự biết mình được dùng chức năng nào; thêm vai trò mới chỉ cần thêm lớp, không sửa frmMain. |
|  |  | `UI/frmMain.cs` : 308 | thay switch theo số vai trò bằng QuyenVaiTro. |
|  |  | `UI/frmMain.cs` : 313 | mỗi vai trò tự trả lời DuocPhep(). |
| 39 | Introduce Null Object | `Entities/NguoiDung.cs` : 42 | trước đây PhienDangNhap.NguoiDungHienTai trả về null và nơi dùng phải tự kiểm tra (frmMain, frmDoiMatKhau...), quên kiểm tra là NullReferenceException. |
| 40 | Introduce Assertion | `DAL/DataAccess/DataService.cs` : 287 | hàm này cập nhật lại đúng câu SELECT đã nạp bảng, nên bắt buộc phải gọi Load(...) trước. |
| 41 | Rename Method | `DAL/DataAccess/DataService.cs` : 284 | đổi tên ExecuteNoneQuery (sai chính tả) thành ExecuteNonQuery, cả hai bản nạp chồng. |
|  |  | `DAL/Repositories/MaSanPhamFactory.cs` : 9 | lớp MaSanPhanFactory (sai chính tả "Phan") đổi tên thành MaSanPhamFactory. |
| 42 | Add Parameter | `BLL/PhieuBanController.cs` : 39 | thêm tham số nguoiLap để ghi ai lập phiếu vào cột ID_NGUOI_DUNG (phục vụ thống kê hóa đơn theo nhân viên). Truyền null nếu không xác định. |
| 43 | Remove Parameter | `UI/frmMain.cs` : 31 | bỏ hàm khởi tạo frmMain(NguoiDung nd): người dùng đã có trong PhienDangNhap (frmDangNhap gọi BatDau trước khi mở form), truyền thêm tham số chỉ gây trùng lặp. Chưa đăng nhập thì form tự đóng ở frmMain_Load. |
| 44 | Separate Query from Modifier | `DAL/DataAccess/DataService.cs` : 375 | ExecuteScalar chỉ đọc, không còn gọi AcceptChanges() làm thay đổi trạng thái bảng đang giữ. |
|  |  | `Utils/CauHinhKho.cs` : 84 | hàm chỉ đọc. Trước đây hàm đọc này có thể chạy CREATE TABLE; nay bảng chưa có thì trả về null để dùng giá trị mặc định, việc tạo bảng chỉ xảy ra trong Gan(). |
| 45 | Parameterize Method | `Utils/ThamSo.cs` : 54 | thay cho 7 cặp hàm/thuộc tính gần như giống hệt nhau (LayMaPhieuNhap/GanMaPhieuNhap, LayMaPhieuBan/GanMaPhieuBan, ..., SanPham, KhachHang, PhieuChi). |
| 46 | Replace Parameter with Explicit Methods | `BLL/KhachHangController.cs` : 23 | Trước đây: HienthiAutoComboBox(cmb, bool loai) – đọc lời gọi "(cmb, true)" không biết true là gì. Nay mỗi giá trị của cờ là một hàm có tên rõ nghĩa. |
| 47 | Preserve Whole Object | `Utils/ThamSo.cs` : 109 | nhận cả đối tượng CuaHang thay vì tách ra 3 tham số (thêm thông tin mới như email, mã số thuế sau này không phải đổi chữ ký hàm). |
| 48 | Replace Parameter with Method Call | `BLL/PhieuNhapController.cs` : 251 | bỏ tham số nguoiDung: hàm tự lấy người đang đăng nhập. (Trước đây form truyền Environment.UserName = tên Windows, không phải nhân viên.) |
| 49 | Introduce Parameter Object | `Entities/KhoangNgay.cs` : 9 | thay cho cặp tham số (DateTime tu, DateTime den) lặp lại ở nhiều hàm, kèm luôn luật "từ ≤ đến" và phép "den + 1 ngày" mà nơi nào cũng tự viết. |
| 50 | Remove Setting Method | `Entities/PhieuBan.cs` : 13 | bỏ setter của Id vì mã phiếu không được đổi sau khi tạo. |
|  |  | `Entities/PhieuNhap.cs` : 9 | mã phiếu chỉ gán qua hàm khởi tạo. |
| 51 | Hide Method | `BLL/XuatKhoController.cs` : 52 | chỉ PhanLoFIFO dùng nên đổi public -> private. |
| 52 | Replace Constructor with Factory Method | `Entities/MaSanPham.cs` : 18 | thay cho 4 đoạn "new MaSanPham(); sp.Id = ...; sp.SoLuong = ...; ..." chép ở XuatKhoController và MaSanPhamController. Lô không có hạn dùng nhận NgayHetHan = DateTime.MaxValue (xếp sau cùng khi xuất FIFO). |
| 53 | Replace Error Code with Exception | `BLL/Service/NghiepVuException.cs` : 7 | thay cho việc trả chuỗi "OK" / chuỗi lỗi. |
| 54 | Replace Exception with Test | `UI/frmBanLe.cs` : 638 | kiểm tra ô rỗng (DBNull) thay vì để Convert ném lỗi rồi nuốt bằng catch rỗng. |
|  |  | `UI/frmBanSi.cs` : 656 | kiểm tra ô rỗng (DBNull) thay vì để Convert ném lỗi rồi nuốt bằng catch rỗng. |
| 55 | Extract Superclass | `UI/frmDoiTacCoSo.cs` : 15 | hai form frmKhachHang / frmDaiLy dài gần 1.000 dòng mỗi form và chỉ khác nhau khoảng 36 dòng. |
| 56 | Pull Up Field | `UI/frmDoiTacCoSo.cs` : 22 | các hằng số và trường trạng thái dưới đây trước đây được khai báo y hệt nhau trong frmKhachHang và frmDaiLy. |
| 57 | Pull Up Method | `UI/frmDoiTacCoSo.cs` : 17 | toàn bộ xử lý (nạp, tìm, lọc, phân trang, thêm/sửa/xóa, LuuDuLieu...) cùng giao diện (Designer) nay nằm ở lớp cha; lớp con chỉ còn khai báo thông tin riêng. |
| 58 | Extract Subclass | `Entities/ThongTinHoaDon.cs` : 177 | điều khoản thanh toán và hạn mức nợ chỉ có ở bán sỉ, nên tách khỏi ThongTinHoaDon (bán lẻ không cần) thành lớp con. |
| 59 | Pull Up Constructor Body | `UI/frmDoiTacCoSo.cs` : 59 | phần gán 4 thông tin mô tả trước đây lặp lại trong hàm khởi tạo của frmKhachHang và frmDaiLy, nay nằm ở hàm khởi tạo lớp cha. |
| 60 | Push Down Field | `Entities/DaiLy.cs` : 11 | Các cột HAN_MUC_CONG_NO, SO_NGAY_CONG_NO, CAP_DAI_LY nằm chung trong bảng KHACH_HANG nhưng chỉ có nghĩa với đại lý, nên chỉ lớp DaiLy giữ chúng (khách lẻ không có). |
| 61 | Push Down Method | `UI/frmDoiTacCoSo.cs` : 95 | cách tạo mã "chỉ là số thứ tự" trước đây nằm ở lớp cha nhưng chỉ form Đại lý dùng (form Khách hàng ghi đè bằng "KH" + số). Đưa phần cài đặt xuống frmDaiLy; lớp cha chỉ còn khai báo hàm (lớp cha vẫn phải tạo được đối tượng cho trình thiết kế form). |
| 62 | Extract Interface | `DAL/Repositories/IDanhMucFactory.cs` : 8 | ba factory này có cùng bộ thao tác "lấy danh sách / lưu", nên phía controller chỉ cần làm việc với giao diện này. |
| 63 | Form Template Method | `UI/BieuDoCot.cs` : 12 | BieuDoBan (frmSoLuongBan) và BieuDoDoanhThu (frmDoanhThu) trước đây mỗi lớp có một OnPaint ~85 dòng gần giống nhau. Nay OnPaint (khuôn mẫu) nằm ở đây; lớp con chỉ cung cấp các bước khác nhau: gom dữ liệu, câu báo khi trống, tiêu đề. |
| 64 | Replace Inheritance with Delegation | `DAL/DataAccess/DataService.cs` : 12 | trước đây DataService kế thừa DataTable chỉ để chứa dữ liệu (nên lộ ra hàng trăm hàm của DataTable). Nay DataService GIỮ một DataTable và trả ra qua thuộc tính Bang. |
| 65 | Collapse Hierarchy | `Entities/DaiLy.cs` : 5 | lớp con KhachLe (tạo ở bước Replace Type Code with Subclasses) không thêm trường hay hành vi nào so với KhachHang, nên gộp lại vào lớp cha: khách lẻ = KhachHang. |
| 66 | Replace Delegation with Inheritance | `BLL/ChiTietPhieuBanController.cs` : 12 | lớp này trước đây giữ một ChiTietPhieuBanFactory và có 5 hàm chỉ chuyển tiếp nguyên xi (Save, SoLuongDangChon, TimDongChuaLuu, DaCoDongDaLuu, KiemTraTonKho). Vì nó dùng gần như toàn bộ giao diện của factory, cho kế thừa luôn và bỏ các hàm chuyển tiếp. |

## B. 23 mẫu thiết kế (Design Pattern – GoF)

| # | Mẫu | Loại | Vị trí (file : dòng) | Ý nghĩa / vai trò trong chương trình |
| --- | --- | --- | --- | --- |
| 1 | Factory Method | Cài đặt | `BLL/BanHang/BanHangService.cs` : 9 | Hàm LapPhieuMoi() (ở lớp cha) không tự "new" phiếu mà gọi hàm tạo TaoPhieu() do lớp con quyết định: BanLeService tạo PhieuBan, BanSiService tạo PhieuBanSi (có hạn mức nợ, điều khoản thanh toán). Form bán lẻ / bán sỉ chỉ cần giữ đúng loại service, phần còn lại dùng chung. |
|  |  | Nơi dùng | `UI/frmBanLe.cs` : 330 | Build() gọi m_KenhBan.LapPhieuMoi() để tạo đúng loại phiếu lẻ / sỉ. |
|  |  | Nơi dùng | `UI/frmBanSi.cs` : 349 | Build() gọi m_KenhBan.LapPhieuMoi() để tạo đúng loại phiếu lẻ / sỉ. |
| 2 | Abstract Factory | Cài đặt | `BLL/BanHang/KenhBanFactory.cs` : 7 | tạo trọn MỘT BỘ đối tượng đi cùng nhau của một kênh bán: dịch vụ bán hàng (giá lẻ/sỉ, loại phiếu), thông tin thêm của hóa đơn (lẻ / sỉ có hạn mức nợ) và chuỗi kiểm tra trước khi lưu (bán sỉ có thêm kiểm tra hạn mức nợ đại lý). Form Bán lẻ / Bán sỉ chỉ chọn một nhà máy (KenhBanLeFactory / KenhBanSiFactory) là có đủ bộ đối tượng khớp nhau, không thể lỡ ghép "phiếu sỉ" với "kiểm tra của bán lẻ". |
|  |  | Nơi dùng | `UI/frmBanLe.cs` : 26 | một nhà máy tạo đủ bộ đối tượng của kênh bán lẻ. |
|  |  | Nơi dùng | `UI/frmBanSi.cs` : 29 | một nhà máy tạo đủ bộ đối tượng của kênh bán sỉ. |
| 3 | Builder | Cài đặt | `BLL/BanHang/HoaDonBuilder.cs` : 12 | Hóa đơn có nhiều phần tùy chọn (dịch vụ phụ, phí vận chuyển, chiết khấu, giảm giá, VAT, người lập...). Thay vì một hàm khởi tạo dài hoặc gán từng thuộc tính rải rác trong form, form gọi lần lượt các bước (mỗi bước trả lại chính builder để gọi nối tiếp) rồi Build(). Build() kiểm tra hợp lệ, tính các khoản tiền (qua Decorator / Strategy) và trả về PhieuBan đúng loại kênh (qua Factory Method). |
|  |  | Nơi dùng | `UI/frmBanLe.cs` : 329 | lắp hóa đơn từng bước rồi Build(). |
|  |  | Nơi dùng | `UI/frmBanSi.cs` : 348 | lắp hóa đơn từng bước rồi Build(). |
| 4 | Prototype | Cài đặt | `Entities/ThongTinHoaDon.cs` : 107 | tạo bản sao đầy đủ của thông tin hóa đơn (kể cả danh sách dịch vụ phụ) mà không cần biết lớp cụ thể là ThongTinHoaDon hay ThongTinHoaDonSi: lớp con ghi đè ChepVao() để chép thêm trường riêng. Dùng để: (1) lập phiếu mới từ "mẫu mặc định" của kênh bán, (2) chụp trạng thái cho Memento, (3) soạn thay đổi trên bản nháp trước khi áp (Command). |
|  |  | Nơi dùng | `UI/frmBanLe.cs` : 28 | "mẫu mặc định" của kênh; mỗi phiếu mới là một bản sao của mẫu. |
|  |  | Nơi dùng | `UI/frmBanSi.cs` : 31 | "mẫu mặc định" của kênh; mỗi phiếu mới là một bản sao của mẫu. |
| 5 | Singleton | Cài đặt | `Utils/PhienDangNhap.cs` : 12 | Cả chương trình chỉ có MỘT phiên đăng nhập; mọi form (bán hàng ghi người lập phiếu, thống kê lọc theo nhân viên, phân quyền menu...) phải thấy cùng một người dùng. Lớp có hàm khởi tạo private, đối tượng duy nhất lấy qua PhienDangNhap.HienTai. Các thuộc tính static bên dưới (NguoiDungHienTai, BatDau...) chỉ là lối tắt tới HienTai để code cũ không phải sửa. |
|  |  | Nơi dùng | `BLL/BanHang/KiemTraHoaDon.cs` : 91 | hỏi quyền của phiên đăng nhập duy nhất. |
|  |  | Nơi dùng | `UI/frmMain.cs` : 312 | lấy quyền từ phiên đăng nhập duy nhất PhienDangNhap.HienTai. |
| 6 | Adapter | Cài đặt | `BLL/ThongKe/IKetNoiDuLieu.cs` : 15 | Lớp cũ DataService có giao diện khác hẳn: tự tạo OleDbCommand, gọi Load(), đọc kết quả ở .Bang, và lỗi không ném ra mà chỉ ghi vào LastError. Các lớp thống kê mới chỉ làm việc với IKetNoiDuLieu; DataServiceAdapter "chuyển đổi" lời gọi TruyVan(sql, thamSo) sang cách dùng của DataService và đổi LastError thành ngoại lệ. Sau này đổi sang SqlClient chỉ cần viết adapter khác, không sửa thống kê. |
|  |  | Nơi dùng | `BLL/ThongKe/ThongKeTheoKhoangNgay.cs` : 38 | mặc định đọc dữ liệu qua DataServiceAdapter (bọc DataService cũ). |
| 7 | Bridge | Cài đặt | `BLL/ThongKe/KenhXuat.cs` : 10 | Có hai chiều thay đổi độc lập: LOẠI thống kê (tồn kho, chi phí, giảm giá, theo nhân viên...) và CÁCH xuất (hiện lên lưới, ghi file CSV mở bằng Excel...). Nếu kế thừa thẳng sẽ phải có 4 x 2 = 8 lớp. Bridge tách hai chiều: lớp thống kê (ThongKeTheoKhoangNgay) giữ một tham chiếu IKenhXuat và gọi Xuat(); thêm loại thống kê hay thêm kênh xuất mới đều không phải sửa chiều còn lại. |
|  |  | Nơi dùng | `UI/frmThongKe.cs` : 142 | cùng một thống kê, đổi kênh xuất (lưới / CSV) mà không cần lớp mới. |
| 8 | Composite | Cài đặt | `Entities/DichVuPhatSinh.cs` : 7 | một dịch vụ trên hóa đơn có thể là dịch vụ lẻ (giao hàng, bốc xếp...) hoặc một GÓI gồm nhiều dịch vụ (ví dụ "Gói chăm sóc lúa" = phun thuốc hộ + tư vấn kỹ thuật), và gói lại có thể chứa gói con. Cả hai cùng giao diện IDichVu nên phần tính tiền (CongDichVu) chỉ gọi SoTien mà không cần phân biệt lẻ hay gói. |
|  |  | Cài đặt | `Entities/ThongTinHoaDon.cs` : 169 | tổng tiền dịch vụ = tiền của "gói" chứa mọi dịch vụ. |
|  |  | Nơi dùng | `BLL/BanHang/ThanhPhanTien.cs` : 132 | gom mọi dịch vụ thành một gói rồi hỏi tổng tiền của gói. |
|  |  | Nơi dùng | `UI/ucDichVuGiamGia.cs` : 64 | thêm cả một gói dịch vụ: gói tự trả ra các dịch vụ lẻ bên trong. |
| 9 | Decorator | Cài đặt | `BLL/BanHang/ThanhPhanTien.cs` : 10 | Tổng tiền = tiền hàng, rồi lần lượt "bọc" thêm: trừ chiết khấu, trừ giảm giá, cộng phí vận chuyển, cộng dịch vụ phụ, cộng VAT. Mỗi lớp bọc chỉ biết phần của mình và gọi tiếp lớp bên trong, nên có thể thêm / bớt / đổi thứ tự khoản phí mà không sửa các lớp khác. Mỗi lớp còn tự ghi một dòng diễn giải để in lên hóa đơn. |
|  |  | Nơi dùng | `UI/frmBanLe.cs` : 650 | tổng tiền tính qua chuỗi lớp bọc (chiết khấu, giảm giá, phí, dịch vụ, VAT). |
|  |  | Nơi dùng | `UI/frmBanSi.cs` : 668 | tổng tiền tính qua chuỗi lớp bọc (chiết khấu, giảm giá, phí, dịch vụ, VAT). |
| 10 | Facade | Cài đặt | `BLL/BanHang/BanHangFacade.cs` : 13 | Lưu một hóa đơn cần nhiều bước trên nhiều lớp: chạy chuỗi kiểm tra (Chain of Responsibility), chuyển trạng thái (State), ghi dòng PHIEU_BAN qua PhieuBanController, cập nhật số phiếu trong THAM_SO, lưu chi tiết qua ChiTietPhieuBanController, lưu dịch vụ phụ, rồi báo sự kiện (Observer). Trước đây frmBanLe và frmBanSi mỗi form tự làm (và chép lại) các bước này; nay chỉ gọi LuuHoaDonMoi(). |
|  |  | Nơi dùng | `UI/frmBanLe.cs` : 340 | một lời gọi thay cho chuỗi bước kiểm tra / ghi phiếu / chi tiết / dịch vụ / báo sự kiện. |
|  |  | Nơi dùng | `UI/frmBanSi.cs` : 359 | một lời gọi thay cho chuỗi bước kiểm tra / ghi phiếu / chi tiết / dịch vụ / báo sự kiện. |
| 11 | Flyweight | Cài đặt | `BLL/DonViTinhController.cs` : 39 | mỗi đơn vị tính (Chai, Gói, Kg...) chỉ có MỘT đối tượng DonViTinh dùng chung cho mọi sản phẩm / lô hàng. Trước đây mỗi lần nạp một dòng sản phẩm lại truy vấn CSDL và tạo đối tượng mới (danh sách 1.000 lô = 1.000 truy vấn + 1.000 đối tượng giống hệt nhau). |
|  |  | Nơi dùng | `BLL/MaSanPhamController.cs` : 59 | đơn vị tính lấy từ kho dùng chung, không tạo đối tượng mới mỗi dòng. |
| 12 | Proxy | Cài đặt | `BLL/ThongKe/ThongKeBaoVe.cs` : 13 | (protection proxy). Form thống kê làm việc với IThongKe nên không phân biệt được đang dùng thống kê thật hay proxy. Proxy kiểm tra quyền của người đang đăng nhập (PhienDangNhap) trước khi chuyển lời gọi cho thống kê thật: không có quyền thống kê thì chặn; nhân viên bán hàng xem thống kê có cột nhân viên (ID_NGUOI_DUNG) thì chỉ còn các dòng của chính mình (tổng cộng tính lại). Admin, Kế toán xem tất cả. |
|  |  | Nơi dùng | `UI/frmThongKe.cs` : 121 | bọc thống kê thật trong proxy kiểm tra quyền. |
| 13 | Chain of Responsibility | Cài đặt | `BLL/BanHang/KiemTraHoaDon.cs` : 12 | Các luật kiểm tra (mã phiếu trùng, vượt hạn mức nợ của đại lý, giảm giá vượt mức cho phép...) là các mắt xích nối thành chuỗi. Hóa đơn được chuyển lần lượt qua từng mắt xích; mắt xích nào thấy sai thì dừng chuỗi bằng NghiepVuException. Thêm / bớt / đổi thứ tự luật chỉ cần sửa chỗ nối chuỗi. (Kiểm tra tồn kho theo lô vẫn làm ở form, vì cần các dòng chưa lưu trên lưới.) |
|  |  | Nơi dùng | `BLL/BanHang/BanHangFacade.cs` : 46 | chuyển hóa đơn qua chuỗi mắt xích kiểm tra. |
| 14 | Command | Cài đặt | `BLL/BanHang/LenhHoaDon.cs` : 8 | mỗi thao tác sửa hóa đơn đang soạn được gói thành một đối tượng "lệnh" có ThucHien() và HoanTac(). Form không tự sửa dữ liệu mà giao lệnh cho LichSuLenh; nhờ vậy thao tác có thể hoàn tác (Ctrl+Z trên form Bán lẻ / Bán sỉ) và thêm loại lệnh mới không phải sửa form. |
|  |  | Nơi dùng | `UI/frmBanLe.cs` : 31 | lịch sử lệnh để hoàn tác bằng Ctrl+Z. |
|  |  | Nơi dùng | `UI/frmBanSi.cs` : 34 | lịch sử lệnh để hoàn tác bằng Ctrl+Z. |
| 15 | Interpreter | Cài đặt | `BLL/BanHang/DieuKienKhuyenMai.cs` : 33 | điều kiện khuyến mãi được viết thành câu ngắn, ví dụ "TIEN_HANG >= 10000000 VA LA_DAI_LY" hoặc "THANG = 11 VA TIEN_HANG >= 1000000 HOAC CAP_DAI_LY = 1" Văn phạm: BieuThuc := Va ('HOAC' Va)* ; Va := DieuKien ('VA' DieuKien)* ; DieuKien := 'KHONG' DieuKien \| Bien (('>='\|'<='\|'>'\|'<'\|'='\|'<>') So)? Mỗi quy tắc văn phạm là một lớp (BieuThucHoac, BieuThucVa, BieuThucKhong, SoSanh, BienDung) có hàm DanhGia(ngữ cảnh). Đổi chương trình khuyến mãi chỉ cần sửa câu điều kiện, không phải sửa code. |
| 16 | Iterator | Cài đặt | `Entities/KhoangNgay.cs` : 47 | duyệt lần lượt từng ngày trong khoảng mà bên ngoài không cần biết cách tính ngày kế tiếp (qua tháng, năm nhuận...). Dùng được trực tiếp trong foreach. |
|  |  | Nơi dùng | `UI/frmDoanhThu.cs` : 651 | duyệt từng ngày trong khoảng, ngày không bán vẫn có cột 0. |
| 17 | Mediator | Cài đặt | `UI/HoaDonMediator.cs` : 9 | Trước đây mỗi ô tự gắn handler và tự sửa ô khác (đơn giá sửa thành tiền, tổng tiền / đã trả / còn nợ sửa lẫn nhau – kể cả ô Còn nợ gọi lại chính handler của nó). Nay các ô chỉ báo "tôi vừa đổi" cho mediator; mediator là nơi DUY NHẤT biết ô nào phải tính lại theo ô nào. Hai form dùng chung một lớp. |
|  |  | Nơi dùng | `UI/frmBanLe.cs` : 56 | các ô số báo cho mediator thay vì tự sửa lẫn nhau. |
|  |  | Nơi dùng | `UI/frmBanSi.cs` : 61 | các ô số báo cho mediator thay vì tự sửa lẫn nhau. |
| 18 | Memento | Cài đặt | `Entities/ThongTinHoaDon.cs` : 145 | chụp lại trạng thái thông tin hóa đơn thành một đối tượng "kín" (bên ngoài không đọc/sửa được nội dung) và khôi phục lại khi cần (hoàn tác bằng Ctrl+Z). |
| 19 | Observer | Cài đặt | `BLL/BanHang/SuKienHoaDon.cs` : 19 | Sau khi lưu hóa đơn có nhiều việc phụ phải làm (ghi nhật ký, làm mới bộ đệm công nợ khách, cập nhật màn hình thống kê đang mở...). Phần lưu (BanHangFacade) chỉ "phát" sự kiện; các bên quan sát tự đăng ký / hủy đăng ký. Thêm việc mới không phải sửa phần lưu hóa đơn. |
|  |  | Nơi dùng | `BLL/BanHang/BanHangFacade.cs` : 62 | báo cho mọi bên nghe. |
|  |  | Nơi dùng | `Program.cs` : 18 | đăng ký các bên nghe sự kiện "hóa đơn đã lưu" khi khởi động. |
|  |  | Nơi dùng | `UI/frmThongKe.cs` : 18 | form là một bên nghe (IQuanSatHoaDon), tự làm mới khi có hóa đơn mới. |
| 20 | State | Cài đặt | `BLL/BanHang/TrangThaiHoaDon.cs` : 10 | Mỗi trạng thái là một lớp tự quyết định hóa đơn có được sửa / xóa / in không và chuyển sang trạng thái nào khi Lưu, Thanh toán, Hủy. Form không còn phải so sánh chuỗi "Nháp", "Đã lưu"... rải rác; chuyển trạng thái sai (ví dụ hủy hóa đơn đã thanh toán) sẽ báo lỗi nghiệp vụ. Tên trạng thái được lưu ở cột PHIEU_BAN.TRANG_THAI. |
|  |  | Nơi dùng | `BLL/BanHang/BanHangFacade.cs` : 49 | trạng thái hiện tại quyết định trạng thái mới khi Lưu. |
|  |  | Nơi dùng | `UI/frmBanLe.cs` : 428 | trạng thái hóa đơn quyết định có cho Sửa / Xóa không. |
|  |  | Nơi dùng | `UI/frmBanSi.cs` : 442 | trạng thái hóa đơn quyết định có cho Sửa / Xóa không. |
| 21 | Strategy | Cài đặt | `BLL/BanHang/ChienLuocGiamGia.cs` : 9 | Mỗi cách giảm giá là một lớp con có cùng hàm TinhTienGiam(); người bán chọn cách giảm khi lập hóa đơn, phần tính tổng tiền không cần biết đang dùng cách nào. Thêm cách giảm mới (ví dụ theo mùa vụ) chỉ cần thêm lớp con, không sửa form hay công thức tổng. Mã chiến lược được lưu vào cột PHIEU_BAN.KIEU_GIAM để thống kê. |
|  |  | Nơi dùng | `UI/frmBanLe.cs` : 60 | chiến lược xuất kho (FIFO / chỉ định) quyết định có cho chọn lô không. |
|  |  | Nơi dùng | `UI/frmBanLe.cs` : 140 | REFACTOR #31: phân lô theo chiến lược xuất kho, không còn if/else theo cấu hình. |
|  |  | Nơi dùng | `UI/frmBanSi.cs` : 68 | chiến lược xuất kho (FIFO / chỉ định) quyết định có cho chọn lô không. |
|  |  | Nơi dùng | `UI/frmBanSi.cs` : 150 | REFACTOR #31: phân lô theo chiến lược xuất kho, không còn if/else theo cấu hình. |
|  |  | Nơi dùng | `UI/ucDichVuGiamGia.cs` : 34 | người bán chọn chiến lược giảm giá; lựa chọn "Khuyến mãi theo điều kiện" |
| 22 | Template Method | Cài đặt | `BLL/ThongKe/ThongKeTheoKhoangNgay.cs` : 24 | ThucHien() cố định trình tự các bước: kiểm tra khoảng ngày -> truy vấn -> đổi tên cột hiển thị -> thêm dòng tổng cộng -> ghi chú khoảng ngày. Lớp con chỉ viết bước khác nhau là TruyVan() (và có thể đổi TenCot / CoDongTong). Bốn thống kê của yêu cầu 4 vì thế có cùng cách chạy và cùng hình thức. |
|  |  | Nơi dùng | `UI/frmThongKe.cs` : 120 | mỗi lớp thống kê chỉ viết bước TruyVan, khung ThucHien dùng chung. |
| 23 | Visitor | Cài đặt | `BLL/BanHang/ThanhPhanTien.cs` : 164 | "người thăm" đi qua từng lớp của bảng tính tiền (Decorator) và làm một việc riêng cho từng loại khoản (tiền hàng, chiết khấu, giảm giá, phí vận chuyển, dịch vụ, VAT). Thêm một cách xử lý mới (in hóa đơn, tổng hợp khoản giảm, xuất file...) chỉ cần viết một lớp người thăm mới, không phải sửa các lớp ThanhPhanTien. |
|  |  | Nơi dùng | `BLL/BanHang/HoaDonBuilder.cs` : 95 | cho người thăm đi qua bảng tính tiền để lấy các dòng diễn giải. |

Kiểm tra: 66/66 kỹ thuật refactoring và 23/23 mẫu thiết kế có nhãn trong mã nguồn.
