public interface IAdminRepository
{
    Task<Admin_Model?> GetAdminByTaiKhoan(string taiKhoan);
}
