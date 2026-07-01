using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class MinhChungController : ControllerBase
{
    private readonly IMinhChungService _service;

    public MinhChungController(IMinhChungService service)
    {
        _service = service;
    }

    [HttpPost]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> AddMinhChung(
        [FromForm] AddMinhChungDTO request)
    {
        try
        {
            var result =
                await _service.AddMinhChung(request);

            return Ok(new
            {
                message = "Them minh chung thanh cong",
                data = result
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

    [HttpGet("{mssv}")]
    public async Task<IActionResult> GetMinhChungByMssv(string mssv)
    {
        try
        {
            var result =
                await _service.GetMinhChungByMssv(mssv);

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

    [HttpPut("{idMinhChung}")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UpdateMinhChung(
        int idMinhChung,
        [FromForm] UpdateMinhChungDTO request)
    {
        try
        {
            var result =
                await _service.UpdateMinhChung(
                    idMinhChung,
                    request);

            return Ok(new
            {
                message = "Cap nhat minh chung thanh cong",
                data = result
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

    [HttpDelete("{idMinhChung}")]
    public async Task<IActionResult> DeleteMinhChung(int idMinhChung)
    {
        try
        {
            int rows =
                await _service.DeleteMinhChung(idMinhChung);

            return Ok(new
            {
                message = "Xoa minh chung thanh cong",
                so_dong = rows
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
