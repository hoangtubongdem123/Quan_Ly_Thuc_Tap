using MySql.Data.MySqlClient;

public class DanhGiaThucTapRepository : IDanhGiaThucTapRepository
{
    private readonly string _connectionString;

    public DanhGiaThucTapRepository(IConfiguration config)
    {
        _connectionString = config.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Missing DefaultConnection connection string.");
    }

    public async Task<int> SaveDanhGia(ChamDiemThucTapDTO request)
    {
        using MySqlConnection conn =
            new MySqlConnection(_connectionString);

        await conn.OpenAsync();

        await ValidateSinhVienThucTapForChamDiem(conn, request);

        using var transaction =
            await conn.BeginTransactionAsync();

        string findDanhGiaSql = @"
            SELECT id_danh_gia
            FROM danh_gia_thuc_tap
            WHERE mssv = @mssv
            LIMIT 1;
        ";

        using MySqlCommand findDanhGiaCmd =
            new MySqlCommand(findDanhGiaSql, conn, transaction);

        AddDanhGiaParameters(findDanhGiaCmd, request);

        object? existingId =
            await findDanhGiaCmd.ExecuteScalarAsync();

        int idDanhGia;

        if (existingId == null)
        {
            string insertDanhGiaSql = @"
                INSERT INTO danh_gia_thuc_tap
                (
                    mssv
                )
                VALUES
                (
                    @mssv
                );

                SELECT LAST_INSERT_ID();
            ";

            using MySqlCommand insertDanhGiaCmd =
                new MySqlCommand(insertDanhGiaSql, conn, transaction);

            AddDanhGiaParameters(insertDanhGiaCmd, request);

            idDanhGia =
                Convert.ToInt32(await insertDanhGiaCmd.ExecuteScalarAsync());
        }
        else
        {
            idDanhGia = Convert.ToInt32(existingId);
        }

        string scoreColumn =
            request.Chang == 1
                ? "diem_chang_1"
                : "diem_chang_2";

        string findDiemSql = @"
            SELECT id_diem_clo
            FROM diem_clo
            WHERE id_danh_gia = @id_danh_gia
            AND nguoi_cham_loai = @nguoi_cham_loai
            AND nguoi_cham_id = @nguoi_cham_id
            AND ma_clo = @ma_clo
            LIMIT 1;
        ";

        int totalRows = 0;

        foreach (var diemClo in request.DiemClo)
        {
            using MySqlCommand findDiemCmd =
                new MySqlCommand(findDiemSql, conn, transaction);

            findDiemCmd.Parameters.AddWithValue("@id_danh_gia", idDanhGia);
            findDiemCmd.Parameters.AddWithValue(
                "@nguoi_cham_loai",
                request.NguoiChamLoai.ToUpperInvariant());
            findDiemCmd.Parameters.AddWithValue("@nguoi_cham_id", request.NguoiChamId);
            findDiemCmd.Parameters.AddWithValue("@ma_clo", diemClo.MaClo.ToUpperInvariant());

            object? existingDiemId =
                await findDiemCmd.ExecuteScalarAsync();

            if (existingDiemId == null)
            {
                string insertDiemSql = $@"
                    INSERT INTO diem_clo
                    (
                        id_danh_gia,
                        nguoi_cham_loai,
                        nguoi_cham_id,
                        ma_clo,
                        {scoreColumn}
                    )
                    VALUES
                    (
                        @id_danh_gia,
                        @nguoi_cham_loai,
                        @nguoi_cham_id,
                        @ma_clo,
                        @diem
                    );
                ";

                using MySqlCommand insertDiemCmd =
                    new MySqlCommand(insertDiemSql, conn, transaction);

                insertDiemCmd.Parameters.AddWithValue("@id_danh_gia", idDanhGia);
                insertDiemCmd.Parameters.AddWithValue(
                    "@nguoi_cham_loai",
                    request.NguoiChamLoai.ToUpperInvariant());
                insertDiemCmd.Parameters.AddWithValue("@nguoi_cham_id", request.NguoiChamId);
                insertDiemCmd.Parameters.AddWithValue("@ma_clo", diemClo.MaClo.ToUpperInvariant());
                insertDiemCmd.Parameters.AddWithValue("@diem", diemClo.Diem);

                totalRows += await insertDiemCmd.ExecuteNonQueryAsync();
            }
            else
            {
                string updateDiemSql = $@"
                    UPDATE diem_clo
                    SET {scoreColumn} = @diem
                    WHERE id_diem_clo = @id_diem_clo
                ";

                using MySqlCommand updateDiemCmd =
                    new MySqlCommand(updateDiemSql, conn, transaction);

                updateDiemCmd.Parameters.AddWithValue("@diem", diemClo.Diem);
                updateDiemCmd.Parameters.AddWithValue("@id_diem_clo", existingDiemId);

                totalRows += await updateDiemCmd.ExecuteNonQueryAsync();
            }
        }

        await transaction.CommitAsync();

        return totalRows;
    }

