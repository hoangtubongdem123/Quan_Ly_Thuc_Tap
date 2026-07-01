using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class SinhVienController : ControllerBase
{
    private readonly ISinhVienService _SinhVienService;

    public SinhVienController(ISinhVienService service)
    {
        _SinhVienService = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var students =
            await _SinhVienService.GetAll();

        return Ok(students);
    }


    [HttpGet("{mssv}")]
    public async Task<IActionResult> GetSinhVien(string mssv)
    {
        var student = await _SinhVienService.GetSinhVien(mssv);

        if (student == null)
        {
            return NotFound();
        }

        return Ok(student);
    }

    [HttpPost]
    public async Task<IActionResult> AddSinhVien(
        [FromBody] Sinh_Vien_Model request)
    {
        try
        {
            await _SinhVienService.AddSinhVien(request);

            return Ok(new
            {
                message = "Them sinh vien thanh cong."
            });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new
            {
                message = ex.Message
            });
        }
    }

    [HttpPut("{mssv}")]
    public async Task<IActionResult> UpdateSinhVien(
        string mssv,
        [FromBody] Sinh_Vien_Model request)
    {
        try
        {
            int affectedRows =
                await _SinhVienService.UpdateSinhVien(mssv, request);

            if (affectedRows == 0)
            {
                return NotFound(new
                {
                    message = "Khong tim thay sinh vien."
                });
            }

            return Ok(new
            {
                message = "Cap nhat sinh vien thanh cong."
            });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new
            {
                message = ex.Message
            });
        }
    }

    [HttpDelete("{mssv}")]
    public async Task<IActionResult> DeleteSinhVien(string mssv)
    {
        try
        {
            int affectedRows =
                await _SinhVienService.DeleteSinhVien(mssv);

            if (affectedRows == 0)
            {
                return NotFound(new
                {
                    message = "Khong tim thay sinh vien."
                });
            }

            return Ok(new
            {
                message = "Xoa sinh vien thanh cong."
            });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new
            {
                message = ex.Message
            });
        }
    }

    [HttpGet("Me")]
    public async Task<IActionResult> GetCurrentSinhVien()
    {
        var student = await GetCurrentStudentFromSession();

        if (student == null)
        {
            return Unauthorized(new
            {
                message = "Chưa đăng nhập sinh viên"
            });
        }

        return Ok(student);
    }

    [HttpGet("Me/KiThucTap")]
    public async Task<IActionResult> GetMyKiThucTap()
    {
        var student = await GetCurrentStudentFromSession();

        if (student == null)
        {
            return Unauthorized(new
            {
                message = "Chưa đăng nhập sinh viên"
            });
        }

        var periods =
            await _SinhVienService.GetKiThucTapByMssv(
                student.Mssv);

        return Ok(periods);
    }

    [HttpGet("Me/ThongBao")]
    public async Task<IActionResult> GetMyThongBao()
    {
        var student = await GetCurrentStudentFromSession();

        if (student == null)
        {
            return Unauthorized(new
            {
                message = "Chưa đăng nhập sinh viên"
            });
        }

        var notifications =
            await _SinhVienService.GetThongBaoByMssv(
                student.Mssv);

        return Ok(notifications);
    }

    private async Task<Sinh_Vien_Model?> GetCurrentStudentFromSession()
    {
        string? gmail =
            HttpContext.Session.GetString("gmail");

        string? role =
            HttpContext.Session.GetString("role");

        if (string.IsNullOrWhiteSpace(gmail)
            || !string.Equals(role, "SinhVien", StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }

        return await _SinhVienService.GetSinhVienByGmail(gmail);
    }




    



}
