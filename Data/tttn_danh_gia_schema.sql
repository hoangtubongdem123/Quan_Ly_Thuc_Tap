CREATE TABLE IF NOT EXISTS danh_gia_thuc_tap (
    id_danh_gia INT AUTO_INCREMENT PRIMARY KEY,
    mssv VARCHAR(20) NOT NULL,

    FOREIGN KEY (mssv)
        REFERENCES sinh_vien_thuc_tap(mssv)
        ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS diem_clo (
    id_diem_clo INT AUTO_INCREMENT PRIMARY KEY,
    id_danh_gia INT NOT NULL,
    nguoi_cham_loai VARCHAR(20) NOT NULL,
    nguoi_cham_id VARCHAR(50) NOT NULL,
    ma_clo VARCHAR(10) NOT NULL,
    diem_chang_1 DECIMAL(4, 1) NULL,
    diem_chang_2 DECIMAL(4, 1) NULL,

    FOREIGN KEY (id_danh_gia)
        REFERENCES danh_gia_thuc_tap(id_danh_gia)
        ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS minh_chung (
    id_minh_chung INT AUTO_INCREMENT PRIMARY KEY,
    ten_minh_chung VARCHAR(255) NOT NULL,
    mssv VARCHAR(20) NOT NULL,
    path VARCHAR(500) NOT NULL,

    FOREIGN KEY (mssv)
        REFERENCES sinh_vien(mssv)
        ON DELETE CASCADE
);