    public async Task<int> UpdateDanhGiaByLoaiNguoiCham(
        ChamDiemThucTapDTO request)
    {
        using MySqlConnection conn =
            new MySqlConnection(_connectionString);

        await conn.OpenAsync();

        await ValidateSinhVienThucTapForChamDiem(conn, request);

        using var transaction =
            await conn.BeginTransactionAsync();

        int idDanhGia =
            await GetOrCreateDanhGia(conn, transaction, request);

        string scoreColumn =
            request.Chang == 1
                ? "diem_chang_1"
                : "diem_chang_2";

        int totalRows = 0;

        foreach (var diemClo in request.DiemClo)
        {
            string updateDiemSql = $@"
                UPDATE diem_clo
                SET {scoreColumn} = @diem
                WHERE id_danh_gia = @id_danh_gia
                AND nguoi_cham_loai = @nguoi_cham_loai
                AND ma_clo = @ma_clo
            ";

            using MySqlCommand updateDiemCmd =
                new MySqlCommand(updateDiemSql, conn, transaction);

            updateDiemCmd.Parameters.AddWithValue("@diem", diemClo.Diem);
            updateDiemCmd.Parameters.AddWithValue("@id_danh_gia", idDanhGia);
            updateDiemCmd.Parameters.AddWithValue(
                "@nguoi_cham_loai",
                request.NguoiChamLoai.ToUpperInvariant());
            updateDiemCmd.Parameters.AddWithValue(
                "@ma_clo",
                diemClo.MaClo.ToUpperInvariant());

            int affectedRows =
                await updateDiemCmd.ExecuteNonQueryAsync();

            if (affectedRows == 0)
            {
                string insertDiemSql = $@"
                    INSERT INTO diem_clo
                    (
                        id_danh_gia,
                        nguoi_cham_loai,
                        nguoi_cham_id,
                        ma_clo,
                        {scoreColumn}
                    )
                    VALUES
                    (
                        @id_danh_gia,
                        @nguoi_cham_loai,
                        @nguoi_cham_id,
                        @ma_clo,
                        @diem
                    );
                ";

                using MySqlCommand insertDiemCmd =
                    new MySqlCommand(insertDiemSql, conn, transaction);

                insertDiemCmd.Parameters.AddWithValue("@id_danh_gia", idDanhGia);
                insertDiemCmd.Parameters.AddWithValue(
                    "@nguoi_cham_loai",
                    request.NguoiChamLoai.ToUpperInvariant());
                insertDiemCmd.Parameters.AddWithValue("@nguoi_cham_id", request.NguoiChamId);
                insertDiemCmd.Parameters.AddWithValue(
                    "@ma_clo",
                    diemClo.MaClo.ToUpperInvariant());
                insertDiemCmd.Parameters.AddWithValue("@diem", diemClo.Diem);

                affectedRows =
                    await insertDiemCmd.ExecuteNonQueryAsync();
            }

            totalRows += affectedRows;
        }

        await transaction.CommitAsync();

        return totalRows;
    }

