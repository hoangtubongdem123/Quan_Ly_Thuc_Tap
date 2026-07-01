using MySql.Data.MySqlClient;

public class GiangVienRepository : IGiangVienRepository
{
    private readonly string _connectionString;

    public GiangVienRepository(IConfiguration config)
    {
        _connectionString = config.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Missing DefaultConnection connection string.");
    }

    public async Task<List<Giang_Vien_Model>> GetAll()
    {
        List<Giang_Vien_Model> gv = new();

        using MySqlConnection conn =
            new MySqlConnection(_connectionString);

        await conn.OpenAsync();

        string sql = "SELECT * FROM giang_vien";

        using MySqlCommand cmd =
            new MySqlCommand(sql, conn);

        using var reader =
            await cmd.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            gv.Add(new Giang_Vien_Model
            {
                MaSoGiangVien = reader["ma_so_giang_vien"].ToString()?? "",
                IdKhoa = Convert.ToInt32(reader["id_khoa"]),
                TenGiangVien = reader["ten_giang_vien"].ToString()?? "",
                GmailGiangVien = reader["gmail_giang_vien"].ToString()?? "",
                PasswordGiangVien = reader["password_giang_vien"].ToString()?? ""
            });
        }

        return gv;
    }    

    public async Task<Giang_Vien_Model?> GetGiangVien(string msgv)
    {
        using MySqlConnection conn =
            new MySqlConnection(_connectionString);

        await conn.OpenAsync();

        string sql = "SELECT * FROM giang_vien WHERE ma_so_giang_vien = @msgv ";

        using MySqlCommand cmd =
            new MySqlCommand(sql, conn);


        cmd.Parameters.AddWithValue("@msgv", msgv);
        using var reader =
            await cmd.ExecuteReaderAsync();

        if(await reader.ReadAsync())
        {       return(new Giang_Vien_Model
        {       MaSoGiangVien = reader["ma_so_giang_vien"].ToString()?? "",
                IdKhoa = Convert.ToInt32(reader["id_khoa"]),
                TenGiangVien = reader["ten_giang_vien"].ToString()?? "",
                GmailGiangVien = reader["gmail_giang_vien"].ToString()?? "",
                PasswordGiangVien = reader["password_giang_vien"].ToString()?? ""
            
        });

        }

    

        return null ;
    }

    public async Task<Giang_Vien_Model?> GetGiangVienByGmail(string gmail)
    {
        using MySqlConnection conn =
            new MySqlConnection(_connectionString);

        await conn.OpenAsync();

        string sql = "SELECT * FROM giang_vien WHERE gmail_giang_vien = @gmail ";

        using MySqlCommand cmd =
            new MySqlCommand(sql, conn);


        cmd.Parameters.AddWithValue("@gmail", gmail);
        using var reader =
            await cmd.ExecuteReaderAsync();

        if(await reader.ReadAsync())
        {
            return new Giang_Vien_Model
            {
                MaSoGiangVien = reader["ma_so_giang_vien"].ToString() ?? "",
                IdKhoa = Convert.ToInt32(reader["id_khoa"]),
                TenGiangVien = reader["ten_giang_vien"].ToString() ?? "",
                GmailGiangVien = reader["gmail_giang_vien"].ToString() ?? "",
                PasswordGiangVien = reader["password_giang_vien"].ToString() ?? ""
            };
        }

        return null;
    }

    public async Task<int> AddGiangVien(Giang_Vien_Model request)
    {
        using MySqlConnection conn =
            new MySqlConnection(_connectionString);

        await conn.OpenAsync();

        string sql = @"
            INSERT INTO giang_vien
            (
                ma_so_giang_vien,
                ten_giang_vien,
                id_khoa,
                gmail_giang_vien,
                password_giang_vien
            )
            VALUES
            (
                @ma_so_giang_vien,
                @ten_giang_vien,
                @id_khoa,
                @gmail_giang_vien,
                @password_giang_vien
            )
        ";

        using MySqlCommand cmd =
            new MySqlCommand(sql, conn);

        AddGiangVienParameters(cmd, request);

        return await cmd.ExecuteNonQueryAsync();
    }

    public async Task<int> UpdateGiangVien(
        string maSoGiangVien,
        Giang_Vien_Model request)
    {
        using MySqlConnection conn =
            new MySqlConnection(_connectionString);

        await conn.OpenAsync();

        string sql = @"
            UPDATE giang_vien
            SET
                ten_giang_vien = @ten_giang_vien,
                id_khoa = @id_khoa,
                gmail_giang_vien = @gmail_giang_vien,
                password_giang_vien = @password_giang_vien
            WHERE ma_so_giang_vien = @ma_so_giang_vien
        ";

        using MySqlCommand cmd =
            new MySqlCommand(sql, conn);

        AddGiangVienParameters(cmd, request);
        cmd.Parameters["@ma_so_giang_vien"].Value = maSoGiangVien;

        return await cmd.ExecuteNonQueryAsync();
    }

    public async Task<int> DeleteGiangVien(string maSoGiangVien)
    {
        using MySqlConnection conn =
            new MySqlConnection(_connectionString);

        await conn.OpenAsync();

        string sql = @"
            DELETE FROM giang_vien
            WHERE ma_so_giang_vien = @ma_so_giang_vien
        ";

        using MySqlCommand cmd =
            new MySqlCommand(sql, conn);

        cmd.Parameters.AddWithValue("@ma_so_giang_vien", maSoGiangVien);

        return await cmd.ExecuteNonQueryAsync();
    }

    private static void AddGiangVienParameters(
        MySqlCommand cmd,
        Giang_Vien_Model request)
    {
        cmd.Parameters.AddWithValue("@ma_so_giang_vien", request.MaSoGiangVien);
        cmd.Parameters.AddWithValue("@ten_giang_vien", request.TenGiangVien);
        cmd.Parameters.AddWithValue(
            "@id_khoa",
            request.IdKhoa > 0 ? request.IdKhoa : DBNull.Value);
        cmd.Parameters.AddWithValue("@gmail_giang_vien", request.GmailGiangVien);
        cmd.Parameters.AddWithValue("@password_giang_vien", request.PasswordGiangVien);
    }
    
    public async Task<int> AddGiangVienThucTap(Giang_Vien_Thuc_Tap_Model request)
    {
        using MySqlConnection conn =
            new MySqlConnection(_connectionString);

        await conn.OpenAsync();

        string sql = @"
            INSERT INTO  giang_vien_thuc_tap
            (
                ma_so_giang_vien,
                id_ki_thuc_tap
            )
            VALUES
            (
                @ma_so_giang_vien,
                @id_ki_thuc_tap

            );

            
        ";

        using MySqlCommand cmd =
            new MySqlCommand(sql, conn);

        cmd.Parameters.AddWithValue("@ma_so_giang_vien", request.MaSoGiangVien);
        cmd.Parameters.AddWithValue("@id_ki_thuc_tap", request.IdKiThucTap);


        int rows =
            await cmd.ExecuteNonQueryAsync();

        return rows;
    }

    public async Task<int> CountSinhVienDangHuongDan(
        string maSoGiangVien,
        int idKiThucTap)
    {
        using MySqlConnection conn =
            new MySqlConnection(_connectionString);

        await conn.OpenAsync();

        string sql = @"
            SELECT COUNT(*)
            FROM sinh_vien_thuc_tap
            WHERE ma_so_giang_vien = @ma_so_giang_vien
            AND id_ki_thuc_tap = @id_ki_thuc_tap
        ";

        using MySqlCommand cmd =
            new MySqlCommand(sql, conn);

        cmd.Parameters.AddWithValue("@ma_so_giang_vien", maSoGiangVien);
        cmd.Parameters.AddWithValue("@id_ki_thuc_tap", idKiThucTap);

        var result =
            await cmd.ExecuteScalarAsync();

        return Convert.ToInt32(result);
    }

    public async Task<List<GiangVienPhuTrachCountDTO>> CountSinhVienDangHuongDanByGiangVien()
    {
        List<GiangVienPhuTrachCountDTO> result = new();

        using MySqlConnection conn =
            new MySqlConnection(_connectionString);

        await conn.OpenAsync();

        string sql = @"
            SELECT ma_so_giang_vien, COUNT(*) AS so_sinh_vien
            FROM sinh_vien_thuc_tap
            WHERE ma_so_giang_vien IS NOT NULL
              AND ma_so_giang_vien <> ''
            GROUP BY ma_so_giang_vien
        ";

        using MySqlCommand cmd =
            new MySqlCommand(sql, conn);

        using var reader =
            await cmd.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            result.Add(new GiangVienPhuTrachCountDTO
            {
                MaSoGiangVien = reader["ma_so_giang_vien"].ToString() ?? "",
                SoSinhVien = Convert.ToInt32(reader["so_sinh_vien"])
            });
        }

        return result;
    }

    public async Task<List<SinhVienPhuTrachGiangVienDTO>> GetSinhVienPhuTrach(
        string maSoGiangVien)
    {
        List<SinhVienPhuTrachGiangVienDTO> result = new();

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
                COALESCE(dvhd.ten_don_vi_hd, '') AS ten_don_vi_hd,
                svtt.ma_so_giang_vien,
                gv.ten_giang_vien,
                COALESCE(svtt.trang_thai, '') AS trang_thai
            FROM sinh_vien_thuc_tap svtt
            INNER JOIN sinh_vien sv
                ON sv.mssv = svtt.mssv
            INNER JOIN ki_thuc_tap ktt
                ON ktt.id_ki_thuc_tap = svtt.id_ki_thuc_tap
            INNER JOIN giang_vien gv
                ON gv.ma_so_giang_vien = svtt.ma_so_giang_vien
            LEFT JOIN don_vi_hd dvhd
                ON dvhd.id_don_vi_hd = svtt.id_don_vi_hd
            WHERE svtt.ma_so_giang_vien = @ma_so_giang_vien
            ORDER BY ktt.time_batdau DESC, sv.ten_sinh_vien
        ";

        using MySqlCommand cmd =
            new MySqlCommand(sql, conn);

        cmd.Parameters.AddWithValue("@ma_so_giang_vien", maSoGiangVien);

        using var reader =
            await cmd.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            result.Add(new SinhVienPhuTrachGiangVienDTO
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
                IdDonViHD = reader["id_don_vi_hd"] == DBNull.Value
                    ? null
                    : Convert.ToInt32(reader["id_don_vi_hd"]),
                TenDonViHD = reader["ten_don_vi_hd"].ToString() ?? "",
                MaSoGiangVien = reader["ma_so_giang_vien"].ToString() ?? "",
                TenGiangVien = reader["ten_giang_vien"].ToString() ?? "",
                TrangThai = reader["trang_thai"].ToString() ?? ""
            });
        }

        return result;
    }








    
    }
