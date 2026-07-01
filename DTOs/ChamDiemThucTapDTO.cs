public class ChamDiemThucTapDTO
{
    public string MSSV { get; set; } = string.Empty;

    public int? IdKiThucTap { get; set; }

    public int Chang { get; set; }

    public string NguoiChamLoai { get; set; } = string.Empty;

    public string NguoiChamId { get; set; } = string.Empty;

    public List<DiemCloDTO> DiemClo { get; set; } = new();
}

public class DiemCloDTO
{
    public string MaClo { get; set; } = string.Empty;

    public decimal Diem { get; set; }
}
