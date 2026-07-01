public interface IKhoaRepository
{
    Task<List<Khoa_Model>> GetAll() ;
    Task<Khoa_Model?> GetKhoa(string id_khoa);
    Task<Khoa_Model?> GetKhoaByGmail(string gmail);
    Task<int> AddKhoa(Khoa_Model request);
    Task<int> UpdateKhoa(int idKhoa, Khoa_Model request);
    Task<int> DeleteKhoa(int idKhoa);
    Task<string?> GetMSSVKiThucTapByDONDKI(int id_don_dki) ;
    Task<int> AddThucTap(KiThucTapDTO request) ;
    Task<bool> KiThucTapExists(int idKiThucTap);
    Task<List<Ki_Thuc_Tap_Model>> GetKiThucTapByKhoa(int idKhoa);
    Task<int> UpdateKiThucTap(int idKiThucTap, UpdateKiThucTapDTO request);
    Task<int> DeleteKiThucTap(int idKiThucTap);
    Task<List<SinhVienThucTapKhoaDTO>> GetSinhVienThucTapByKi(int idKiThucTap);
    Task<List<TieuChiDanhGiaConfig>> GetTieuChiByKhoa(int idKhoa);
    Task<int> AddTieuChi(TieuChiDanhGiaDTO request);
    Task<int> UpdateTieuChi(int idTieuChi, TieuChiDanhGiaDTO request);
    Task<int> DeleteTieuChi(int idTieuChi);


}
