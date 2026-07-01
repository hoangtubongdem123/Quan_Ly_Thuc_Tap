public class KiThucTapDTO
{
    public string TenKiThucTap { get; set; } = string.Empty;

    public DateTime? TimeBatDau { get; set; }

    public DateTime? TimeKetThuc { get; set; }

    public string TrangThai { get; set; } = string.Empty;

    public int IdKhoa { get; set; }

    public int? IdTieuChi { get; set; }
}
