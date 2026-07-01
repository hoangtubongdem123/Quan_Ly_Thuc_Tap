using MySql.Data.MySqlClient;

public class SinhVienRepository : ISinhVienRepository 
{
    private readonly string _connectionString;
    

    public SinhVienRepository(IConfiguration config)
    {
        _connectionString = config.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Missing DefaultConnection connection string.");
    }

    public async Task<List<Sinh_Vien_Model>> GetAll()
    {
        List<Sinh_Vien_Model> students = new();

        using MySqlConnection conn =
            new MySqlConnection(_connectionString);

        await conn.OpenAsync();

        string sql = "SELECT * FROM sinh_vien";

        using MySqlCommand cmd =
            new MySqlCommand(sql, conn);

        using var reader =
            await cmd.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            students.Add(new Sinh_Vien_Model
            {
                Mssv = reader["mssv"].ToString() ?? "",

                IdKhoa = Convert.ToInt32(reader["id_khoa"]),

                TenSinhVien = reader["ten_sinh_vien"].ToString()??"",

                GmailSinhVien = reader["gmail_sinh_vien"].ToString()??"",

                PasswordSinhVien = reader["password_sinh_vien"]
                    .ToString()??""
            });
        }

        return students;
    }


    public async Task<Sinh_Vien_Model?> GetSinhVien(string mssv)
    {
        using MySqlConnection conn =
            new MySqlConnection(_connectionString);

        await conn.OpenAsync();

        string sql = "SELECT * FROM sinh_vien WHERE mssv = @mssv ";

        using MySqlCommand cmd =
            new MySqlCommand(sql, conn);


        cmd.Parameters.AddWithValue("@mssv", mssv);
        using var reader =
            await cmd.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return new Sinh_Vien_Model
            {
                Mssv = reader["mssv"].ToString() ?? "",
                IdKhoa = Convert.ToInt32(reader["id_khoa"]),
                TenSinhVien =
                    reader["ten_sinh_vien"].ToString() ?? "",
                GmailSinhVien =
                    reader["gmail_sinh_vien"].ToString() ?? "",
                PasswordSinhVien =
                    reader["password_sinh_vien"].ToString() ?? ""

            };
            
        }

        return null;
    }

    public async Task<Sinh_Vien_Model?> GetSinhVienByGmail(string gmail)
    {
        using MySqlConnection conn =
            new MySqlConnection(_connectionString);

        await conn.OpenAsync();

        string sql = "SELECT * FROM sinh_vien WHERE gmail_sinh_vien = @gmail ";

        using MySqlCommand cmd =
            new MySqlCommand(sql, conn);


        cmd.Parameters.AddWithValue("@gmail", gmail);
        using var reader =
            await cmd.ExecuteReaderAsync();

        if(await reader.ReadAsync())
        {
            return new Sinh_Vien_Model
            {
                Mssv = reader["mssv"].ToString() ?? "",
                IdKhoa = Convert.ToInt32(reader["id_khoa"]),
                TenSinhVien = reader["ten_sinh_vien"].ToString() ?? "",
                GmailSinhVien = reader["gmail_sinh_vien"].ToString() ?? "",
                PasswordSinhVien = reader["password_sinh_vien"].ToString() ?? ""
            };
        }

        return null; 
    }

    public async Task<int> AddSinhVien(Sinh_Vien_Model request)
    {
        using MySqlConnection conn =
            new MySqlConnection(_connectionString);

        await conn.OpenAsync();

        string sql = @"
            INSERT INTO sinh_vien
            (
                mssv,
                id_khoa,
                ten_sinh_vien,
                gmail_sinh_vien,
                password_sinh_vien
            )
            VALUES
            (
                @mssv,
                @id_khoa,
                @ten_sinh_vien,
                @gmail_sinh_vien,
                @password_sinh_vien
            )
        ";

        using MySqlCommand cmd =
            new MySqlCommand(sql, conn);

        AddSinhVienParameters(cmd, request);

        return await cmd.ExecuteNonQueryAsync();
    }

    public async Task<int> UpdateSinhVien(string mssv, Sinh_Vien_Model request)
    {
        using MySqlConnection conn =
            new MySqlConnection(_connectionString);

        await conn.OpenAsync();

        string sql = @"
            UPDATE sinh_vien
            SET
                id_khoa = @id_khoa,
                ten_sinh_vien = @ten_sinh_vien,
                gmail_sinh_vien = @gmail_sinh_vien,
                password_sinh_vien = @password_sinh_vien
            WHERE mssv = @mssv
        ";

        using MySqlCommand cmd =
            new MySqlCommand(sql, conn);

        AddSinhVienParameters(cmd, request);
        cmd.Parameters["@mssv"].Value = mssv;

        return await cmd.ExecuteNonQueryAsync();
    }

    public async Task<int> DeleteSinhVien(string mssv)
    {
        using MySqlConnection conn =
            new MySqlConnection(_connectionString);

        await conn.OpenAsync();

        string sql = @"
            DELETE FROM sinh_vien
            WHERE mssv = @mssv
        ";

        using MySqlCommand cmd =
            new MySqlCommand(sql, conn);

        cmd.Parameters.AddWithValue("@mssv", mssv);

        return await cmd.ExecuteNonQueryAsync();
    }

    private static void AddSinhVienParameters(
        MySqlCommand cmd,
        Sinh_Vien_Model request)
    {
        cmd.Parameters.AddWithValue("@mssv", request.Mssv);
        cmd.Parameters.AddWithValue(
            "@id_khoa",
            request.IdKhoa > 0 ? request.IdKhoa : DBNull.Value);
        cmd.Parameters.AddWithValue("@ten_sinh_vien", request.TenSinhVien);
        cmd.Parameters.AddWithValue("@gmail_sinh_vien", request.GmailSinhVien);
        cmd.Parameters.AddWithValue("@password_sinh_vien", request.PasswordSinhVien);
    }

    public async Task<List<SinhVienKiThucTapDTO>> GetKiThucTapByMssv(string mssv)
    {
        List<SinhVienKiThucTapDTO> periods = new();

        using MySqlConnection conn =
            new MySqlConnection(_connectionString);

        await conn.OpenAsync();

        string sql = @"
            SELECT
                svtt.mssv,
                sv.ten_sinh_vien,
                svtt.id_ki_thuc_tap,
                ktt.ten_ki_thuc_tap,
                ktt.time_batdau,
                ktt.time_ketthuc,
                ktt.trang_thai AS trang_thai_ki,
                svtt.id_don_vi_hd,
                COALESCE(dvhd.ten_don_vi_hd, '') AS ten_don_vi_hd,
                COALESCE(svtt.ma_so_giang_vien, '') AS ma_so_giang_vien,
                COALESCE(gv.ten_giang_vien, '') AS ten_giang_vien,
                COALESCE(svtt.trang_thai, '') AS trang_thai_phan_cong
            FROM sinh_vien_thuc_tap svtt
            INNER JOIN sinh_vien sv
                ON sv.mssv = svtt.mssv
            INNER JOIN ki_thuc_tap ktt
                ON ktt.id_ki_thuc_tap = svtt.id_ki_thuc_tap
            LEFT JOIN don_vi_hd dvhd
                ON dvhd.id_don_vi_hd = svtt.id_don_vi_hd
            LEFT JOIN giang_vien gv
                ON gv.ma_so_giang_vien = svtt.ma_so_giang_vien
            WHERE svtt.mssv = @mssv
            ORDER BY ktt.time_batdau DESC, svtt.id_ki_thuc_tap DESC
        ";

        using MySqlCommand cmd =
            new MySqlCommand(sql, conn);

        cmd.Parameters.AddWithValue("@mssv", mssv);

        using var reader =
            await cmd.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            periods.Add(new SinhVienKiThucTapDTO
            {
                MSSV = reader["mssv"].ToString() ?? "",
                TenSinhVien = reader["ten_sinh_vien"].ToString() ?? "",
                IdKiThucTap = Convert.ToInt32(reader["id_ki_thuc_tap"]),
                TenKiThucTap = reader["ten_ki_thuc_tap"].ToString() ?? "",
                TimeBatDau = reader["time_batdau"] == DBNull.Value
                    ? null
                    : Convert.ToDateTime(reader["time_batdau"]),
                TimeKetThuc = reader["time_ketthuc"] == DBNull.Value
                    ? null
                    : Convert.ToDateTime(reader["time_ketthuc"]),
                TrangThaiKiThucTap = reader["trang_thai_ki"].ToString() ?? "",
                IdDonViHD = reader["id_don_vi_hd"] == DBNull.Value
                    ? null
                    : Convert.ToInt32(reader["id_don_vi_hd"]),
                TenDonViHD = reader["ten_don_vi_hd"].ToString() ?? "",
                MaSoGiangVien = reader["ma_so_giang_vien"].ToString() ?? "",
                TenGiangVien = reader["ten_giang_vien"].ToString() ?? "",
                TrangThaiPhanCong = reader["trang_thai_phan_cong"].ToString() ?? ""
            });
        }

        return periods;
    }

    public async Task<List<Thong_Bao_Model>> GetThongBaoByMssv(string mssv)
    {
        List<Thong_Bao_Model> notifications = new();

        using MySqlConnection conn =
            new MySqlConnection(_connectionString);

        await conn.OpenAsync();

        string sql = @"
            SELECT
                id_thong_bao,
                loai_nguoi_nhan,
                ma_nguoi_nhan,
                tieu_de,
                noi_dung,
                ngay_tao
            FROM thong_bao
            WHERE LOWER(loai_nguoi_nhan) = 'sinhvien'
            AND ma_nguoi_nhan = @mssv
            ORDER BY ngay_tao DESC, id_thong_bao DESC
        ";

        using MySqlCommand cmd =
            new MySqlCommand(sql, conn);

        cmd.Parameters.AddWithValue("@mssv", mssv);

        using var reader =
            await cmd.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            notifications.Add(new Thong_Bao_Model
            {
                IdThongBao = Convert.ToInt32(reader["id_thong_bao"]),
                LoaiNguoiNhan = reader["loai_nguoi_nhan"].ToString() ?? "",
                MaNguoiNhan = reader["ma_nguoi_nhan"].ToString() ?? "",
                TieuDe = reader["tieu_de"].ToString() ?? "",
                NoiDung = reader["noi_dung"].ToString() ?? "",
                NgayTao = reader["ngay_tao"] == DBNull.Value
                    ? DateTime.MinValue
                    : Convert.ToDateTime(reader["ngay_tao"])
            });
        }

        return notifications;
    }



    public async Task<int> GetIdKhoaByMSSV(
        string mssv)
    {
        using MySqlConnection conn =
            new MySqlConnection(_connectionString);

        await conn.OpenAsync();

        string sql = @"
            SELECT id_khoa
            FROM sinh_vien
            WHERE mssv = @mssv
        ";

        using MySqlCommand cmd =
            new MySqlCommand(sql, conn);

        cmd.Parameters.AddWithValue(
            "@mssv",
            mssv);

        var result =
            await cmd.ExecuteScalarAsync();

        if (result == null)
        {
            return 0;
        }

        return Convert.ToInt32(result);
    }





    public async Task<int> AddSinhVienThucTap(List<AddSinhVienThucTapDTO> listSV)
    {
        using MySqlConnection conn =
            new MySqlConnection(_connectionString);

        await conn.OpenAsync();

        int totalRows = 0;
  

        foreach (var sv in listSV)
        {   

            
            string sql = @"
                INSERT INTO sinh_vien_thuc_tap
                (
                    mssv,
                    id_ki_thuc_tap,
                    id_don_vi_hd,
                    ma_so_giang_vien,
                    trang_thai
                )
                VALUES
                (
                    @mssv,
                    @id_ki_thuc_tap,
                    @id_don_vi_hd,
                    @ma_so_giang_vien,
                    @trang_thai
                );
            ";

            using MySqlCommand cmd =
                new MySqlCommand(sql, conn);

            cmd.Parameters.AddWithValue(
                "@mssv",
                sv.MSSV);

            cmd.Parameters.AddWithValue(
                "@id_ki_thuc_tap",
                sv.IdKiThucTap);

            cmd.Parameters.AddWithValue(
                "@id_don_vi_hd",
                sv.IdDonViHD);

            cmd.Parameters.AddWithValue(
                "@ma_so_giang_vien",
                DBNull.Value);

            cmd.Parameters.AddWithValue(
                "@trang_thai",
                "Chờ Phân Công GVHD");

            totalRows +=
                await cmd.ExecuteNonQueryAsync();
        }

        return totalRows;
    }

    public async Task<bool> SinhVienThucTapExists(
        string mssv,
        int idKiThucTap)
    {
        using MySqlConnection conn =
            new MySqlConnection(_connectionString);

        await conn.OpenAsync();

        string sql = @"
            SELECT COUNT(*)
            FROM sinh_vien_thuc_tap
            WHERE mssv = @mssv
            AND id_ki_thuc_tap = @id_ki_thuc_tap
        ";

        using MySqlCommand cmd =
            new MySqlCommand(sql, conn);

        cmd.Parameters.AddWithValue("@mssv", mssv);
        cmd.Parameters.AddWithValue("@id_ki_thuc_tap", idKiThucTap);

        var result =
            await cmd.ExecuteScalarAsync();

        return Convert.ToInt32(result) > 0;
    }

    public async Task<int> PhanCongGiangVien(
        string mssv,
        int idKiThucTap,
        string maSoGiangVien)
    {
        using MySqlConnection conn =
            new MySqlConnection(_connectionString);

        await conn.OpenAsync();

        string sql = @"
            UPDATE sinh_vien_thuc_tap
            SET ma_so_giang_vien = @ma_so_giang_vien,
                trang_thai = @trang_thai
            WHERE mssv = @mssv
            AND id_ki_thuc_tap = @id_ki_thuc_tap
        ";

        using MySqlCommand cmd =
            new MySqlCommand(sql, conn);

        cmd.Parameters.AddWithValue("@ma_so_giang_vien", maSoGiangVien);
        cmd.Parameters.AddWithValue("@trang_thai", "Đã Phân Công GVHD");
        cmd.Parameters.AddWithValue("@mssv", mssv);
        cmd.Parameters.AddWithValue("@id_ki_thuc_tap", idKiThucTap);

        return await cmd.ExecuteNonQueryAsync();
    }

    public async Task<int> DeleteSinhVienThucTap(
        string mssv,
        int idKiThucTap)
    {
        using MySqlConnection conn =
            new MySqlConnection(_connectionString);

        await conn.OpenAsync();

        string sql = @"
            DELETE FROM sinh_vien_thuc_tap
            WHERE mssv = @mssv
            AND id_ki_thuc_tap = @id_ki_thuc_tap
        ";

        using MySqlCommand cmd =
            new MySqlCommand(sql, conn);

        cmd.Parameters.AddWithValue("@mssv", mssv);
        cmd.Parameters.AddWithValue("@id_ki_thuc_tap", idKiThucTap);

        return await cmd.ExecuteNonQueryAsync();
    }

    public async Task<int> GetIdKiThucTapByMSSV(
        string mssv)
    {
        using MySqlConnection conn =
            new MySqlConnection(_connectionString);

        await conn.OpenAsync();

        string sql = @"
            SELECT id_ki_thuc_tap
            FROM sinh_vien_thuc_tap
            WHERE mssv = @mssv
            LIMIT 1
        ";

        using MySqlCommand cmd =
            new MySqlCommand(sql, conn);

        cmd.Parameters.AddWithValue(
            "@mssv",
            mssv);

        var result =
            await cmd.ExecuteScalarAsync();

        if (result == null)
        {
            return 0;
        }

        return Convert.ToInt32(result);
    }





}
