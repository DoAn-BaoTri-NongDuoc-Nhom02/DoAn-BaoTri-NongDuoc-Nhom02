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
| `main`             | Phiên bản gốc ban đầu của dự án               | ✅ Ổn định      |
| `Refactoring`      | Thực hiện tái cấu trúc mã nguồn               |[Refactoring] |
| `Design-Patterns`  | Áp dụng các Design Patterns vào hệ thống      | [Design Patterns] |

---

### 1️⃣ Nhánh `main`
> Phiên bản gốc của phần mềm Cửa hàng Nông dược

- Đây là mã nguồn ban đầu chưa chỉnh sửa
- Dùng làm cơ sở để so sánh với các phiên bản bảo trì
- Không được commit trực tiếp các thay đổi lớn vào nhánh này

**Cách tải dự án từ main về máy :**
```bash
git clone https://github.com/DoAn-BaoTri-NongDuoc-Nhom02/DoAn-BaoTri-NongDuoc-Nhom02.git
