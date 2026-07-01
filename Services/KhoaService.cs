using ClosedXML.Excel;

public class KhoaService : IKhoaService
{
    private readonly IConfiguration _configuration;
    private readonly IKhoaRepository _khoaRepository;
    private readonly IGiangVienRepository _giangvienRepository;
    private readonly ISinhVienRepository _sinhvienRepository;
    private readonly IDonViHuongDanRepository _donvihuongdanRepository ;
    public KhoaService(IKhoaRepository khoaRepository,IGiangVienRepository giangvienRepository,ISinhVienRepository sinhvienRepository,IDonViHuongDanRepository donvihuongdanRepository,IConfiguration configuration)
    {
        _khoaRepository = khoaRepository;
        _giangvienRepository = giangvienRepository ;
        _sinhvienRepository = sinhvienRepository ;
        _donvihuongdanRepository = donvihuongdanRepository ;
        _configuration = configuration;
    }

    public async Task<List<Khoa_Model>> GetAll()
    {
        return await _khoaRepository.GetAll();
    }
    public async Task<Khoa_Model?> GetKhoa(string id_khoa)
    {
        return await _khoaRepository.GetKhoa(id_khoa);
    }    

    public async Task<Khoa_Model?> GetKhoaByGmail(string gmail)
    {
        return await _khoaRepository.GetKhoaByGmail(gmail);
    } 

    public async Task<int> AddKhoa(Khoa_Model request)
    {
        ValidateKhoa(request);

        return await _khoaRepository.AddKhoa(request);
    }

    public async Task<int> UpdateKhoa(int idKhoa, Khoa_Model request)
    {
        if (idKhoa <= 0)
        {
            throw new ArgumentException("Ma khoa khong hop le.");
        }

        ValidateKhoa(request);

        return await _khoaRepository.UpdateKhoa(idKhoa, request);
    }

    public async Task<int> DeleteKhoa(int idKhoa)
    {
        if (idKhoa <= 0)
        {
            throw new ArgumentException("Ma khoa khong hop le.");
        }

        return await _khoaRepository.DeleteKhoa(idKhoa);
    }


    public async Task<int> AddThucTap(KiThucTapDTO kithuctap)
    {
        if (string.IsNullOrWhiteSpace(kithuctap.TenKiThucTap))
        {
            throw new ArgumentException("Tên kỳ thực tập không được để trống.");
        }

        if (kithuctap.TimeBatDau.HasValue
            && kithuctap.TimeKetThuc.HasValue
            && kithuctap.TimeKetThuc.Value < kithuctap.TimeBatDau.Value)
        {
            throw new ArgumentException("Ngày kết thúc không được trước ngày bắt đầu.");
        }

        if (kithuctap.IdKhoa <= 0)
        {
            throw new ArgumentException("Thông tin khoa không hợp lệ.");
        }
        
        int idKiThucTap =
            await _khoaRepository.AddThucTap(kithuctap);

        
        return idKiThucTap;
    }

    public async Task<List<Ki_Thuc_Tap_Model>> GetKiThucTapByKhoa(int idKhoa)
    {
        return await _khoaRepository.GetKiThucTapByKhoa(idKhoa);
    }

    public async Task<int> UpdateKiThucTap(
        int idKiThucTap,
        UpdateKiThucTapDTO request)
    {
        if (string.IsNullOrWhiteSpace(request.TenKiThucTap))
        {
            throw new ArgumentException("Tên kì thực tập không được để trống.");
        }

        if (request.TimeKetThuc < request.TimeBatDau)
        {
            throw new ArgumentException("Ngày kết thúc không được trước ngày bắt đầu.");
        }

        if (string.IsNullOrWhiteSpace(request.TrangThai))
        {
            throw new ArgumentException("Trạng thái không được để trống.");
        }

        return await _khoaRepository.UpdateKiThucTap(
            idKiThucTap,
            request);
    }

    public async Task<int> DeleteKiThucTap(int idKiThucTap)
    {
        return await _khoaRepository.DeleteKiThucTap(idKiThucTap);
    }

    public async Task<List<SinhVienThucTapKhoaDTO>> GetSinhVienThucTapByKi(
        int idKiThucTap)
    {
        return await _khoaRepository.GetSinhVienThucTapByKi(idKiThucTap);
    }

    public async Task<List<TieuChiDanhGiaConfig>> GetTieuChiByKhoa(int idKhoa)
    {
        return await _khoaRepository.GetTieuChiByKhoa(idKhoa);
    }

    public async Task<int> AddTieuChi(TieuChiDanhGiaDTO request)
    {
        ValidateTieuChi(request);

        return await _khoaRepository.AddTieuChi(request);
    }

    public async Task<int> UpdateTieuChi(
        int idTieuChi,
        TieuChiDanhGiaDTO request)
    {
        ValidateTieuChi(request);

        return await _khoaRepository.UpdateTieuChi(
            idTieuChi,
            request);
    }

