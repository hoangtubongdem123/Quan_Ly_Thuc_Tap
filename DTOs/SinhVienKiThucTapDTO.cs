public class SinhVienKiThucTapDTO
{
    public string MSSV { get; set; } = string.Empty;

    public string TenSinhVien { get; set; } = string.Empty;

    public int IdKiThucTap { get; set; }

    public string TenKiThucTap { get; set; } = string.Empty;

    public DateTime? TimeBatDau { get; set; }

    public DateTime? TimeKetThuc { get; set; }

    public string TrangThaiKiThucTap { get; set; } = string.Empty;

    public int? IdDonViHD { get; set; }

    public string TenDonViHD { get; set; } = string.Empty;

    public string MaSoGiangVien { get; set; } = string.Empty;

    public string TenGiangVien { get; set; } = string.Empty;

    public string TrangThaiPhanCong { get; set; } = string.Empty;
}
