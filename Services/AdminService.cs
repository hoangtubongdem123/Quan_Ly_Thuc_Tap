public class AdminService : IAdminService
{
    private readonly IAdminRepository _adminRepository;

    public AdminService(IAdminRepository adminRepository)
    {
        _adminRepository = adminRepository;
    }

    public async Task<Admin_Model?> GetAdminByTaiKhoan(string taiKhoan)
    {
        return await _adminRepository.GetAdminByTaiKhoan(taiKhoan);
    }
}
