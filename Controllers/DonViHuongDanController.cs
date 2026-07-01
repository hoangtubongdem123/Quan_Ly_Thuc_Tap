using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class DonViHuongDanController : ControllerBase
{
    private readonly IDonViHuongDanService _service;

    public DonViHuongDanController(IDonViHuongDanService service)
    {
        _service = service;
    }

    [HttpGet("Me/SinhVienPhuTrach")]
    public async Task<IActionResult> GetMySinhVienPhuTrach()
    {
        string? role =
            HttpContext.Session.GetString("role");

        int? idDonViHD =
            HttpContext.Session.GetInt32("idDonViHD");

        if (!idDonViHD.HasValue
            || !string.Equals(role, "DVHD", StringComparison.OrdinalIgnoreCase))
        {
            return Unauthorized(new
            {
                message = "Chua dang nhap don vi huong dan"
            });
        }

        try
        {
            var result =
                await _service.GetSinhVienPhuTrach(idDonViHD.Value);

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
}
