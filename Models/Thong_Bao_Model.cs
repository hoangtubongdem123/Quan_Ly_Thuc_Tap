public class Thong_Bao_Model
{
    public int IdThongBao { get; set; }

    public string LoaiNguoiNhan { get; set; } = string.Empty;

    public string MaNguoiNhan { get; set; } = string.Empty;

    public string TieuDe { get; set; } = string.Empty;

    public string NoiDung { get; set; } = string.Empty;

    public DateTime NgayTao { get; set; }
}