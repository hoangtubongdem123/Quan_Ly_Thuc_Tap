public interface IDanhGiaThucTapService
{
    Task<int> ChamDiem(ChamDiemThucTapDTO request);

    Task<int> CapNhatDiemTuKhoa(ChamDiemThucTapDTO request);

    Task<TieuChiDanhGiaConfig> GetTieuChi(int idKiThucTap);

    Task<KetQuaDanhGiaThucTapDTO> GetKetQua(string mssv, int idKiThucTap);
}
