public interface ISinhVienRepository
{
    Task<List<Sinh_Vien_Model>> GetAll();


    Task<Sinh_Vien_Model?> GetSinhVien(string mssv);

    Task<Sinh_Vien_Model?> GetSinhVienByGmail(string gmail);
    Task<int> AddSinhVien(Sinh_Vien_Model request);
    Task<int> UpdateSinhVien(string mssv, Sinh_Vien_Model request);
    Task<int> DeleteSinhVien(string mssv);

    Task<List<SinhVienKiThucTapDTO>> GetKiThucTapByMssv(string mssv);

    Task<List<Thong_Bao_Model>> GetThongBaoByMssv(string mssv);

    Task<int> GetIdKhoaByMSSV( string mssv);
    Task<int> AddSinhVienThucTap(List<AddSinhVienThucTapDTO> listSV) ;
    Task<bool> SinhVienThucTapExists(string mssv, int idKiThucTap);
    Task<int> PhanCongGiangVien(string mssv, int idKiThucTap, string maSoGiangVien);
    Task<int> DeleteSinhVienThucTap(string mssv, int idKiThucTap);

    Task<int> GetIdKiThucTapByMSSV(string mssv);


}
