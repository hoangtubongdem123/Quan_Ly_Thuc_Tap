public interface IDanhGiaThucTapRepository
{
    Task<int> SaveDanhGia(ChamDiemThucTapDTO request);

    Task<int> UpdateDanhGiaByLoaiNguoiCham(ChamDiemThucTapDTO request);

    Task<List<DanhGiaCloRecord>> GetDanhGiaClo(string mssv, int idKiThucTap);

    Task<TieuChiDanhGiaConfig?> GetTieuChiByKiThucTap(int idKiThucTap);

    Task<TieuChiDanhGiaConfig?> GetTieuChiBySinhVien(string mssv, int? idKiThucTap);
}

public class DanhGiaCloRecord
{
    public int Chang { get; set; }

    public string NguoiChamLoai { get; set; } = string.Empty;

    public string MaClo { get; set; } = string.Empty;

    public decimal Diem { get; set; }
}

public class TieuChiDanhGiaConfig
{
    public int IdTieuChi { get; set; }

    public string TenTieuChi { get; set; } = string.Empty;

    public int IdKhoa { get; set; }

    public decimal PhanTramChang1 { get; set; }

    public decimal PhanTramChang2 { get; set; }

    public List<TieuChiCloConfig> Clos { get; set; } = new();
}

public class TieuChiCloConfig
{
    public int IdClo { get; set; }

    public string TenClo { get; set; } = string.Empty;

    public string MoTaClo { get; set; } = string.Empty;

    public decimal TrongSoHocPhan { get; set; }

    public decimal TrongSoDonViHuongDan { get; set; }

    public decimal TrongSoGiangVienHuongDan { get; set; }
}
