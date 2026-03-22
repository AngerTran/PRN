# Danh sách tài khoản seed — CapstoneReviewTool

> Nguồn: `Data/DbSeeder.cs` — mật khẩu mặc định seed: `**Password123!**`  
> Đăng nhập bằng **email** trên form Login.

---

## Hội đồng / Committee (4 tài khoản)


| Email                                                   | Mật khẩu     | Họ tên                   | Mã GV  | Role Identity |
| ------------------------------------------------------- | ------------ | ------------------------ | ------ | ------------- |
| [committee@fpt.edu.vn](mailto:committee@fpt.edu.vn)     | Password123! | Hội đồng — Trần Minh Đức | COMM01 | Committee     |
| [committee02@fpt.edu.vn](mailto:committee02@fpt.edu.vn) | Password123! | TS. Lê Hoàng Nam         | COMM02 | Committee     |
| [committee03@fpt.edu.vn](mailto:committee03@fpt.edu.vn) | Password123! | ThS. Phạm Thu Hà         | COMM03 | Committee     |
| [committee04@fpt.edu.vn](mailto:committee04@fpt.edu.vn) | Password123! | GV. Đỗ Quang Vinh        | COMM04 | Committee     |


---

## Sinh viên (20 tài khoản)


| STT | Email                                               | Mật khẩu     | Mã SV   | Họ tên       |
| --- | --------------------------------------------------- | ------------ | ------- | ------------ |
| 1   | [student01@fpt.edu.vn](mailto:student01@fpt.edu.vn) | Password123! | SE25001 | Sinh viên 01 |
| 2   | [student02@fpt.edu.vn](mailto:student02@fpt.edu.vn) | Password123! | SE25002 | Sinh viên 02 |
| 3   | [student03@fpt.edu.vn](mailto:student03@fpt.edu.vn) | Password123! | SE25003 | Sinh viên 03 |
| 4   | [student04@fpt.edu.vn](mailto:student04@fpt.edu.vn) | Password123! | SE25004 | Sinh viên 04 |
| 5   | [student05@fpt.edu.vn](mailto:student05@fpt.edu.vn) | Password123! | SE25005 | Sinh viên 05 |
| 6   | [student06@fpt.edu.vn](mailto:student06@fpt.edu.vn) | Password123! | SE25006 | Sinh viên 06 |
| 7   | [student07@fpt.edu.vn](mailto:student07@fpt.edu.vn) | Password123! | SE25007 | Sinh viên 07 |
| 8   | [student08@fpt.edu.vn](mailto:student08@fpt.edu.vn) | Password123! | SE25008 | Sinh viên 08 |
| 9   | [student09@fpt.edu.vn](mailto:student09@fpt.edu.vn) | Password123! | SE25009 | Sinh viên 09 |
| 10  | [student10@fpt.edu.vn](mailto:student10@fpt.edu.vn) | Password123! | SE25010 | Sinh viên 10 |
| 11  | [student11@fpt.edu.vn](mailto:student11@fpt.edu.vn) | Password123! | SE25011 | Sinh viên 11 |
| 12  | [student12@fpt.edu.vn](mailto:student12@fpt.edu.vn) | Password123! | SE25012 | Sinh viên 12 |
| 13  | [student13@fpt.edu.vn](mailto:student13@fpt.edu.vn) | Password123! | SE25013 | Sinh viên 13 |
| 14  | [student14@fpt.edu.vn](mailto:student14@fpt.edu.vn) | Password123! | SE25014 | Sinh viên 14 |
| 15  | [student15@fpt.edu.vn](mailto:student15@fpt.edu.vn) | Password123! | SE25015 | Sinh viên 15 |
| 16  | [student16@fpt.edu.vn](mailto:student16@fpt.edu.vn) | Password123! | SE25016 | Sinh viên 16 |
| 17  | [student17@fpt.edu.vn](mailto:student17@fpt.edu.vn) | Password123! | SE25017 | Sinh viên 17 |
| 18  | [student18@fpt.edu.vn](mailto:student18@fpt.edu.vn) | Password123! | SE25018 | Sinh viên 18 |
| 19  | [student19@fpt.edu.vn](mailto:student19@fpt.edu.vn) | Password123! | SE25019 | Sinh viên 19 |
| 20  | [student20@fpt.edu.vn](mailto:student20@fpt.edu.vn) | Password123! | SE25020 | Sinh viên 20 |


---

## Tài khoản Demo (không tạo sẵn bởi DbSeeder)

Được tạo khi gọi `Account/DemoLogin?role=...` (lần đầu), mật khẩu: `**Demo123!`**


| Email (theo role)                                           | Mật khẩu | Ghi chú         |
| ----------------------------------------------------------- | -------- | --------------- |
| [demo.student@fpt.edu.vn](mailto:demo.student@fpt.edu.vn)   | Demo123! | role `student`  |
| [demo.lecturer@fpt.edu.vn](mailto:demo.lecturer@fpt.edu.vn) | Demo123! | role `lecturer` |
| [demo.admin@fpt.edu.vn](mailto:demo.admin@fpt.edu.vn)       | Demo123! | role `admin`    |


---

## Lưu ý

- User cũ từ seed trước (ví dụ `student@fpt.edu.vn`) có thể vẫn còn trong DB; không nằm trong bảng trên nếu bạn đã đổi seeder.
- **Không** commit file này lên kho công khai nếu dùng mật khẩu thật; đây chỉ phục vụ môi trường dev/demo.