    public async Task<int> DeleteTieuChi(int idTieuChi)
    {
        return await _khoaRepository.DeleteTieuChi(idTieuChi);
    }

    public async Task<ImportSinhVienThucTapResultDTO> PreviewImportSinhVienThucTap(
        IFormFile file,
        int idKiThucTap)
    {
        return await ProcessSinhVienThucTapExcel(
            file,
            idKiThucTap,
            false);
    }

    public async Task<ImportSinhVienThucTapResultDTO> ImportSinhVienThucTap(
        IFormFile file,
        int idKiThucTap)
    {
        return await ProcessSinhVienThucTapExcel(
            file,
            idKiThucTap,
            true);
    }

    private async Task<ImportSinhVienThucTapResultDTO> ProcessSinhVienThucTapExcel(
        IFormFile file,
        int idKiThucTap,
        bool saveToDatabase)
    {
        if (file == null || file.Length == 0)
        {
            throw new ArgumentException("File Excel không hợp lệ.");
        }

        bool kiThucTapTonTai =
            await _khoaRepository.KiThucTapExists(idKiThucTap);

        if (!kiThucTapTonTai)
        {
            throw new ArgumentException("Kỳ thực tập không tồn tại.");
        }

        ImportSinhVienThucTapResultDTO result = new();
        List<AddSinhVienThucTapDTO> sinhVienHopLe = new();

        using var stream = file.OpenReadStream();
        using var workbook = new XLWorkbook(stream);
        var worksheet = workbook.Worksheets.First();
        var lastRow = worksheet.LastRowUsed()?.RowNumber() ?? 1;

        for (int row = 2; row <= lastRow; row++)
        {
            string mssv = worksheet.Cell(row, 1).GetFormattedString().Trim();
            string hoTen = worksheet.Cell(row, 2).GetFormattedString().Trim();
            string lop = worksheet.Cell(row, 3).GetFormattedString().Trim();
            string tenDonViHD = worksheet.Cell(row, 4).GetFormattedString().Trim();

            result.TongSoDong++;

            ImportSinhVienThucTapRowResultDTO rowResult = new()
            {
                Dong = row,
                MSSV = mssv,
                HoTen = hoTen,
                Lop = lop,
                TenDonViHD = tenDonViHD
            };

            if (string.IsNullOrWhiteSpace(mssv))
            {
                rowResult.ThanhCong = false;
                rowResult.ThongBao = "Thiếu MSSV.";
                result.KetQua.Add(rowResult);
                continue;
            }

            if (string.IsNullOrWhiteSpace(tenDonViHD))
            {
                rowResult.ThanhCong = false;
                rowResult.ThongBao = "Thiếu ĐVHD.";
                result.KetQua.Add(rowResult);
                continue;
            }

            var sinhVien =
                await _sinhvienRepository.GetSinhVien(mssv);

            if (sinhVien == null)
            {
                rowResult.ThanhCong = false;
                rowResult.ThongBao = "Sinh viên không tồn tại trong cơ sở dữ liệu.";
                result.KetQua.Add(rowResult);
                continue;
            }

            bool daTonTai =
                await _sinhvienRepository.SinhVienThucTapExists(
                    mssv,
                    idKiThucTap);

            if (daTonTai)
            {
                rowResult.ThanhCong = false;
                rowResult.ThongBao = "Sinh viên đã có trong kỳ thực tập này.";
                result.KetQua.Add(rowResult);
                continue;
            }

            int idDonViHD =
                await _donvihuongdanRepository.GetIDDonViHuongDanByName(
                    tenDonViHD,
                    idKiThucTap);

            if (idDonViHD == 0)
            {
                rowResult.ThanhCong = false;
                rowResult.ThongBao = "Tên ĐVHD không tồn tại trong kỳ thực tập này.";
                result.KetQua.Add(rowResult);
                continue;
            }

            sinhVienHopLe.Add(new AddSinhVienThucTapDTO
            {
                MSSV = mssv,
                IdKiThucTap = idKiThucTap,
                IdDonViHD = idDonViHD
            });

            rowResult.ThanhCong = true;
            rowResult.ThongBao = "Hợp lệ.";
            result.KetQua.Add(rowResult);
        }

        if (saveToDatabase && sinhVienHopLe.Count > 0)
        {
            await _sinhvienRepository.AddSinhVienThucTap(sinhVienHopLe);

            ThucTapSubject subject = new();
            subject.Attach(new ThongBaoSinhVienObserver(_configuration));

            foreach (var sinhVien in sinhVienHopLe)
            {
                await subject.Notify(
                    sinhVien.MSSV,
                    "Thông báo thực tập",
                    "Bạn đã được thêm vào danh sách sinh viên thực tập. Vui lòng chờ Khoa phân công GVHD.");
            }
        }

        result.SoDongThanhCong =
            result.KetQua.Count(item => item.ThanhCong);

        result.SoDongLoi =
            result.KetQua.Count(item => !item.ThanhCong);

        return result;
    }

