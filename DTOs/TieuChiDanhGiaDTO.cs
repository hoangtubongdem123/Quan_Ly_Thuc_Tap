public class TieuChiDanhGiaDTO
{
    public string TenTieuChi { get; set; } = string.Empty;

    public int IdKhoa { get; set; }

    public decimal PhanTramChang1 { get; set; } = 30m;

    public decimal PhanTramChang2 { get; set; } = 70m;

    public List<TieuChiCloDTO> Clos { get; set; } = new();
}

public class TieuChiCloDTO
{
    public string TenClo { get; set; } = string.Empty;

    public string MoTaClo { get; set; } = string.Empty;

    public decimal TrongSoHp { get; set; }

    public decimal TrongSoDvhd { get; set; }

    public decimal TrongSoGvhd { get; set; }
}
