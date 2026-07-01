public interface ISinhVienService
{
    Task<List<Sinh_Vien_Model>> GetAll();
    Task<Sinh_Vien_Model?> GetSinhVien(string mssv);


    Task<Sinh_Vien_Model?> GetSinhVienByGmail(string gmail);
    Task<int> AddSinhVien(Sinh_Vien_Model request);
    Task<int> UpdateSinhVien(string mssv, Sinh_Vien_Model request);
    Task<int> DeleteSinhVien(string mssv);

    Task<List<SinhVienKiThucTapDTO>> GetKiThucTapByMssv(string mssv);

    Task<List<Thong_Bao_Model>> GetThongBaoByMssv(string mssv);

    
    Task<int> AddSinhVienThuc_Tap(List<AddSinhVienThucTapDTO> listsvthuctap);



}
