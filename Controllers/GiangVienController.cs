using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class GiangVienController : ControllerBase
{
    private readonly IGiangVienService _GiangVienService;

    public GiangVienController( IGiangVienService service)
    {
        _GiangVienService = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var gvs =
            await _GiangVienService.GetAll();

        return Ok(gvs);
    }

    [HttpGet("SoSinhVienPhuTrach")]
    public async Task<IActionResult> CountSinhVienDangHuongDanByGiangVien()
    {
        var result =
            await _GiangVienService.CountSinhVienDangHuongDanByGiangVien();

        return Ok(result);
    }

    [HttpGet("Me/SinhVienPhuTrach")]
    public async Task<IActionResult> GetMySinhVienPhuTrach()
    {
        string? role =
            HttpContext.Session.GetString("role");

        string? maSoGiangVien =
            HttpContext.Session.GetString("maSoGiangVien");

        if (string.IsNullOrWhiteSpace(maSoGiangVien)
            || !string.Equals(role, "GiangVien", StringComparison.OrdinalIgnoreCase))
        {
            return Unauthorized(new
            {
                message = "Chua dang nhap giang vien"
            });
        }

        try
        {
            var result =
                await _GiangVienService.GetSinhVienPhuTrach(maSoGiangVien);

            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new
            {
                message = ex.Message
            });
        }
    }

    [HttpGet("{msgv}")]
    public async Task<IActionResult> GetGiangVien(string msgv)
    {
        var gv = await _GiangVienService.GetGiangVien(msgv);

        if (gv == null)
        {
            return NotFound();
        }

        return Ok(gv);
    }

    [HttpPost]
    public async Task<IActionResult> AddGiangVien(
        [FromBody] Giang_Vien_Model request)
    {
        try
        {
            await _GiangVienService.AddGiangVien(request);

            return Ok(new
            {
                message = "Them giang vien thanh cong."
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

    [HttpPut("{msgv}")]
    public async Task<IActionResult> UpdateGiangVien(
        string msgv,
        [FromBody] Giang_Vien_Model request)
    {
        try
        {
            int affectedRows =
                await _GiangVienService.UpdateGiangVien(msgv, request);

            if (affectedRows == 0)
            {
                return NotFound(new
                {
                    message = "Khong tim thay giang vien."
                });
            }

            return Ok(new
            {
                message = "Cap nhat giang vien thanh cong."
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

    [HttpDelete("{msgv}")]
    public async Task<IActionResult> DeleteGiangVien(string msgv)
    {
        try
        {
            int affectedRows =
                await _GiangVienService.DeleteGiangVien(msgv);

            if (affectedRows == 0)
            {
                return NotFound(new
                {
                    message = "Khong tim thay giang vien."
                });
            }

            return Ok(new
            {
                message = "Xoa giang vien thanh cong."
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


    



}
