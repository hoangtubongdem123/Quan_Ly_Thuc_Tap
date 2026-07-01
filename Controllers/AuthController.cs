using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IKhoaService _khoaService;
    private readonly ISinhVienService _sinhvienService;
    private readonly IGiangVienService _giangVienService;
    private readonly IDonViHuongDanService _donViHuongDanService;
    private readonly IAdminService _adminService;

    public AuthController(
        IKhoaService khoaService,
        ISinhVienService sinhvienService,
        IGiangVienService giangVienService,
        IDonViHuongDanService donViHuongDanService,
        IAdminService adminService)
    {
        _khoaService = khoaService;
        _sinhvienService = sinhvienService;
        _giangVienService = giangVienService;
        _donViHuongDanService = donViHuongDanService;
        _adminService = adminService;
    }

    [HttpPost("login-khoa")]
    public async Task<IActionResult> KhoaLogin(
        [FromBody] Khoa_Model model)
    {
        var khoa =
            await _khoaService.GetKhoaByGmail(
                model.GmailKhoa
            );

        if (khoa == null)
        {
            return Unauthorized(new
            {
                message = "Tài khoản không tồn tại"
            });
        }

        if (model.PasswordKhoa == khoa.PasswordKhoa)
        {
            HttpContext.Session.SetString(
                "gmail",
                khoa.GmailKhoa
            );

            HttpContext.Session.SetString(
                "role",
                "Khoa"
            );

            return Ok(new
            {
                message = "Đăng nhập thành công"
            });
        }

        return Unauthorized(new
        {
            message = "Sai mật khẩu"
        });
    }

    [HttpPost("login-sinhvien")]
    public async Task<IActionResult> SinhVienLogin(
        [FromBody] LoginSinhVienDTO model)
    {
        var sinhvien =
            await _sinhvienService.GetSinhVienByGmail(
                model.GmailSinhVien
            );

        if (sinhvien == null)
        {
            return Unauthorized(new
            {
                message = "Tài khoản không tồn tại"
            });
        }

        if (model.PasswordSinhVien
            == sinhvien.PasswordSinhVien)
        {
            HttpContext.Session.SetString(
                "gmail",
                sinhvien.GmailSinhVien
            );

            HttpContext.Session.SetString(
                "role",
                "SinhVien"
            );

            return Ok(new
            {
                message = "Đăng nhập thành công"
            });
        }

        return Unauthorized(new
        {
            message = "Sai mật khẩu"
        });
    }

    [HttpPost("login-giangvien")]
    public async Task<IActionResult> GiangVienLogin(
        [FromBody] LoginGiangVienDTO model)
    {
        var giangVien =
            await _giangVienService.GetGiangVienByGmail(
                model.GmailGiangVien
            );

        if (giangVien == null)
        {
            return Unauthorized(new
            {
                message = "TÃ i khoáº£n khÃ´ng tá»“n táº¡i"
            });
        }

        if (model.PasswordGiangVien
            == giangVien.PasswordGiangVien)
        {
            HttpContext.Session.SetString(
                "gmail",
                giangVien.GmailGiangVien
            );

            HttpContext.Session.SetString(
                "role",
                "GiangVien"
            );

            HttpContext.Session.SetString(
                "maSoGiangVien",
                giangVien.MaSoGiangVien
            );

            return Ok(new
            {
                message = "ÄÄƒng nháº­p thÃ nh cÃ´ng",
                maSoGiangVien = giangVien.MaSoGiangVien,
                tenGiangVien = giangVien.TenGiangVien
            });
        }

        return Unauthorized(new
        {
            message = "Sai máº­t kháº©u"
        });
    }

    [HttpPost("login-dvhd")]
    public async Task<IActionResult> DonViHuongDanLogin(
        [FromBody] LoginDonViHuongDanDTO model)
    {
        var donViHuongDan =
            await _donViHuongDanService.GetDonViHuongDanByGmail(
                model.GmailDonViHD
            );

        if (donViHuongDan == null)
        {
            return Unauthorized(new
            {
                message = "TÃ i khoáº£n khÃ´ng tá»“n táº¡i"
            });
        }

        if (model.PasswordDonViHD
            == donViHuongDan.PasswordDonViHD)
        {
            HttpContext.Session.SetString(
                "gmail",
                donViHuongDan.GmailDonViHD
            );

            HttpContext.Session.SetString(
                "role",
                "DVHD"
            );

            HttpContext.Session.SetInt32(
                "idDonViHD",
                donViHuongDan.IdDonViHD
            );

            return Ok(new
            {
                message = "ÄÄƒng nháº­p thÃ nh cÃ´ng",
                idDonViHD = donViHuongDan.IdDonViHD,
                tenDonViHD = donViHuongDan.TenDonViHD,
                idKiThucTap = donViHuongDan.IdKiThucTap
            });
        }

        return Unauthorized(new
        {
            message = "Sai máº­t kháº©u"
        });
    }

    [HttpPost("login-admin")]
    public async Task<IActionResult> AdminLogin(
        [FromBody] LoginAdminDTO model)
    {
        var admin =
            await _adminService.GetAdminByTaiKhoan(
                model.TaiKhoanAdmin
            );

        if (admin == null)
        {
            return Unauthorized(new
            {
                message = "TÃ i khoáº£n khÃ´ng tá»“n táº¡i"
            });
        }

        if (model.PasswordAdmin == admin.PasswordAdmin)
        {
            HttpContext.Session.SetString(
                "taikhoan",
                admin.TaiKhoanAdmin
            );

            HttpContext.Session.SetString(
                "role",
                "Admin"
            );

            HttpContext.Session.SetInt32(
                "idAdmin",
                admin.IdAdmin
            );

            return Ok(new
            {
                message = "ÄÄƒng nháº­p thÃ nh cÃ´ng",
                idAdmin = admin.IdAdmin,
                taiKhoanAdmin = admin.TaiKhoanAdmin
            });
        }

        return Unauthorized(new
        {
            message = "Sai máº­t kháº©u"
        });
    }

    [HttpGet("profile")]
    public IActionResult Profile()
    {
        var gmail = HttpContext.Session.GetString("gmail");
        var role = HttpContext.Session.GetString("role");

        if (gmail == null)
        {
            return Unauthorized("Chưa đăng nhập");
        }

        return Ok(new
        {
            gmail,
            role
        });
    }

    [HttpPost("logout")]
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();

        return Ok("Đã đăng xuất");
    }
}
