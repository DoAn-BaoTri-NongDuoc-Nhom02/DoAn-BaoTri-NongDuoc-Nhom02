DoAn-BaoTri-NongDuoc-Nhom02
DoAn-BaoTri-NongDuoc-Nhom02
Repository navigation
Code
Issues
Pull requests
Actions
Projects
Wiki
Security and quality
Insights
Settings
DoAn-BaoTri-NongDuoc-Nhom02
/
README.md
in
main

Edit

Preview
Indent mode

Spaces
Indent size

2
Line wrap mode

Soft wrap
Editing README.md file contents
  4
  5
  6
  7
  8
  9
 10
 11
 12
 13
 14
 15
 16
 17
 18
 19
 20
 21
 22
 23
 24
 25
 26
 27
 28
 29
 30
 31
 32
 33
 34
 35
 36
 37
 38
 39
 40
 41
 42
 43
 44
 45
 46
 47
 48
 49
 50
 51
 52
 53
 54
 55
 56
 57
 58
 59
 60
 61
 62
 63
 64
 65
 66
 67
# Đồ án Bảo trì Phần mềm - Cửa hàng Nông dược
Mục tiêu của đồ án là thực hiện các hoạt động **bảo trì phần mềm** trên hệ thống có sẵn, bao gồm:
- Tham khảo trên web: https://refactoring.guru/
##  Cấu trúc các nhánh (Branches)
| Nhánh              | Mục đích                                      | Trạng thái      |
|--------------------|-----------------------------------------------|-----------------|
| `main`             | KHÔNG ĐỤNG VÀO -> chỉ chứa dự án góc and dự án hoàn chỉnh              |     |
| `Develop`          | Thực hiện tái cấu trúc mã nguồn (down về trước khi làm Và Pull request từ nhánh cá nhân sau khi làm xong| |
| `[Tên]->'lưu ý branches cá nhân phải link Soure về Develop`  | Up dự án đã làm vào nhánh cá nhân và Pull request vào Develop    |
## Hướng dẫn cài đặt Database
from nằm trong UI còn code csdl nằm foder Database
1. Mở SQL Server Management Studio
2. Chạy file `Database/CuahangNongDuoc.sql` để tạo database và các bảng
3. Sửa lại chuỗi kết nối trong `App.config` cho đúng với máy bạn (nếu cần)
##  Hướng dẫn làm việc với Git
```bash
### 1. Tải dự án về máy lần đầu (chỉ lấy nhánh Develop)

Mở **PowerShell** hoặc **Git Bash**, chạy lệnh:

```powershell
git clone -b Develop --single-branch https://github.com/DoAn-BaoTri-NongDuoc-Nhom02/DoAn-BaoTri-NongDuoc-Nhom02.git
```

Sau khi xong, dự án sẽ nằm ở:

```
C:\...\...\DoAn-BaoTri-NongDuoc-Nhom02
```

Vào thư mục dự án:

```powershell
cd DoAn-BaoTri-NongDuoc-Nhom02
```

---

### 2. Cập nhật code mới nhất từ nhánh Develop (khi đã có sẵn trên máy)

```powershell
# 1. Vào thư mục dự án
cd C:\Users\asus\DoAn-BaoTri-NongDuoc-Nhom02

# 2. Chuyển sang nhánh Develop
git checkout Develop

# 3. Lấy code mới nhất từ GitHub
git pull origin Develop
```

### 4. Sau khi sửa xong, đẩy code lên GitHub
1. Làm việc dưới máy (Local)
```bash
git init                           # Khởi tạo Git cho dự án (chỉ làm lần đầu)
git checkout -b hoang-minh         # Tạo và chuyển sang nhánh cá nhân mới
git add .                          # Thêm tất cả file thay đổi vào hàng chờ
git commit -m "Nội dung thay đổi"  # Lưu lại phiên bản code hiện tại

2. Đồng bộ và đẩy lên GitHub (Remote)
bash
git remote add origin <URL-GitHub>  # Liên kết với kho GitHub (chỉ làm lần đầu)
git pull origin hoang-minh --rebase # Kéo code mới từ GitHub về để tránh lỗi lệch commit
git push origin hoang-minh          # Đẩy code từ máy lên nhánh cá nhân trên GitHub```

Use Control + Shift + m to toggle the tab key moving focus. Alternatively, use esc then tab to move to the next interactive element on the page.
Không có tệp nào được chọn
Attach files by dragging & dropping, selecting or pasting them.
