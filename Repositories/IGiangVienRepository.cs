public interface IGiangVienRepository
{
Task<List<Giang_Vien_Model>> GetAll();
Task<Giang_Vien_Model?> GetGiangVien(string msgv);
Task<Giang_Vien_Model?> GetGiangVienByGmail(string gmail);
Task<int> AddGiangVien(Giang_Vien_Model request);
Task<int> UpdateGiangVien(string maSoGiangVien, Giang_Vien_Model request);
Task<int> DeleteGiangVien(string maSoGiangVien);
Task<int> AddGiangVienThucTap(Giang_Vien_Thuc_Tap_Model request);
Task<int> CountSinhVienDangHuongDan(string maSoGiangVien, int idKiThucTap);
Task<List<GiangVienPhuTrachCountDTO>> CountSinhVienDangHuongDanByGiangVien();
Task<List<SinhVienPhuTrachGiangVienDTO>> GetSinhVienPhuTrach(string maSoGiangVien);


}