    public async Task<List<DanhGiaCloRecord>> GetDanhGiaClo(
        string mssv,
        int idKiThucTap)
    {
        using MySqlConnection conn =
            new MySqlConnection(_connectionString);

        await conn.OpenAsync();

        string sql = @"
            SELECT
                dct.nguoi_cham_loai,
                dct.ma_clo,
                AVG(dct.diem_chang_1) AS diem_chang_1,
                AVG(dct.diem_chang_2) AS diem_chang_2
            FROM danh_gia_thuc_tap dgt
            INNER JOIN diem_clo dct
                ON dct.id_danh_gia = dgt.id_danh_gia
            INNER JOIN sinh_vien_thuc_tap svtt
                ON svtt.mssv = dgt.mssv
            WHERE dgt.mssv = @mssv
            AND svtt.id_ki_thuc_tap = @id_ki_thuc_tap
            GROUP BY dct.nguoi_cham_loai, dct.ma_clo
        ";

        using MySqlCommand cmd =
            new MySqlCommand(sql, conn);

        cmd.Parameters.AddWithValue("@mssv", mssv);
        cmd.Parameters.AddWithValue("@id_ki_thuc_tap", idKiThucTap);

        using var reader =
            await cmd.ExecuteReaderAsync();

        List<DanhGiaCloRecord> records = new();

        while (await reader.ReadAsync())
        {
            string nguoiChamLoai =
                reader["nguoi_cham_loai"].ToString() ?? "";

            string maClo =
                reader["ma_clo"].ToString() ?? "";

            if (reader["diem_chang_1"] != DBNull.Value)
            {
                records.Add(new DanhGiaCloRecord
                {
                    Chang = 1,
                    NguoiChamLoai = nguoiChamLoai,
                    MaClo = maClo,
                    Diem = Convert.ToDecimal(reader["diem_chang_1"])
                });
            }

            if (reader["diem_chang_2"] != DBNull.Value)
            {
                records.Add(new DanhGiaCloRecord
                {
                    Chang = 2,
                    NguoiChamLoai = nguoiChamLoai,
                    MaClo = maClo,
                    Diem = Convert.ToDecimal(reader["diem_chang_2"])
                });
            }
        }

        return records;
    }

    public async Task<TieuChiDanhGiaConfig?> GetTieuChiByKiThucTap(
        int idKiThucTap)
    {
        using MySqlConnection conn =
            new MySqlConnection(_connectionString);

        await conn.OpenAsync();

        return await GetTieuChiByKiThucTap(conn, idKiThucTap);
    }

    public async Task<TieuChiDanhGiaConfig?> GetTieuChiBySinhVien(
        string mssv,
        int? idKiThucTap)
    {
        using MySqlConnection conn =
            new MySqlConnection(_connectionString);

        await conn.OpenAsync();

        string sql = @"
            SELECT id_ki_thuc_tap
            FROM sinh_vien_thuc_tap
            WHERE mssv = @mssv
        ";

        if (idKiThucTap.HasValue)
        {
            sql += " AND id_ki_thuc_tap = @id_ki_thuc_tap";
        }

        sql += @"
            ORDER BY id_ki_thuc_tap DESC
            LIMIT 1
        ";

        using MySqlCommand cmd =
            new MySqlCommand(sql, conn);

        cmd.Parameters.AddWithValue("@mssv", mssv);

        if (idKiThucTap.HasValue)
        {
            cmd.Parameters.AddWithValue("@id_ki_thuc_tap", idKiThucTap.Value);
        }

        object? result =
            await cmd.ExecuteScalarAsync();

        if (result == null || result == DBNull.Value)
        {
            return null;
        }

        return await GetTieuChiByKiThucTap(
            conn,
            Convert.ToInt32(result));
    }

