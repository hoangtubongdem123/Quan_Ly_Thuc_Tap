public class DanhGiaThucTapService : IDanhGiaThucTapService
{
    private readonly IDanhGiaThucTapRepository _repository;

    public DanhGiaThucTapService(
        IDanhGiaThucTapRepository repository)
    {
        _repository = repository;
    }

    public async Task<int> ChamDiem(ChamDiemThucTapDTO request)
    {
        TieuChiDanhGiaConfig config =
            await ValidateChamDiem(request);

        return await _repository.SaveDanhGia(request);
    }

    public async Task<int> CapNhatDiemTuKhoa(ChamDiemThucTapDTO request)
    {
        await ValidateChamDiem(request);

        return await _repository.UpdateDanhGiaByLoaiNguoiCham(request);
    }

    private async Task<TieuChiDanhGiaConfig> ValidateChamDiem(
        ChamDiemThucTapDTO request)
    {
        ValidateChamDiemCoBan(request);

        request.NguoiChamLoai =
            request.NguoiChamLoai.ToUpperInvariant();

        TieuChiDanhGiaConfig config =
            await GetTieuChiForChamDiem(request);

        foreach (var diem in request.DiemClo)
        {
            diem.MaClo = diem.MaClo.Trim().ToUpperInvariant();

            if (!config.Clos.Any(item =>
                    item.TenClo.Equals(diem.MaClo, StringComparison.OrdinalIgnoreCase)))
            {
                throw new ArgumentException($"Mã CLO không hợp lệ: {diem.MaClo}.");
            }

            if (diem.Diem < 0m || diem.Diem > 10m)
            {
                throw new ArgumentException("Điểm CLO phải nằm trong khoảng 0 đến 10.");
            }
        }

        return config;
    }

    public async Task<TieuChiDanhGiaConfig> GetTieuChi(
        int idKiThucTap)
    {
        return await GetTieuChiForKiThucTap(idKiThucTap);
    }

    public async Task<KetQuaDanhGiaThucTapDTO> GetKetQua(
        string mssv,
        int idKiThucTap)
    {
        TieuChiDanhGiaConfig config =
            await GetTieuChiForKiThucTap(idKiThucTap);

        var records =
            await _repository.GetDanhGiaClo(mssv, idKiThucTap);

        KetQuaDanhGiaThucTapDTO result = new()
        {
            MSSV = mssv,
            IdKiThucTap = idKiThucTap
        };

        decimal diemHocPhan = 0m;
        bool duDuLieuTatCaClo = true;
        decimal trongSoChang1 = NormalizeWeight(config.PhanTramChang1);
        decimal trongSoChang2 = NormalizeWeight(config.PhanTramChang2);

        foreach (var clo in config.Clos)
        {
            decimal? diemChang1Dvhd =
                GetDiemNguoiCham(records, clo.TenClo, 1, "DVHD");

            decimal? diemChang1Gvhd =
                GetDiemNguoiCham(records, clo.TenClo, 1, "GVHD");

            decimal? diemChang1 =
                TinhDiemChang(records, clo, 1);

            decimal? diemChang2Dvhd =
                GetDiemNguoiCham(records, clo.TenClo, 2, "DVHD");

            decimal? diemChang2Gvhd =
                GetDiemNguoiCham(records, clo.TenClo, 2, "GVHD");

            decimal? diemChang2 =
                TinhDiemChang(records, clo, 2);

            decimal? diemClo = null;
            decimal? diemQuyDoi = null;
            int? mucDat = null;
            bool duDuLieu = diemChang1.HasValue && diemChang2.HasValue;

            if (diemChang1 is decimal diemChang1Value
                && diemChang2 is decimal diemChang2Value)
            {
                diemClo =
                    Math.Round(
                        diemChang1Value * trongSoChang1
                        + diemChang2Value * trongSoChang2,
                        1);

                mucDat = TinhMucDat(diemClo.Value);
                diemQuyDoi =
                    Math.Round(
                        diemClo.Value * NormalizeWeight(clo.TrongSoHocPhan),
                        2);

                diemHocPhan += diemQuyDoi.Value;
            }
            else
            {
                duDuLieuTatCaClo = false;
            }

            result.KetQuaClo.Add(new KetQuaCloDTO
            {
                MaClo = clo.TenClo,
                TenClo = clo.TenClo,
                MoTaClo = clo.MoTaClo,
                TrongSoHocPhan = NormalizeWeight(clo.TrongSoHocPhan),
                DiemChang1Dvhd = diemChang1Dvhd,
                DiemChang1Gvhd = diemChang1Gvhd,
                DiemChang1 = diemChang1,
                DiemChang2Dvhd = diemChang2Dvhd,
                DiemChang2Gvhd = diemChang2Gvhd,
                DiemChang2 = diemChang2,
                DiemClo = diemClo,
                DiemQuyDoi = diemQuyDoi,
                MucDat = mucDat,
                DuDuLieu = duDuLieu
            });
        }

        if (duDuLieuTatCaClo)
        {
            result.DiemHocPhan =
                Math.Round(diemHocPhan, 1);

            result.MucDatHocPhan =
                TinhMucDat(result.DiemHocPhan.Value);

            result.DuDieuKienHoanThanh =
                result.KetQuaClo.All(item => item.MucDat >= 1)
                && result.MucDatHocPhan >= 1;
        }

        return result;
    }

    private async Task<TieuChiDanhGiaConfig> GetTieuChiForChamDiem(
        ChamDiemThucTapDTO request)
    {
        TieuChiDanhGiaConfig? config =
            await _repository.GetTieuChiBySinhVien(
                request.MSSV,
                request.IdKiThucTap);

        return ValidateTieuChiConfig(config);
    }

    private async Task<TieuChiDanhGiaConfig> GetTieuChiForKiThucTap(
        int idKiThucTap)
    {
        TieuChiDanhGiaConfig? config =
            await _repository.GetTieuChiByKiThucTap(idKiThucTap);

        return ValidateTieuChiConfig(config);
    }

    private static TieuChiDanhGiaConfig ValidateTieuChiConfig(
        TieuChiDanhGiaConfig? config)
    {
        if (config == null || config.Clos.Count == 0)
        {
            throw new ArgumentException("Chưa cấu hình tiêu chí đánh giá cho khoa của kỳ thực tập này.");
        }

        decimal tongChang =
            NormalizeWeight(config.PhanTramChang1)
            + NormalizeWeight(config.PhanTramChang2);

        if (Math.Abs(tongChang - 1m) > 0.01m)
        {
            throw new ArgumentException("Tổng phần trăm chặng 1 và chặng 2 phải bằng 100%.");
        }

        decimal tongHocPhan =
            config.Clos.Sum(item => NormalizeWeight(item.TrongSoHocPhan));

        if (Math.Abs(tongHocPhan - 1m) > 0.01m)
        {
            throw new ArgumentException("Tổng trọng số học phần của các CLO phải bằng 100%.");
        }

        foreach (var clo in config.Clos)
        {
            decimal tongNguoiCham =
                NormalizeWeight(clo.TrongSoDonViHuongDan)
                + NormalizeWeight(clo.TrongSoGiangVienHuongDan);

            if (Math.Abs(tongNguoiCham - 1m) > 0.01m)
            {
                throw new ArgumentException($"Tổng trọng số ĐVHD và GVHD của {clo.TenClo} phải bằng 100%.");
            }
        }

        return config;
    }

    private static decimal? TinhDiemChang(
        List<DanhGiaCloRecord> records,
        TieuChiCloConfig clo,
        int chang)
    {
        decimal trongSoDvhd =
            NormalizeWeight(clo.TrongSoDonViHuongDan);

        decimal trongSoGvhd =
            NormalizeWeight(clo.TrongSoGiangVienHuongDan);

        decimal? dvhd =
            records
                .Where(item =>
                    item.MaClo.Equals(clo.TenClo, StringComparison.OrdinalIgnoreCase)
                    && item.Chang == chang
                    && item.NguoiChamLoai.Equals("DVHD", StringComparison.OrdinalIgnoreCase))
                .Select(item => item.Diem)
                .Cast<decimal?>()
                .FirstOrDefault();

        decimal? gvhd =
            records
                .Where(item =>
                    item.MaClo.Equals(clo.TenClo, StringComparison.OrdinalIgnoreCase)
                    && item.Chang == chang
                    && item.NguoiChamLoai.Equals("GVHD", StringComparison.OrdinalIgnoreCase))
                .Select(item => item.Diem)
                .Cast<decimal?>()
                .FirstOrDefault();

        if (trongSoDvhd > 0 && !dvhd.HasValue)
        {
            return null;
        }

        if (trongSoGvhd > 0 && !gvhd.HasValue)
        {
            return null;
        }

        decimal score =
            (dvhd ?? 0m) * trongSoDvhd
            + (gvhd ?? 0m) * trongSoGvhd;

        return Math.Round(score, 1);
    }

    private static decimal? GetDiemNguoiCham(
        List<DanhGiaCloRecord> records,
        string maClo,
        int chang,
        string nguoiChamLoai)
    {
        return records
            .Where(item =>
                item.MaClo.Equals(maClo, StringComparison.OrdinalIgnoreCase)
                && item.Chang == chang
                && item.NguoiChamLoai.Equals(nguoiChamLoai, StringComparison.OrdinalIgnoreCase))
            .Select(item => item.Diem)
            .Cast<decimal?>()
            .FirstOrDefault();
    }

    private static decimal NormalizeWeight(decimal value)
    {
        return value > 1m
            ? value / 100m
            : value;
    }

    private static int TinhMucDat(decimal diem)
    {
        if (diem < 4m)
        {
            return 0;
        }

        if (diem < 5.5m)
        {
            return 1;
        }

        if (diem < 7m)
        {
            return 2;
        }

        if (diem < 8.5m)
        {
            return 3;
        }

        return 4;
    }

    private static void ValidateChamDiemCoBan(ChamDiemThucTapDTO request)
    {
        if (request.Chang is not 1 and not 2)
        {
            throw new ArgumentException("Chặng đánh giá chỉ được là 1 hoặc 2.");
        }

        string nguoiChamLoai =
            request.NguoiChamLoai.ToUpperInvariant();

        if (nguoiChamLoai is not "GVHD" and not "DVHD")
        {
            throw new ArgumentException("Loại người chấm chỉ được là GVHD hoặc DVHD.");
        }

        if (string.IsNullOrWhiteSpace(request.MSSV))
        {
            throw new ArgumentException("Thiếu MSSV.");
        }

        if (string.IsNullOrWhiteSpace(request.NguoiChamId))
        {
            throw new ArgumentException("Thiếu mã người chấm.");
        }

        if (request.DiemClo.Count == 0)
        {
            throw new ArgumentException("Thiếu danh sách điểm CLO.");
        }
    }
}