    private static void ValidateTieuChi(TieuChiDanhGiaDTO request)
    {
        if (string.IsNullOrWhiteSpace(request.TenTieuChi))
        {
            throw new ArgumentException("Tên tiêu chí không được để trống.");
        }

        if (request.IdKhoa <= 0)
        {
            throw new ArgumentException("Thông tin khoa không hợp lệ.");
        }

        if (request.Clos.Count == 0)
        {
            throw new ArgumentException("Cần có ít nhất một CLO.");
        }

        if (Math.Abs(request.PhanTramChang1 + request.PhanTramChang2 - 100m) > 0.01m)
        {
            throw new ArgumentException("Tổng phần trăm chặng 1 và chặng 2 phải bằng 100.");
        }

        decimal tongHocPhan =
            request.Clos.Sum(item => item.TrongSoHp);

        if (Math.Abs(tongHocPhan - 100m) > 0.01m)
        {
            throw new ArgumentException("Tổng trọng số học phần của các CLO phải bằng 100.");
        }

        foreach (var clo in request.Clos)
        {
            if (string.IsNullOrWhiteSpace(clo.TenClo))
            {
                throw new ArgumentException("Tên CLO không được để trống.");
            }

            if (clo.TrongSoHp < 0
                || clo.TrongSoDvhd < 0
                || clo.TrongSoGvhd < 0)
            {
                throw new ArgumentException("Các trọng số không được âm.");
            }

            if (Math.Abs(clo.TrongSoDvhd + clo.TrongSoGvhd - 100m) > 0.01m)
            {
                throw new ArgumentException($"Tổng trọng số ĐVHD và GVHD của {clo.TenClo} phải bằng 100.");
            }
        }
    }

    private static void ValidateKhoa(Khoa_Model request)
    {
        if (string.IsNullOrWhiteSpace(request.TenKhoa))
        {
            throw new ArgumentException("Ten khoa khong duoc de trong.");
        }

        if (string.IsNullOrWhiteSpace(request.GmailKhoa))
        {
            throw new ArgumentException("Gmail khoa khong duoc de trong.");
        }

        if (string.IsNullOrWhiteSpace(request.PasswordKhoa))
        {
            throw new ArgumentException("Mat khau khoa khong duoc de trong.");
        }
    }

    public async Task<string> PhanCongGiangVien(
        PhanCongGiangVienDTO request)
    {
        var giangVien =
            await _giangvienRepository.GetGiangVien(
                request.MaSoGiangVien);

        if (giangVien == null)
        {
            throw new InvalidOperationException("Giảng viên không tồn tại.");
        }

        bool sinhVienThucTapTonTai =
            await _sinhvienRepository.SinhVienThucTapExists(
                request.MSSV,
                request.IdKiThucTap);

        if (!sinhVienThucTapTonTai)
        {
            throw new InvalidOperationException("Sinh viên chưa có trong kỳ thực tập này.");
        }

        int soSinhVienDangHuongDan =
            await _giangvienRepository.CountSinhVienDangHuongDan(
                request.MaSoGiangVien,
                request.IdKiThucTap);

        if (soSinhVienDangHuongDan >= 5)
        {
            throw new InvalidOperationException("Giảng viên đã hướng dẫn đủ 5 sinh viên trong kỳ thực tập này.");
        }

        int affectedRows =
            await _sinhvienRepository.PhanCongGiangVien(
                request.MSSV,
                request.IdKiThucTap,
                request.MaSoGiangVien);

        if (affectedRows == 0)
        {
            throw new InvalidOperationException("Không thể phân công GVHD cho sinh viên.");
        }

        ThucTapSubject subject = new();
        subject.Attach(new ThongBaoSinhVienObserver(_configuration));

        await subject.Notify(
            request.MSSV,
            "Thông báo phân công GVHD",
            $"Bạn đã được phân công GVHD: {giangVien.TenGiangVien}.");

        return "Phân công GVHD thành công.";
    }

    public async Task<int> DeleteSinhVienThucTap(
        string mssv,
        int idKiThucTap)
    {
        if (string.IsNullOrWhiteSpace(mssv))
        {
            throw new ArgumentException("MSSV không được để trống.");
        }

        bool sinhVienThucTapTonTai =
            await _sinhvienRepository.SinhVienThucTapExists(
                mssv,
                idKiThucTap);

        if (!sinhVienThucTapTonTai)
        {
            throw new InvalidOperationException("Sinh viên chưa có trong kỳ thực tập này.");
        }

        return await _sinhvienRepository.DeleteSinhVienThucTap(
            mssv,
            idKiThucTap);
    }


    


}
