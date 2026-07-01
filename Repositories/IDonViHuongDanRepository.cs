public interface IDonViHuongDanRepository
{
    Task<int> AddListDonViHuongDan(List<AddDonViHuongDanDTO> listDV);
    Task<List<Don_Vi_HD_Model>> GetDonViHuongDanByKi(int idKiThucTap);
    Task<int> AddDonViHuongDan(AddDonViHuongDanDTO request);
    Task<int> UpdateDonViHuongDan(int idDonViHD, UpdateDonViHuongDanDTO request);
    Task<int> DeleteDonViHuongDan(int idDonViHD);


    Task<int> GetIDDonViHuongDanByName(string NameDVHD,int id_ki_thuc_tap);
    Task<int> GetOrCreateDonViHuongDan(string tenDonViHD, int idKiThucTap);
    Task<int> TaoPasswordDonViHD(int idDonViHD,string password);
    Task<Don_Vi_HD_Model?> GetDonViHuongDanByGmail(string gmail);
    Task<List<SinhVienPhuTrachDonViHuongDanDTO>> GetSinhVienPhuTrach(int idDonViHD);

}
