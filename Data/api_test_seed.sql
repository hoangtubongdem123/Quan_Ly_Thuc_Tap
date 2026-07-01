USE quanly_thuctap;

INSERT INTO khoa
(
    id_khoa,
    ten_khoa,
    gmail_khoa,
    password_khoa
)
VALUES
(1, 'Khoa Cong nghe thong tin', 'cntt@huce.edu.vn', '123456')
ON DUPLICATE KEY UPDATE
    ten_khoa = VALUES(ten_khoa),
    gmail_khoa = VALUES(gmail_khoa),
    password_khoa = VALUES(password_khoa);

INSERT INTO sinh_vien
(
    mssv,
    id_khoa,
    ten_sinh_vien,
    gmail_sinh_vien,
    password_sinh_vien
)
VALUES
(22010001, 1, 'Nguyen Van An', 'an.22010001@huce.edu.vn', '123456'),
(22010002, 1, 'Tran Thi Binh', 'binh.22010002@huce.edu.vn', '123456'),
(22010003, 1, 'Le Minh Cuong', 'cuong.22010003@huce.edu.vn', '123456')
ON DUPLICATE KEY UPDATE
    id_khoa = VALUES(id_khoa),
    ten_sinh_vien = VALUES(ten_sinh_vien),
    gmail_sinh_vien = VALUES(gmail_sinh_vien),
    password_sinh_vien = VALUES(password_sinh_vien);

INSERT INTO giang_vien
(
    ma_so_giang_vien,
    id_khoa,
    ten_giang_vien,
    gmail_giang_vien,
    password_giang_vien
)
VALUES
('GV001', 1, 'ThS Nguyen Huong Giang', 'giang.gv001@huce.edu.vn', '123456'),
('GV002', 1, 'TS Pham Quang Huy', 'huy.gv002@huce.edu.vn', '123456')
ON DUPLICATE KEY UPDATE
    id_khoa = VALUES(id_khoa),
    ten_giang_vien = VALUES(ten_giang_vien),
    gmail_giang_vien = VALUES(gmail_giang_vien),
    password_giang_vien = VALUES(password_giang_vien);

INSERT INTO ki_thuc_tap
(
    id_ki_thuc_tap,
    ten_ki_thuc_tap,
    time_batdau,
    time_ketthuc,
    trang_thai,
    id_khoa
)
VALUES
(1001, 'Ky thuc tap CNTT 2026', '2026-06-01', '2026-08-01', 'Dang Mo', 1)
ON DUPLICATE KEY UPDATE
    ten_ki_thuc_tap = VALUES(ten_ki_thuc_tap),
    time_batdau = VALUES(time_batdau),
    time_ketthuc = VALUES(time_ketthuc),
    trang_thai = VALUES(trang_thai),
    id_khoa = VALUES(id_khoa);

INSERT INTO don_vi_hd
(
    id_don_vi_hd,
    id_ki_thuc_tap,
    ten_don_vi_hd,
    gmail_don_vi_hd,
    password_don_vi_hd
)
VALUES
(501, 1001, 'Cong ty ABC Tech', 'hr@abctech.vn', '123456'),
(502, 1001, 'Cong ty Sao Viet', 'hr@saoviet.vn', '123456')
ON DUPLICATE KEY UPDATE
    id_ki_thuc_tap = VALUES(id_ki_thuc_tap),
    ten_don_vi_hd = VALUES(ten_don_vi_hd),
    gmail_don_vi_hd = VALUES(gmail_don_vi_hd),
    password_don_vi_hd = VALUES(password_don_vi_hd);
