public class ImportSinhVienThucTapResultDTO
{
    public int TongSoDong { get; set; }

    public int SoDongThanhCong { get; set; }

    public int SoDongLoi { get; set; }

    public List<ImportSinhVienThucTapRowResultDTO> KetQua { get; set; } = new();
}

public class ImportSinhVienThucTapRowResultDTO
{
    public int Dong { get; set; }

    public string MSSV { get; set; } = string.Empty;

    public string HoTen { get; set; } = string.Empty;

    public string Lop { get; set; } = string.Empty;

    public string TenDonViHD { get; set; } = string.Empty;

    public bool ThanhCong { get; set; }

    public string ThongBao { get; set; } = string.Empty;
}
