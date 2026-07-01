public interface IAdminService
{
    Task<Admin_Model?> GetAdminByTaiKhoan(string taiKhoan);
}
