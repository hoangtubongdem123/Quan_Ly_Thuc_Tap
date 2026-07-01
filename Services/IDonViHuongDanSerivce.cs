public interface IDonViHuongDanService
{
    Task<int> AddListDonViHuongDan(List<AddDonViHuongDanDTO> listDV);
    Task<List<Don_Vi_HD_Model>> GetDonViHuongDanByKi(int idKiThucTap);
    Task<int> AddDonViHuongDan(AddDonViHuongDanDTO request);
    Task<int> UpdateDonViHuongDan(int idDonViHD, UpdateDonViHuongDanDTO request);
    Task<int> DeleteDonViHuongDan(int idDonViHD);
    Task<Don_Vi_HD_Model?> GetDonViHuongDanByGmail(string gmail);
    Task<List<SinhVienPhuTrachDonViHuongDanDTO>> GetSinhVienPhuTrach(int idDonViHD);


}
