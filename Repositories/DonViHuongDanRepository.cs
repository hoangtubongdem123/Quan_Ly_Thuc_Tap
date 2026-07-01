using MySql.Data.MySqlClient;

public class DonViHuongDanRepository :IDonViHuongDanRepository
{
    private readonly string _connectionString;

    public DonViHuongDanRepository(IConfiguration config)
    {
        _connectionString = config.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Missing DefaultConnection connection string.");
    }

    public async Task<int> AddListDonViHuongDan(List<AddDonViHuongDanDTO> listDV)
    {
        using MySqlConnection conn =
            new MySqlConnection(_connectionString);

        await conn.OpenAsync();

        int totalRows = 0;

        foreach (var dv in listDV)
        {
            string sql = @"
                INSERT INTO don_vi_hd
                (
                    id_ki_thuc_tap,
                    ten_don_vi_hd,
                    gmail_don_vi_hd,
                    password_don_vi_hd
                )
                VALUES
                (
                    @id_ki_thuc_tap,
                    @ten_don_vi_hd,
                    @gmail_don_vi_hd,
                    @password_don_vi_hd
                );
            ";

            using MySqlCommand cmd =
                new MySqlCommand(sql, conn);

            cmd.Parameters.AddWithValue(
                "@id_ki_thuc_tap",
                dv.IdKiThucTap);

            cmd.Parameters.AddWithValue(
                "@ten_don_vi_hd",
                dv.TenDonViHD);

            cmd.Parameters.AddWithValue(
                "@gmail_don_vi_hd",
                dv.GmailDonViHD);

            cmd.Parameters.AddWithValue(
                "@password_don_vi_hd",
                "123456");

            totalRows +=
                await cmd.ExecuteNonQueryAsync();
        }

        return totalRows;
    }

    public async Task<List<Don_Vi_HD_Model>> GetDonViHuongDanByKi(int idKiThucTap)
    {
        List<Don_Vi_HD_Model> result = new();

        using MySqlConnection conn =
            new MySqlConnection(_connectionString);

        await conn.OpenAsync();

        string sql = @"
            SELECT *
            FROM don_vi_hd
            WHERE id_ki_thuc_tap = @id_ki_thuc_tap
            ORDER BY ten_don_vi_hd
        ";

        using MySqlCommand cmd =
            new MySqlCommand(sql, conn);

        cmd.Parameters.AddWithValue("@id_ki_thuc_tap", idKiThucTap);

        using var reader =
            await cmd.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            result.Add(new Don_Vi_HD_Model
            {
                IdDonViHD = reader.GetInt32(reader.GetOrdinal("id_don_vi_hd")),
                IdKiThucTap = reader.GetInt32(reader.GetOrdinal("id_ki_thuc_tap")),
                TenDonViHD = reader.GetString(reader.GetOrdinal("ten_don_vi_hd")),
                GmailDonViHD = reader.IsDBNull(reader.GetOrdinal("gmail_don_vi_hd"))
                    ? string.Empty
                    : reader.GetString(reader.GetOrdinal("gmail_don_vi_hd")),
                PasswordDonViHD = reader.IsDBNull(reader.GetOrdinal("password_don_vi_hd"))
                    ? string.Empty
                    : reader.GetString(reader.GetOrdinal("password_don_vi_hd"))
            });
        }

