public interface IKhoaService
{
    Task<List<Khoa_Model>> GetAll() ;
    Task<Khoa_Model?> GetKhoa(string id_khoa);
    Task<Khoa_Model?> GetKhoaByGmail(string gmail);
    Task<int> AddKhoa(Khoa_Model request);
    Task<int> UpdateKhoa(int idKhoa, Khoa_Model request);
    Task<int> DeleteKhoa(int idKhoa);
    Task<int> AddThucTap(KiThucTapDTO request) ;
    Task<List<Ki_Thuc_Tap_Model>> GetKiThucTapByKhoa(int idKhoa);
    Task<int> UpdateKiThucTap(int idKiThucTap, UpdateKiThucTapDTO request);
    Task<int> DeleteKiThucTap(int idKiThucTap);
    Task<List<SinhVienThucTapKhoaDTO>> GetSinhVienThucTapByKi(int idKiThucTap);
    Task<List<TieuChiDanhGiaConfig>> GetTieuChiByKhoa(int idKhoa);
    Task<int> AddTieuChi(TieuChiDanhGiaDTO request);
    Task<int> UpdateTieuChi(int idTieuChi, TieuChiDanhGiaDTO request);
    Task<int> DeleteTieuChi(int idTieuChi);
    Task<ImportSinhVienThucTapResultDTO> PreviewImportSinhVienThucTap(IFormFile file, int idKiThucTap);
    Task<ImportSinhVienThucTapResultDTO> ImportSinhVienThucTap(IFormFile file, int idKiThucTap);
    Task<string> PhanCongGiangVien(PhanCongGiangVienDTO request);
    Task<int> DeleteSinhVienThucTap(string mssv, int idKiThucTap);


}
