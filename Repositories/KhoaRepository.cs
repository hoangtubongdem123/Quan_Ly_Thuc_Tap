using MySql.Data.MySqlClient;
public class KhoaRepository : IKhoaRepository
{
    private readonly string _connectionString;

    public KhoaRepository(IConfiguration config)
    {
        _connectionString = config.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Missing DefaultConnection connection string.");
    }

    public async Task<List<Khoa_Model>> GetAll()
    {
        List<Khoa_Model> khoa = new();

        using MySqlConnection conn =
            new MySqlConnection(_connectionString);

        await conn.OpenAsync();

        string sql = "SELECT * FROM khoa";

        using MySqlCommand cmd =
            new MySqlCommand(sql, conn);

        using var reader =
            await cmd.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            khoa.Add(new Khoa_Model
            {

                IdKhoa = Convert.ToInt32(
                    reader["id_khoa"]
                ),

                TenKhoa = reader["ten_khoa"].ToString()?? "",

                GmailKhoa = reader["gmail_khoa"].ToString()?? "",

                PasswordKhoa = reader["password_khoa"]
                    .ToString()?? ""
            });
        }

        return khoa;
    }


    public async Task<Khoa_Model?> GetKhoa(string id_khoa)
    {
        using MySqlConnection conn =
            new MySqlConnection(_connectionString);

        await conn.OpenAsync();

        string sql = "SELECT * FROM khoa WHERE id_khoa = @id_khoa ";

        using MySqlCommand cmd =
            new MySqlCommand(sql, conn);


        cmd.Parameters.AddWithValue("@id_khoa", id_khoa);
        using var reader =
            await cmd.ExecuteReaderAsync();

        if (await reader.ReadAsync())
        {
            return new Khoa_Model
            {
                IdKhoa = Convert.ToInt32(reader["id_khoa"]),
                TenKhoa = reader["ten_khoa"].ToString() ?? "",
                GmailKhoa = reader["gmail_khoa"].ToString() ?? "",
                PasswordKhoa = reader["password_khoa"].ToString() ?? "",
                PathCauHinhDiem = reader["path_cau_hinh_diem"].ToString() ?? ""
            };
        }

        return null;
    }    


    public async Task<Khoa_Model?> GetKhoaByGmail(string gmail)
    {
        using MySqlConnection conn =
            new MySqlConnection(_connectionString);

        await conn.OpenAsync();

        string sql = "SELECT * FROM khoa WHERE gmail_khoa = @gmail ";

        using MySqlCommand cmd =
            new MySqlCommand(sql, conn);


        cmd.Parameters.AddWithValue("@gmail", gmail);
        using var reader =
            await cmd.ExecuteReaderAsync();

        if (await reader.ReadAsync())
        {
            return new Khoa_Model
            {
                IdKhoa = Convert.ToInt32(reader["id_khoa"]),
                TenKhoa = reader["ten_khoa"].ToString()?? "",
                GmailKhoa = reader["gmail_khoa"].ToString()?? "",
                PasswordKhoa = reader["password_khoa"].ToString()?? ""
            };
        }

        return null;
    }  

    public async Task<int> AddKhoa(Khoa_Model request)
    {
        using MySqlConnection conn =
            new MySqlConnection(_connectionString);

        await conn.OpenAsync();

        string sql = @"
            INSERT INTO khoa
            (
                ten_khoa,
                gmail_khoa,
                password_khoa,
                path_cau_hinh_diem
            )
            VALUES
            (
                @ten_khoa,
                @gmail_khoa,
                @password_khoa,
                @path_cau_hinh_diem
            );

            SELECT LAST_INSERT_ID();
        ";

        using MySqlCommand cmd =
            new MySqlCommand(sql, conn);

        AddKhoaParameters(cmd, request);

        var result =
            await cmd.ExecuteScalarAsync();

        return Convert.ToInt32(result);
    }

    public async Task<int> UpdateKhoa(int idKhoa, Khoa_Model request)
    {
        using MySqlConnection conn =
            new MySqlConnection(_connectionString);

        await conn.OpenAsync();

        string sql = @"
            UPDATE khoa
            SET
                ten_khoa = @ten_khoa,
                gmail_khoa = @gmail_khoa,
                password_khoa = @password_khoa,
                path_cau_hinh_diem = @path_cau_hinh_diem
            WHERE id_khoa = @id_khoa
        ";

        using MySqlCommand cmd =
            new MySqlCommand(sql, conn);

        AddKhoaParameters(cmd, request);
        cmd.Parameters.AddWithValue("@id_khoa", idKhoa);

        return await cmd.ExecuteNonQueryAsync();
    }

    public async Task<int> DeleteKhoa(int idKhoa)
    {
        using MySqlConnection conn =
            new MySqlConnection(_connectionString);

        await conn.OpenAsync();

        string sql = @"
            DELETE FROM khoa
            WHERE id_khoa = @id_khoa
        ";

        using MySqlCommand cmd =
            new MySqlCommand(sql, conn);

        cmd.Parameters.AddWithValue("@id_khoa", idKhoa);

        return await cmd.ExecuteNonQueryAsync();
    }

    private static void AddKhoaParameters(
        MySqlCommand cmd,
        Khoa_Model request)
    {
        cmd.Parameters.AddWithValue("@ten_khoa", request.TenKhoa);
        cmd.Parameters.AddWithValue("@gmail_khoa", request.GmailKhoa);
        cmd.Parameters.AddWithValue("@password_khoa", request.PasswordKhoa);
        cmd.Parameters.AddWithValue(
            "@path_cau_hinh_diem",
            string.IsNullOrWhiteSpace(request.PathCauHinhDiem)
                ? DBNull.Value
                : request.PathCauHinhDiem);
    }