    private static void AddDanhGiaParameters(
        MySqlCommand cmd,
        ChamDiemThucTapDTO request)
    {
        cmd.Parameters.AddWithValue("@mssv", request.MSSV);
    }

    private static async Task<int> GetOrCreateDanhGia(
        MySqlConnection conn,
        System.Data.Common.DbTransaction transaction,
        ChamDiemThucTapDTO request)
    {
        string findDanhGiaSql = @"
            SELECT id_danh_gia
            FROM danh_gia_thuc_tap
            WHERE mssv = @mssv
            LIMIT 1;
        ";

        using MySqlCommand findDanhGiaCmd =
            new MySqlCommand(findDanhGiaSql, conn, (MySqlTransaction)transaction);

        AddDanhGiaParameters(findDanhGiaCmd, request);

        object? existingId =
            await findDanhGiaCmd.ExecuteScalarAsync();

        if (existingId != null)
        {
            return Convert.ToInt32(existingId);
        }

        string insertDanhGiaSql = @"
            INSERT INTO danh_gia_thuc_tap
            (
                mssv
            )
            VALUES
            (
                @mssv
            );

            SELECT LAST_INSERT_ID();
        ";

        using MySqlCommand insertDanhGiaCmd =
            new MySqlCommand(insertDanhGiaSql, conn, (MySqlTransaction)transaction);

        AddDanhGiaParameters(insertDanhGiaCmd, request);

        return Convert.ToInt32(await insertDanhGiaCmd.ExecuteScalarAsync());
    }

