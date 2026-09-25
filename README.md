# 🌾 Đồ án Bảo trì Phần mềm - Cửa hàng Nông dược

**Nhóm 02**  
**Môn:** Thiết kế & Phát triển & Bảo trì phần mềm  

---

## 📌 Giới thiệu dự án

Hệ thống quản lý **Cửa hàng Nông dược** được xây dựng bằng công nghệ:
- Ngôn ngữ: **C#**
- Giao diện: **Windows Forms**
- Cơ sở dữ liệu: **SQL Server**

Mục tiêu của đồ án là thực hiện các hoạt động **bảo trì phần mềm** trên hệ thống có sẵn, bao gồm:
- Tái cấu trúc mã nguồn (Refactoring)
- Áp dụng các mẫu thiết kế (Design Patterns)
- Sửa lỗi và cải tiến chức năng
- Tham khảo trên web: https://refactoring.guru/
---

## 🌿 Cấu trúc các nhánh (Branches)

| Nhánh              | Mục đích                                      | Trạng thái      |
|--------------------|-----------------------------------------------|-----------------|
| `main`             | Phiên bản gốc ban đầu của dự án               |     |
| `Refactoring`      | Thực hiện tái cấu trúc mã nguồn               |[Refactoring] |
| `Design-Patterns`  | Áp dụng các Design Patterns vào hệ thống      | [Design Patterns] |

---

### 1️⃣ Nhánh `main`
> Phiên bản gốc và phiên bản đã chỉnh sửa 
## 🚀 Hướng dẫn làm việc với Git

## 🚀 Hướng dẫn làm việc với Git

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
