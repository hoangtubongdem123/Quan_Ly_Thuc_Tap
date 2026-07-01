public class Ki_Thuc_Tap_Model
{
    public int IdKiThucTap { get; set; }

    public string TenKiThucTap { get; set; } = string.Empty;

    public DateTime TimeBatDau { get; set; }

    public DateTime TimeKetThuc { get; set; }

    public int IdKhoa { get; set; }

    public int? IdTieuChi { get; set; }

    public string TrangThai { get; set; } = string.Empty;
}