    private static async Task<TieuChiDanhGiaConfig?> GetTieuChiByKiThucTap(
        MySqlConnection conn,
        int idKiThucTap)
    {
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
            FROM ki_thuc_tap ktt
            INNER JOIN tieu_chi_danh_gia tcdg
                ON tcdg.id_tieu_chi = COALESCE(
                    ktt.id_tieu_chi,
                    (
                        SELECT MAX(tcdg2.id_tieu_chi)
                        FROM tieu_chi_danh_gia tcdg2
                        WHERE tcdg2.id_khoa = ktt.id_khoa
                    )
                )
            INNER JOIN tieu_chi_clo tcc
                ON tcc.id_tieu_chi = tcdg.id_tieu_chi
            WHERE ktt.id_ki_thuc_tap = @id_ki_thuc_tap
            ORDER BY tcc.id_clo
        ";

        using MySqlCommand cmd =
            new MySqlCommand(sql, conn);

        cmd.Parameters.AddWithValue("@id_ki_thuc_tap", idKiThucTap);

        using var reader =
            await cmd.ExecuteReaderAsync();

        TieuChiDanhGiaConfig? config = null;

        while (await reader.ReadAsync())
        {
            config ??= new TieuChiDanhGiaConfig
            {
                IdTieuChi = Convert.ToInt32(reader["id_tieu_chi"]),
                TenTieuChi = reader["ten_tieu_chi"].ToString() ?? "",
                IdKhoa = Convert.ToInt32(reader["id_khoa"]),
                PhanTramChang1 = Convert.ToDecimal(reader["phan_tram_chang_1"]),
                PhanTramChang2 = Convert.ToDecimal(reader["phan_tram_chang_2"])
            };

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

        return config;
    }

    private static async Task ValidateSinhVienThucTapForChamDiem(
        MySqlConnection conn,
        ChamDiemThucTapDTO request)
    {
        string countSinhVienSql = @"
            SELECT COUNT(*)
            FROM sinh_vien_thuc_tap
            WHERE mssv = @mssv
        ";

        if (request.IdKiThucTap.HasValue)
        {
            countSinhVienSql += " AND id_ki_thuc_tap = @id_ki_thuc_tap";
        }

        using MySqlCommand countSinhVienCmd =
            new MySqlCommand(countSinhVienSql, conn);

        countSinhVienCmd.Parameters.AddWithValue("@mssv", request.MSSV);

        if (request.IdKiThucTap.HasValue)
        {
            countSinhVienCmd.Parameters.AddWithValue(
                "@id_ki_thuc_tap",
                request.IdKiThucTap.Value);
        }

        int soDongSinhVien =
            Convert.ToInt32(await countSinhVienCmd.ExecuteScalarAsync());

        if (soDongSinhVien == 0)
        {
            throw new ArgumentException("MSSV chưa có trong bảng sinh viên thực tập.");
        }

        string countKiThucTapSql = @"
            SELECT COUNT(*)
            FROM sinh_vien_thuc_tap
            WHERE mssv = @mssv
            AND id_ki_thuc_tap IS NOT NULL
        ";

        if (request.IdKiThucTap.HasValue)
        {
            countKiThucTapSql += " AND id_ki_thuc_tap = @id_ki_thuc_tap";
        }

        using MySqlCommand countKiThucTapCmd =
            new MySqlCommand(countKiThucTapSql, conn);

        countKiThucTapCmd.Parameters.AddWithValue("@mssv", request.MSSV);

        if (request.IdKiThucTap.HasValue)
        {
            countKiThucTapCmd.Parameters.AddWithValue(
                "@id_ki_thuc_tap",
                request.IdKiThucTap.Value);
        }

        int soDongKiThucTap =
            Convert.ToInt32(await countKiThucTapCmd.ExecuteScalarAsync());

        if (soDongKiThucTap == 0)
        {
            throw new ArgumentException("ID kỳ thực tập chưa có trong bảng sinh viên thực tập.");
        }

        string sinhVienThucTapSql = @"
            SELECT id_don_vi_hd, ma_so_giang_vien
            FROM sinh_vien_thuc_tap
            WHERE mssv = @mssv
        ";

        if (request.IdKiThucTap.HasValue)
        {
            sinhVienThucTapSql += " AND id_ki_thuc_tap = @id_ki_thuc_tap";
        }

        sinhVienThucTapSql += @"
            ORDER BY id_ki_thuc_tap DESC
            LIMIT 1
        ";

        using MySqlCommand sinhVienThucTapCmd =
            new MySqlCommand(sinhVienThucTapSql, conn);

        sinhVienThucTapCmd.Parameters.AddWithValue("@mssv", request.MSSV);

        if (request.IdKiThucTap.HasValue)
        {
            sinhVienThucTapCmd.Parameters.AddWithValue(
                "@id_ki_thuc_tap",
                request.IdKiThucTap.Value);
        }
        using var reader =
            await sinhVienThucTapCmd.ExecuteReaderAsync();

        if (!await reader.ReadAsync())
        {
            throw new ArgumentException("Sinh viên chưa có trong kỳ thực tập này.");
        }

        object idDonViHdValue = reader["id_don_vi_hd"];
        object maSoGiangVienValue = reader["ma_so_giang_vien"];

        string nguoiChamLoai =
            request.NguoiChamLoai.ToUpperInvariant();

        if (nguoiChamLoai == "GVHD")
        {
            string maSoGiangVien =
                maSoGiangVienValue == DBNull.Value
                    ? ""
                    : maSoGiangVienValue.ToString() ?? "";

            if (!maSoGiangVien.Equals(
                    request.NguoiChamId,
                    StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException("Mã giảng viên chấm không khớp với GVHD của sinh viên thực tập.");
            }

            return;
        }

        if (nguoiChamLoai == "DVHD")
        {
            if (!int.TryParse(request.NguoiChamId, out int idDonViCham))
            {
                throw new ArgumentException("ID đơn vị hướng dẫn chấm phải là số.");
            }

            int? idDonViHuongDan =
                idDonViHdValue == DBNull.Value
                    ? null
                    : Convert.ToInt32(idDonViHdValue);

            if (idDonViHuongDan != idDonViCham)
            {
                throw new ArgumentException("ID đơn vị hướng dẫn chấm không khớp với ĐVHD của sinh viên thực tập.");
            }
        }
    }
}
