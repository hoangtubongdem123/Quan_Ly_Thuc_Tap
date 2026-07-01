public interface IGiangVienService
{
Task<List<Giang_Vien_Model>> GetAll();
Task<Giang_Vien_Model?> GetGiangVien(string msgv);
Task<Giang_Vien_Model?> GetGiangVienByGmail(string gmail);
Task<int> AddGiangVien(Giang_Vien_Model request);
Task<int> UpdateGiangVien(string maSoGiangVien, Giang_Vien_Model request);
Task<int> DeleteGiangVien(string maSoGiangVien);
Task<List<GiangVienPhuTrachCountDTO>> CountSinhVienDangHuongDanByGiangVien();
Task<List<SinhVienPhuTrachGiangVienDTO>> GetSinhVienPhuTrach(string maSoGiangVien);


}
