public class SinhVienPhuTrachGiangVienDTO
{
    public string MSSV { get; set; } = string.Empty;

    public string TenSinhVien { get; set; } = string.Empty;

    public int IdKiThucTap { get; set; }

    public string TenKiThucTap { get; set; } = string.Empty;

    public DateTime? TimeBatDau { get; set; }

    public DateTime? TimeKetThuc { get; set; }

    public int? IdDonViHD { get; set; }

    public string TenDonViHD { get; set; } = string.Empty;

    public string MaSoGiangVien { get; set; } = string.Empty;

    public string TenGiangVien { get; set; } = string.Empty;

    public string TrangThai { get; set; } = string.Empty;
}
