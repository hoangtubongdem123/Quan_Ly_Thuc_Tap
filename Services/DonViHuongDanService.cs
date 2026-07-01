public class DonViHuongDanService : IDonViHuongDanService
{
    private readonly IDonViHuongDanRepository _DonViHuongDanRepository;

    public DonViHuongDanService(IDonViHuongDanRepository DonViHuongDanRepository)
    {
        _DonViHuongDanRepository = DonViHuongDanRepository;
    }
    public async Task<int> AddListDonViHuongDan(List<AddDonViHuongDanDTO> listdvhd)
    {
        
        int idDVHD = await _DonViHuongDanRepository.AddListDonViHuongDan(listdvhd);

        
        return idDVHD;
    }

    public async Task<List<Don_Vi_HD_Model>> GetDonViHuongDanByKi(int idKiThucTap)
    {
        if (idKiThucTap <= 0)
        {
            throw new ArgumentException("Kỳ thực tập không hợp lệ.");
        }

        return await _DonViHuongDanRepository.GetDonViHuongDanByKi(idKiThucTap);
    }

    public async Task<int> AddDonViHuongDan(AddDonViHuongDanDTO request)
    {
        ValidateDonViHuongDan(request.IdKiThucTap, request.TenDonViHD);

        return await _DonViHuongDanRepository.AddDonViHuongDan(request);
    }

    public async Task<int> UpdateDonViHuongDan(int idDonViHD, UpdateDonViHuongDanDTO request)
    {
        if (idDonViHD <= 0)
        {
            throw new ArgumentException("ĐVHD không hợp lệ.");
        }

        ValidateDonViHuongDan(1, request.TenDonViHD);

        return await _DonViHuongDanRepository.UpdateDonViHuongDan(
            idDonViHD,
            request);
    }

    public async Task<int> DeleteDonViHuongDan(int idDonViHD)
    {
        if (idDonViHD <= 0)
        {
            throw new ArgumentException("ĐVHD không hợp lệ.");
        }

        return await _DonViHuongDanRepository.DeleteDonViHuongDan(idDonViHD);
    }

    public async Task<Don_Vi_HD_Model?> GetDonViHuongDanByGmail(string gmail)
    {
        return await _DonViHuongDanRepository.GetDonViHuongDanByGmail(gmail);
    }

    public async Task<List<SinhVienPhuTrachDonViHuongDanDTO>> GetSinhVienPhuTrach(
        int idDonViHD)
    {
        if (idDonViHD <= 0)
        {
            throw new ArgumentException("ID DVHD khong hop le.");
        }

        return await _DonViHuongDanRepository.GetSinhVienPhuTrach(idDonViHD);
    }

    private static void ValidateDonViHuongDan(int idKiThucTap, string tenDonViHD)
    {
        if (idKiThucTap <= 0)
        {
            throw new ArgumentException("Kỳ thực tập không hợp lệ.");
        }

        if (string.IsNullOrWhiteSpace(tenDonViHD))
        {
            throw new ArgumentException("Tên ĐVHD không được để trống.");
        }
    }
}    
    
    
    
    