        return result;
    }

    public async Task<int> AddDonViHuongDan(AddDonViHuongDanDTO request)
    {
        using MySqlConnection conn =
            new MySqlConnection(_connectionString);

        await conn.OpenAsync();

        string sql = @"
            INSERT INTO don_vi_hd
            (
                id_ki_thuc_tap,
                ten_don_vi_hd,
                gmail_don_vi_hd,
                password_don_vi_hd
            )
            VALUES
            (
                @id_ki_thuc_tap,
                @ten_don_vi_hd,
                @gmail_don_vi_hd,
                @password_don_vi_hd
            );

            SELECT LAST_INSERT_ID();
        ";

        using MySqlCommand cmd =
            new MySqlCommand(sql, conn);

        cmd.Parameters.AddWithValue("@id_ki_thuc_tap", request.IdKiThucTap);
        cmd.Parameters.AddWithValue("@ten_don_vi_hd", request.TenDonViHD);
        cmd.Parameters.AddWithValue("@gmail_don_vi_hd", string.IsNullOrWhiteSpace(request.GmailDonViHD) ? DBNull.Value : request.GmailDonViHD);
        cmd.Parameters.AddWithValue("@password_don_vi_hd", "123456");

        var result =
            await cmd.ExecuteScalarAsync();

        return Convert.ToInt32(result);
    }

    public async Task<int> UpdateDonViHuongDan(int idDonViHD, UpdateDonViHuongDanDTO request)
    {
        using MySqlConnection conn =
            new MySqlConnection(_connectionString);

        await conn.OpenAsync();

        string sql = @"
            UPDATE don_vi_hd
            SET ten_don_vi_hd = @ten_don_vi_hd,
                gmail_don_vi_hd = @gmail_don_vi_hd
            WHERE id_don_vi_hd = @id_don_vi_hd
        ";

        using MySqlCommand cmd =
            new MySqlCommand(sql, conn);

        cmd.Parameters.AddWithValue("@ten_don_vi_hd", request.TenDonViHD);
        cmd.Parameters.AddWithValue("@gmail_don_vi_hd", string.IsNullOrWhiteSpace(request.GmailDonViHD) ? DBNull.Value : request.GmailDonViHD);
        cmd.Parameters.AddWithValue("@id_don_vi_hd", idDonViHD);

        return await cmd.ExecuteNonQueryAsync();
    }

    public async Task<int> DeleteDonViHuongDan(int idDonViHD)
    {
        using MySqlConnection conn =
            new MySqlConnection(_connectionString);

        await conn.OpenAsync();

        string sql = @"
            DELETE FROM don_vi_hd
            WHERE id_don_vi_hd = @id_don_vi_hd
        ";

        using MySqlCommand cmd =
            new MySqlCommand(sql, conn);

        cmd.Parameters.AddWithValue("@id_don_vi_hd", idDonViHD);

        return await cmd.ExecuteNonQueryAsync();
    }

    public async Task<int> GetIDDonViHuongDanByName(string NameDVHD,int id_ki_thuc_tap)
    {
        using MySqlConnection conn =
            new MySqlConnection(_connectionString);

        await conn.OpenAsync();

        string sql = @"
            SELECT id_don_vi_hd
            FROM don_vi_hd
            WHERE ten_don_vi_hd = @NameDVHD
            AND id_ki_thuc_tap = @id_ki_thuc_tap
        ";

        using MySqlCommand cmd =
            new MySqlCommand(sql, conn);

        cmd.Parameters.AddWithValue(
            "@NameDVHD",
            NameDVHD);

        cmd.Parameters.AddWithValue(
            "@id_ki_thuc_tap",
            id_ki_thuc_tap);

        var result =
            await cmd.ExecuteScalarAsync();

        if (result == null)
        {
            return 0;
        }

        return Convert.ToInt32(result);
    }

    public async Task<int> GetOrCreateDonViHuongDan(
        string tenDonViHD,
        int idKiThucTap)
    {
        int idDonViHD =
            await GetIDDonViHuongDanByName(
                tenDonViHD,
                idKiThucTap);

        if (idDonViHD > 0)
        {
            return idDonViHD;
        }

        using MySqlConnection conn =
            new MySqlConnection(_connectionString);

        await conn.OpenAsync();

        string sql = @"
            INSERT INTO don_vi_hd
            (
                id_ki_thuc_tap,
                ten_don_vi_hd,
                gmail_don_vi_hd,
                password_don_vi_hd
            )
            VALUES
            (
                @id_ki_thuc_tap,
                @ten_don_vi_hd,
                @gmail_don_vi_hd,
                @password_don_vi_hd
            );

            SELECT LAST_INSERT_ID();
        ";

        using MySqlCommand cmd =
            new MySqlCommand(sql, conn);

        cmd.Parameters.AddWithValue("@id_ki_thuc_tap", idKiThucTap);
        cmd.Parameters.AddWithValue("@ten_don_vi_hd", tenDonViHD);
        cmd.Parameters.AddWithValue("@gmail_don_vi_hd", DBNull.Value);
        cmd.Parameters.AddWithValue("@password_don_vi_hd", "123456");

        var result =
            await cmd.ExecuteScalarAsync();

        return Convert.ToInt32(result);
    }










    public async Task<int> TaoPasswordDonViHD(
        int idDonViHD,
        string password)
    {
        using MySqlConnection conn =
            new MySqlConnection(_connectionString);

        await conn.OpenAsync();

        string sql = @"
            UPDATE don_vi_hd
            SET password_don_vi_hd = @password
            WHERE id_don_vi_hd = @id
        ";

        using MySqlCommand cmd =
            new MySqlCommand(sql, conn);

        cmd.Parameters.AddWithValue("@password", password);

        cmd.Parameters.AddWithValue("@id", idDonViHD);

        return await cmd.ExecuteNonQueryAsync();
    }    

    public async Task<Don_Vi_HD_Model?> GetDonViHuongDanByGmail(string gmail)
    {
        using MySqlConnection conn =
            new MySqlConnection(_connectionString);

        await conn.OpenAsync();

        string sql = @"
            SELECT *
            FROM don_vi_hd
            WHERE gmail_don_vi_hd = @gmail
        ";

        using MySqlCommand cmd =
            new MySqlCommand(sql, conn);

        cmd.Parameters.AddWithValue("@gmail", gmail);

        using var reader =
            await cmd.ExecuteReaderAsync();

        if (!await reader.ReadAsync())
        {
            return null;
        }

        return new Don_Vi_HD_Model
        {
            IdDonViHD = reader.GetInt32(reader.GetOrdinal("id_don_vi_hd")),
            IdKiThucTap = reader.GetInt32(reader.GetOrdinal("id_ki_thuc_tap")),
            TenDonViHD = reader.GetString(reader.GetOrdinal("ten_don_vi_hd")),
            GmailDonViHD = reader.IsDBNull(reader.GetOrdinal("gmail_don_vi_hd"))
                ? string.Empty
                : reader.GetString(reader.GetOrdinal("gmail_don_vi_hd")),
            PasswordDonViHD = reader.IsDBNull(reader.GetOrdinal("password_don_vi_hd"))
                ? string.Empty
                : reader.GetString(reader.GetOrdinal("password_don_vi_hd"))
        };
    }

    public async Task<List<SinhVienPhuTrachDonViHuongDanDTO>> GetSinhVienPhuTrach(
        int idDonViHD)
    {
        List<SinhVienPhuTrachDonViHuongDanDTO> result = new();

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
                svtt.id_don_vi_hd,
                dvhd.ten_don_vi_hd,
                COALESCE(svtt.ma_so_giang_vien, '') AS ma_so_giang_vien,
                COALESCE(gv.ten_giang_vien, '') AS ten_giang_vien,
                COALESCE(svtt.trang_thai, '') AS trang_thai
            FROM sinh_vien_thuc_tap svtt
            INNER JOIN sinh_vien sv
                ON sv.mssv = svtt.mssv
            INNER JOIN ki_thuc_tap ktt
                ON ktt.id_ki_thuc_tap = svtt.id_ki_thuc_tap
            INNER JOIN don_vi_hd dvhd
                ON dvhd.id_don_vi_hd = svtt.id_don_vi_hd
            LEFT JOIN giang_vien gv
                ON gv.ma_so_giang_vien = svtt.ma_so_giang_vien
            WHERE svtt.id_don_vi_hd = @id_don_vi_hd
            ORDER BY ktt.time_batdau DESC, sv.ten_sinh_vien
        ";

        using MySqlCommand cmd =
            new MySqlCommand(sql, conn);

        cmd.Parameters.AddWithValue("@id_don_vi_hd", idDonViHD);

        using var reader =
            await cmd.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            result.Add(new SinhVienPhuTrachDonViHuongDanDTO
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
                IdDonViHD = Convert.ToInt32(reader["id_don_vi_hd"]),
                TenDonViHD = reader["ten_don_vi_hd"].ToString() ?? "",
                MaSoGiangVien = reader["ma_so_giang_vien"].ToString() ?? "",
                TenGiangVien = reader["ten_giang_vien"].ToString() ?? "",
                TrangThai = reader["trang_thai"].ToString() ?? ""
            });
        }

        return result;
    }

    
    
    }
