using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class DanhGiaThucTapController : ControllerBase
{
    private readonly IDanhGiaThucTapService _service;

    public DanhGiaThucTapController(
        IDanhGiaThucTapService service)
    {
        _service = service;
    }

    [HttpPost("ChamDiem")]
    public async Task<IActionResult> ChamDiem(
        [FromBody] ChamDiemThucTapDTO request)
    {
        try
        {
            int soDong =
                await _service.ChamDiem(request);

            return Ok(new
            {
                message = "Chấm điểm thành công",
                so_dong = soDong
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

    [HttpPut("Khoa/CapNhatDiem")]
    public async Task<IActionResult> CapNhatDiemTuKhoa(
        [FromBody] ChamDiemThucTapDTO request)
    {
        try
        {
            int soDong =
                await _service.CapNhatDiemTuKhoa(request);

            return Ok(new
            {
                message = "Cap nhat diem thanh cong",
                so_dong = soDong
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

    [HttpGet("KetQua/{idKiThucTap}/{mssv}")]
    public async Task<IActionResult> KetQua(
        int idKiThucTap,
        string mssv)
    {
        try
        {
            var result =
                await _service.GetKetQua(
                    mssv,
                    idKiThucTap);

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

    [HttpGet("TieuChi/{idKiThucTap}")]
    public async Task<IActionResult> TieuChi(
        int idKiThucTap)
    {
        try
        {
            var result =
                await _service.GetTieuChi(idKiThucTap);

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
