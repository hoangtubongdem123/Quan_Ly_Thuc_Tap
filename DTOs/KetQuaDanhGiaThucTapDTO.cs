public class KetQuaDanhGiaThucTapDTO
{
    public string MSSV { get; set; } = string.Empty;

    public int IdKiThucTap { get; set; }

    public decimal? DiemHocPhan { get; set; }

    public int? MucDatHocPhan { get; set; }

    public bool DuDieuKienHoanThanh { get; set; }

    public List<KetQuaCloDTO> KetQuaClo { get; set; } = new();
}

public class KetQuaCloDTO
{
    public string MaClo { get; set; } = string.Empty;

    public string TenClo { get; set; } = string.Empty;

    public string MoTaClo { get; set; } = string.Empty;

    public decimal TrongSoHocPhan { get; set; }

    public decimal? DiemChang1Dvhd { get; set; }

    public decimal? DiemChang1Gvhd { get; set; }

    public decimal? DiemChang1 { get; set; }

    public decimal? DiemChang2Dvhd { get; set; }

    public decimal? DiemChang2Gvhd { get; set; }

    public decimal? DiemChang2 { get; set; }

    public decimal? DiemClo { get; set; }

    public decimal? DiemQuyDoi { get; set; }

    public int? MucDat { get; set; }

    public bool DuDuLieu { get; set; }
}