public async Task<string?> GetMSSVKiThucTapByDONDKI(
    int id_don_dki)
{
    using MySqlConnection conn =
        new MySqlConnection(_connectionString);

    await conn.OpenAsync();

    string sql = @"
        SELECT mssv
        FROM don_dang_ki_thuc_tap
        WHERE id_don_dki = @id_don_dki
        LIMIT 1
    ";

    using MySqlCommand cmd =
        new MySqlCommand(sql, conn);

    cmd.Parameters.AddWithValue(
        "@id_don_dki",
        id_don_dki);

    var result =
        await cmd.ExecuteScalarAsync();

    if (result == null)
    {
        return null;
    }

    return result.ToString();
}






    public async Task<int> AddThucTap(KiThucTapDTO request)
    {
        using MySqlConnection conn =
            new MySqlConnection(_connectionString);

        await conn.OpenAsync();

        string sql = @"
            INSERT INTO ki_thuc_tap
            (
                ten_ki_thuc_tap,
                time_batdau,
                time_ketthuc,
                trang_thai,
                id_khoa,
                id_tieu_chi
            )
            VALUES
            (
                @ten,
                @batdau,
                @ketthuc,
                @trangthai,
                @id_khoa,
                @id_tieu_chi
            );

            SELECT LAST_INSERT_ID();
        ";

        using MySqlCommand cmd =
            new MySqlCommand(sql, conn);

        DateTime timebatdau =
            request.TimeBatDau ?? DateTime.Now;

        DateTime timeketthuc =
            request.TimeKetThuc ?? timebatdau.AddMonths(2);

        string trangThai =
            string.IsNullOrWhiteSpace(request.TrangThai)
                ? "Đang mở"
                : request.TrangThai;

        cmd.Parameters.AddWithValue("@ten", request.TenKiThucTap);
        cmd.Parameters.AddWithValue("@batdau", timebatdau);
        cmd.Parameters.AddWithValue("@ketthuc", timeketthuc);
        cmd.Parameters.AddWithValue("@trangthai", trangThai);
        cmd.Parameters.AddWithValue("@id_khoa", request.IdKhoa);
        cmd.Parameters.AddWithValue(
            "@id_tieu_chi",
            request.IdTieuChi.HasValue ? request.IdTieuChi.Value : DBNull.Value);

        var result = await cmd.ExecuteScalarAsync();

        return Convert.ToInt32(result);
    }

    public async Task<bool> KiThucTapExists(int idKiThucTap)
    {
        using MySqlConnection conn =
            new MySqlConnection(_connectionString);

        await conn.OpenAsync();

        string sql = @"
            SELECT COUNT(*)
            FROM ki_thuc_tap
            WHERE id_ki_thuc_tap = @id_ki_thuc_tap
        ";

        using MySqlCommand cmd =
            new MySqlCommand(sql, conn);

        cmd.Parameters.AddWithValue(
            "@id_ki_thuc_tap",
            idKiThucTap);

        var result =
            await cmd.ExecuteScalarAsync();

        return Convert.ToInt32(result) > 0;
    }

    public async Task<List<Ki_Thuc_Tap_Model>> GetKiThucTapByKhoa(int idKhoa)
    {
        List<Ki_Thuc_Tap_Model> kiThucTap = new();

        using MySqlConnection conn =
            new MySqlConnection(_connectionString);

        await conn.OpenAsync();

        string sql = @"
            SELECT *
            FROM ki_thuc_tap
            WHERE id_khoa = @id_khoa
            ORDER BY id_ki_thuc_tap DESC
        ";

        using MySqlCommand cmd =
            new MySqlCommand(sql, conn);

        cmd.Parameters.AddWithValue("@id_khoa", idKhoa);

        using var reader =
            await cmd.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            kiThucTap.Add(new Ki_Thuc_Tap_Model
            {
                IdKiThucTap = Convert.ToInt32(reader["id_ki_thuc_tap"]),
                TenKiThucTap = reader["ten_ki_thuc_tap"].ToString() ?? "",
                TimeBatDau = Convert.ToDateTime(reader["time_batdau"]),
                TimeKetThuc = Convert.ToDateTime(reader["time_ketthuc"]),
                IdKhoa = Convert.ToInt32(reader["id_khoa"]),
                IdTieuChi = reader["id_tieu_chi"] == DBNull.Value
                    ? null
                    : Convert.ToInt32(reader["id_tieu_chi"]),
                TrangThai = reader["trang_thai"].ToString() ?? ""
            });
        }

        return kiThucTap;
    }

    public async Task<int> UpdateKiThucTap(
        int idKiThucTap,
        UpdateKiThucTapDTO request)
    {
        using MySqlConnection conn =
            new MySqlConnection(_connectionString);

        await conn.OpenAsync();

        string sql = @"
            UPDATE ki_thuc_tap
            SET
                ten_ki_thuc_tap = @ten,
                time_batdau = @batdau,
                time_ketthuc = @ketthuc,
                trang_thai = @trangthai,
                id_tieu_chi = @id_tieu_chi
            WHERE id_ki_thuc_tap = @id_ki_thuc_tap
        ";

        using MySqlCommand cmd =
            new MySqlCommand(sql, conn);

        cmd.Parameters.AddWithValue("@ten", request.TenKiThucTap);
        cmd.Parameters.AddWithValue("@batdau", request.TimeBatDau);
        cmd.Parameters.AddWithValue("@ketthuc", request.TimeKetThuc);
        cmd.Parameters.AddWithValue("@trangthai", request.TrangThai);
        cmd.Parameters.AddWithValue(
            "@id_tieu_chi",
            request.IdTieuChi.HasValue ? request.IdTieuChi.Value : DBNull.Value);
        cmd.Parameters.AddWithValue("@id_ki_thuc_tap", idKiThucTap);

        return await cmd.ExecuteNonQueryAsync();
    }

    public async Task<int> DeleteKiThucTap(int idKiThucTap)
    {
        using MySqlConnection conn =
            new MySqlConnection(_connectionString);

        await conn.OpenAsync();

        string sql = @"
            DELETE FROM ki_thuc_tap
            WHERE id_ki_thuc_tap = @id_ki_thuc_tap
        ";

        using MySqlCommand cmd =
            new MySqlCommand(sql, conn);

        cmd.Parameters.AddWithValue("@id_ki_thuc_tap", idKiThucTap);

        return await cmd.ExecuteNonQueryAsync();
    }

    public async Task<List<SinhVienThucTapKhoaDTO>> GetSinhVienThucTapByKi(
        int idKiThucTap)
    {
        List<SinhVienThucTapKhoaDTO> students = new();

        using MySqlConnection conn =
            new MySqlConnection(_connectionString);

        await conn.OpenAsync();

        string sql = @"
            SELECT
                svtt.mssv,
                sv.ten_sinh_vien,
                svtt.id_ki_thuc_tap,
                svtt.id_don_vi_hd,
                COALESCE(dvhd.ten_don_vi_hd, '') AS ten_don_vi_hd,
                COALESCE(svtt.ma_so_giang_vien, '') AS ma_so_giang_vien,
                COALESCE(gv.ten_giang_vien, '') AS ten_giang_vien,
                svtt.trang_thai
            FROM sinh_vien_thuc_tap svtt
            INNER JOIN sinh_vien sv
                ON sv.mssv = svtt.mssv
            LEFT JOIN don_vi_hd dvhd
                ON dvhd.id_don_vi_hd = svtt.id_don_vi_hd
            LEFT JOIN giang_vien gv
                ON gv.ma_so_giang_vien = svtt.ma_so_giang_vien
            WHERE svtt.id_ki_thuc_tap = @id_ki_thuc_tap
            ORDER BY sv.ten_sinh_vien, svtt.mssv
        ";

        using MySqlCommand cmd =
            new MySqlCommand(sql, conn);

        cmd.Parameters.AddWithValue("@id_ki_thuc_tap", idKiThucTap);

        using var reader =
            await cmd.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            students.Add(new SinhVienThucTapKhoaDTO
            {
                MSSV = reader["mssv"].ToString() ?? "",
                TenSinhVien = reader["ten_sinh_vien"].ToString() ?? "",
                IdKiThucTap = Convert.ToInt32(reader["id_ki_thuc_tap"]),
                IdDonViHD = reader["id_don_vi_hd"] == DBNull.Value
                    ? null
                    : Convert.ToInt32(reader["id_don_vi_hd"]),
                TenDonViHD = reader["ten_don_vi_hd"].ToString() ?? "",
                MaSoGiangVien = reader["ma_so_giang_vien"].ToString() ?? "",
                TenGiangVien = reader["ten_giang_vien"].ToString() ?? "",
                TrangThai = reader["trang_thai"].ToString() ?? ""
            });
        }

        return students;
    }

    public async Task<List<TieuChiDanhGiaConfig>> GetTieuChiByKhoa(int idKhoa)
    {
        using MySqlConnection conn =
            new MySqlConnection(_connectionString);

        await conn.OpenAsync();

        string sql = @"
            SELECT
                tcdg.id_tieu_chi,
                tcdg.ten_tieu_chi,
                tcdg.id_khoa,
                tcdg.phan_tram_chang_1,
                tcdg.phan_tram_chang_2,
                tcc.id_clo,
                tcc.ten_clo,
                tcc.mo_ta_clo,
                tcc.trong_so_hp,
                tcc.trong_so_dvhd,
                tcc.trong_so_gvhd
            FROM tieu_chi_danh_gia tcdg
            LEFT JOIN tieu_chi_clo tcc
                ON tcc.id_tieu_chi = tcdg.id_tieu_chi
            WHERE tcdg.id_khoa = @id_khoa
            ORDER BY tcdg.id_tieu_chi DESC, tcc.id_clo
        ";

        using MySqlCommand cmd =
            new MySqlCommand(sql, conn);

        cmd.Parameters.AddWithValue("@id_khoa", idKhoa);

        using var reader =
            await cmd.ExecuteReaderAsync();

        Dictionary<int, TieuChiDanhGiaConfig> configs = new();

        while (await reader.ReadAsync())
        {
            int idTieuChi =
                Convert.ToInt32(reader["id_tieu_chi"]);

            if (!configs.TryGetValue(idTieuChi, out var config))
            {
                config = new TieuChiDanhGiaConfig
                {
                    IdTieuChi = idTieuChi,
                    TenTieuChi = reader["ten_tieu_chi"].ToString() ?? "",
                    IdKhoa = Convert.ToInt32(reader["id_khoa"]),
                    PhanTramChang1 = Convert.ToDecimal(reader["phan_tram_chang_1"]),
                    PhanTramChang2 = Convert.ToDecimal(reader["phan_tram_chang_2"])
                };

                configs.Add(idTieuChi, config);
            }

            if (reader["id_clo"] != DBNull.Value)
            {
                config.Clos.Add(new TieuChiCloConfig
                {
                    IdClo = Convert.ToInt32(reader["id_clo"]),
                    TenClo = reader["ten_clo"].ToString() ?? "",
                    MoTaClo = reader["mo_ta_clo"].ToString() ?? "",
                    TrongSoHocPhan = Convert.ToDecimal(reader["trong_so_hp"]),
                    TrongSoDonViHuongDan = Convert.ToDecimal(reader["trong_so_dvhd"]),
                    TrongSoGiangVienHuongDan = Convert.ToDecimal(reader["trong_so_gvhd"])
                });
            }
        }

        return configs.Values.ToList();
    }

    public async Task<int> AddTieuChi(TieuChiDanhGiaDTO request)
    {
        using MySqlConnection conn =
            new MySqlConnection(_connectionString);

        await conn.OpenAsync();

        using MySqlTransaction transaction =
            conn.BeginTransaction();

        string insertTieuChiSql = @"
            INSERT INTO tieu_chi_danh_gia
            (
                ten_tieu_chi,
                id_khoa,
                phan_tram_chang_1,
                phan_tram_chang_2
            )
            VALUES
            (
                @ten_tieu_chi,
                @id_khoa,
                @phan_tram_chang_1,
                @phan_tram_chang_2
            );

            SELECT LAST_INSERT_ID();
        ";

        using MySqlCommand insertTieuChiCmd =
            new MySqlCommand(insertTieuChiSql, conn, transaction);

        AddTieuChiParameters(insertTieuChiCmd, request);

        int idTieuChi =
            Convert.ToInt32(await insertTieuChiCmd.ExecuteScalarAsync());

        await InsertClos(conn, transaction, idTieuChi, request.Clos);

        await transaction.CommitAsync();

        return idTieuChi;
    }

    public async Task<int> UpdateTieuChi(
        int idTieuChi,
        TieuChiDanhGiaDTO request)
    {
        using MySqlConnection conn =
            new MySqlConnection(_connectionString);

        await conn.OpenAsync();

        using MySqlTransaction transaction =
            conn.BeginTransaction();

        string updateTieuChiSql = @"
            UPDATE tieu_chi_danh_gia
            SET
                ten_tieu_chi = @ten_tieu_chi,
                id_khoa = @id_khoa,
                phan_tram_chang_1 = @phan_tram_chang_1,
                phan_tram_chang_2 = @phan_tram_chang_2
            WHERE id_tieu_chi = @id_tieu_chi
        ";

        using MySqlCommand updateTieuChiCmd =
            new MySqlCommand(updateTieuChiSql, conn, transaction);

        AddTieuChiParameters(updateTieuChiCmd, request);
        updateTieuChiCmd.Parameters.AddWithValue("@id_tieu_chi", idTieuChi);

        int affectedRows =
            await updateTieuChiCmd.ExecuteNonQueryAsync();

        if (affectedRows == 0)
        {
            await transaction.RollbackAsync();
            return 0;
        }

        string deleteCloSql = @"
            DELETE FROM tieu_chi_clo
            WHERE id_tieu_chi = @id_tieu_chi
        ";

        using MySqlCommand deleteCloCmd =
            new MySqlCommand(deleteCloSql, conn, transaction);

        deleteCloCmd.Parameters.AddWithValue("@id_tieu_chi", idTieuChi);
        await deleteCloCmd.ExecuteNonQueryAsync();

        await InsertClos(conn, transaction, idTieuChi, request.Clos);

        await transaction.CommitAsync();

        return affectedRows;
    }

    public async Task<int> DeleteTieuChi(int idTieuChi)
    {
        using MySqlConnection conn =
            new MySqlConnection(_connectionString);

        await conn.OpenAsync();

        string sql = @"
            DELETE FROM tieu_chi_danh_gia
            WHERE id_tieu_chi = @id_tieu_chi
        ";

        using MySqlCommand cmd =
            new MySqlCommand(sql, conn);

        cmd.Parameters.AddWithValue("@id_tieu_chi", idTieuChi);

        return await cmd.ExecuteNonQueryAsync();
    }

    private static void AddTieuChiParameters(
        MySqlCommand cmd,
        TieuChiDanhGiaDTO request)
    {
        cmd.Parameters.AddWithValue("@ten_tieu_chi", request.TenTieuChi);
        cmd.Parameters.AddWithValue("@id_khoa", request.IdKhoa);
        cmd.Parameters.AddWithValue("@phan_tram_chang_1", request.PhanTramChang1);
        cmd.Parameters.AddWithValue("@phan_tram_chang_2", request.PhanTramChang2);
    }

    private static async Task InsertClos(
        MySqlConnection conn,
        MySqlTransaction transaction,
        int idTieuChi,
        List<TieuChiCloDTO> clos)
    {
        string sql = @"
            INSERT INTO tieu_chi_clo
            (
                ten_clo,
                id_tieu_chi,
                mo_ta_clo,
                trong_so_hp,
                trong_so_dvhd,
                trong_so_gvhd
            )
            VALUES
            (
                @ten_clo,
                @id_tieu_chi,
                @mo_ta_clo,
                @trong_so_hp,
                @trong_so_dvhd,
                @trong_so_gvhd
            )
        ";

        foreach (var clo in clos)
        {
            using MySqlCommand cmd =
                new MySqlCommand(sql, conn, transaction);

            cmd.Parameters.AddWithValue("@ten_clo", clo.TenClo);
            cmd.Parameters.AddWithValue("@id_tieu_chi", idTieuChi);
            cmd.Parameters.AddWithValue("@mo_ta_clo", clo.MoTaClo);
            cmd.Parameters.AddWithValue("@trong_so_hp", clo.TrongSoHp);
            cmd.Parameters.AddWithValue("@trong_so_dvhd", clo.TrongSoDvhd);
            cmd.Parameters.AddWithValue("@trong_so_gvhd", clo.TrongSoGvhd);

            await cmd.ExecuteNonQueryAsync();
        }
    }



}    
