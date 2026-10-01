# Đồ án Bảo trì Phần mềm - Cửa hàng Nông dược
**Nhóm 02**  
**Môn:** Thiết kế & Phát triển & Bảo trì phần mềm  
Hệ thống quản lý **Cửa hàng Nông dược** được xây dựng bằng công nghệ:
- Ngôn ngữ: **C#**
- Giao diện: **Windows Forms**
- Cơ sở dữ liệu: **SQL Server**

Mục tiêu của đồ án là thực hiện các hoạt động **bảo trì phần mềm** trên hệ thống có sẵn, bao gồm:
- Tham khảo trên web: https://refactoring.guru/
##  Cấu trúc các nhánh (Branches)
| Nhánh              | Mục đích                                      | Trạng thái      |
|--------------------|-----------------------------------------------|-----------------|
| `main`             | KHÔNG ĐỤNG VÀO -> chỉ chứa dự án góc and dự án hoàn chỉnh              |     |
| `Develop`          | Thực hiện tái cấu trúc mã nguồn (down về trước khi làm Và Pull request từ nhánh cá nhân sau khi làm xong| |
| `[Tên]->'lưu ý branches cá nhân phải link Soure về Develop`  | Up dự án đã làm vào nhánh cá nhân và Pull request vào Develop    |
## Hướng dẫn cài đặt Database

1. Mở SQL Server Management Studio
2. Chạy file `Database/CuahangNongDuoc.sql` để tạo database và các bảng
3. Sửa lại chuỗi kết nối trong `App.config` cho đúng với máy bạn (nếu cần)
##  Hướng dẫn làm việc với Git
```bash
# 1. Tải dự án về máy (chỉ làm 1 lần)
git clone https://github.com/DoAn-BaoTri-NongDuoc-Nhom02/DoAn-BaoTri-NongDuoc-Nhom02.git
cd DoAn-BaoTri-NongDuoc-Nhom02

# 2. Chuyển sang nhánh cần làm việc
git checkout Refactoring
# hoặc: git checkout Design-Patterns

# 3. Lấy code mới nhất trước khi bắt đầu sửa
git pull

# 4. Sau khi sửa xong, đẩy code lên GitHub
git add .
git commit -m "Mô tả những gì đã làm"
git push
