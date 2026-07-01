using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class KhoaController : ControllerBase
{
    private readonly IKhoaService _khoaService;
    private readonly ISinhVienService _sinhvienService;
    private readonly IDonViHuongDanService _donvihuongdanService;

    public KhoaController(
        IKhoaService khoaService,
        ISinhVienService sinhvienService,
        IDonViHuongDanService donvihuongdanService)
    {
        _khoaService = khoaService;
        _sinhvienService = sinhvienService;
        _donvihuongdanService = donvihuongdanService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var khoa =
            await _khoaService.GetAll();

        return Ok(khoa);
    }

    [HttpGet("{id_khoa}")]
    public async Task<IActionResult> GetKhoa(string id_khoa)
    {
        var khoa = await _khoaService.GetKhoa(id_khoa);

        if (khoa == null)
        {
            return NotFound(new
            {
                message = "Không tìm thấy khoa"
            });
        }

        return Ok(khoa);
    }

    [HttpPost]
    public async Task<IActionResult> AddKhoa([FromBody] Khoa_Model request)
    {
        try
        {
            int id =
                await _khoaService.AddKhoa(request);

            return Ok(new
            {
                message = "Them khoa thanh cong.",
                id_khoa = id
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

    [HttpPut("{id_khoa:int}")]
    public async Task<IActionResult> UpdateKhoa(
        int id_khoa,
        [FromBody] Khoa_Model request)
    {
        try
        {
            int affectedRows =
                await _khoaService.UpdateKhoa(id_khoa, request);

            if (affectedRows == 0)
            {
                return NotFound(new
                {
                    message = "Khong tim thay khoa."
                });
            }

            return Ok(new
            {
                message = "Cap nhat khoa thanh cong."
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

    [HttpDelete("{id_khoa:int}")]
    public async Task<IActionResult> DeleteKhoa(int id_khoa)
    {
        try
        {
            int affectedRows =
                await _khoaService.DeleteKhoa(id_khoa);

            if (affectedRows == 0)
            {
                return NotFound(new
                {
                    message = "Khong tim thay khoa."
                });
            }

            return Ok(new
            {
                message = "Xoa khoa thanh cong."
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

    [HttpGet("{id_khoa}/KiThucTap")]
    public async Task<IActionResult> GetKiThucTapByKhoa(int id_khoa)
    {
        var kiThucTap =
            await _khoaService.GetKiThucTapByKhoa(id_khoa);

        return Ok(kiThucTap);
    }

    [HttpPost("CreateKiThucTap")]
    public async Task<IActionResult> AddKiThucTap(
        [FromBody] KiThucTapDTO request)
    {
        int id = await _khoaService.AddThucTap(request);

        return Ok(new
        {
            message = "Tạo kỳ thực tập thành công",
            id_ki_thuc_tap = id
        });
    }

    [HttpPut("KiThucTap/{id_ki_thuc_tap}")]
    public async Task<IActionResult> UpdateKiThucTap(
        int id_ki_thuc_tap,
        [FromBody] UpdateKiThucTapDTO request)
    {
        try
        {
            int affectedRows =
                await _khoaService.UpdateKiThucTap(
                    id_ki_thuc_tap,
                    request);

            if (affectedRows == 0)
            {
                return NotFound(new
                {
                    message = "Không tìm thấy kì thực tập"
                });
            }

            return Ok(new
            {
                message = "Cập nhật kì thực tập thành công",
                so_dong_cap_nhat = affectedRows
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

    [HttpDelete("KiThucTap/{id_ki_thuc_tap}")]
    public async Task<IActionResult> DeleteKiThucTap(int id_ki_thuc_tap)
    {
        int affectedRows =
            await _khoaService.DeleteKiThucTap(id_ki_thuc_tap);

        if (affectedRows == 0)
        {
            return NotFound(new
            {
                message = "Không tìm thấy kì thực tập"
            });
        }

        return Ok(new
        {
            message = "Xóa kì thực tập thành công",
            so_dong_xoa = affectedRows
        });
    }

    [HttpGet("KiThucTap/{id_ki_thuc_tap}/SinhVien")]
    public async Task<IActionResult> GetSinhVienThucTapByKi(int id_ki_thuc_tap)
    {
        var sinhVien =
            await _khoaService.GetSinhVienThucTapByKi(id_ki_thuc_tap);

        return Ok(sinhVien);
    }

    [HttpGet("{id_khoa}/TieuChi")]
    public async Task<IActionResult> GetTieuChiByKhoa(int id_khoa)
    {
        var tieuChi =
            await _khoaService.GetTieuChiByKhoa(id_khoa);

        return Ok(tieuChi);
    }

    [HttpPost("TieuChi")]
    public async Task<IActionResult> AddTieuChi(
        [FromBody] TieuChiDanhGiaDTO request)
    {
        try
        {
            int id =
                await _khoaService.AddTieuChi(request);

            return Ok(new
            {
                message = "Tạo tiêu chí đánh giá thành công",
                id_tieu_chi = id
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

    [HttpPut("TieuChi/{id_tieu_chi}")]
    public async Task<IActionResult> UpdateTieuChi(
        int id_tieu_chi,
        [FromBody] TieuChiDanhGiaDTO request)
    {
        try
        {
            int affectedRows =
                await _khoaService.UpdateTieuChi(
                    id_tieu_chi,
                    request);

            if (affectedRows == 0)
            {
                return NotFound(new
                {
                    message = "Không tìm thấy tiêu chí đánh giá"
                });
            }

            return Ok(new
            {
                message = "Cập nhật tiêu chí đánh giá thành công",
                so_dong_cap_nhat = affectedRows
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

    [HttpDelete("TieuChi/{id_tieu_chi}")]
    public async Task<IActionResult> DeleteTieuChi(int id_tieu_chi)
    {
        int affectedRows =
            await _khoaService.DeleteTieuChi(id_tieu_chi);

        if (affectedRows == 0)
        {
            return NotFound(new
            {
                message = "Không tìm thấy tiêu chí đánh giá"
            });
        }

        return Ok(new
        {
            message = "Xóa tiêu chí đánh giá thành công",
            so_dong_xoa = affectedRows
        });
    }

    [HttpPost("ThemSVThucTap")]
    public async Task<IActionResult> AddSVThucTap(
        [FromBody] List<AddSinhVienThucTapDTO> request)
    {
        int id = await _sinhvienService.AddSinhVienThuc_Tap(request);

        return Ok(new
        {
            message = "Thêm danh sách sinh viên thực tập thành công",
            so_dong_them = id
        });
    }

    [HttpPost("ThemDVHDThucTap")]
    public async Task<IActionResult> AddDVHD(
        [FromBody] List<AddDonViHuongDanDTO> request)
    {
        int id = await _donvihuongdanService.AddListDonViHuongDan(request);

        return Ok(new
        {
            message = "Thêm danh sách ĐVHD thành công",
            so_dong_them = id
        });
    }

    [HttpGet("KiThucTap/{id_ki_thuc_tap}/DonViHuongDan")]
    public async Task<IActionResult> GetDonViHuongDanByKi(int id_ki_thuc_tap)
    {
        try
        {
            var result =
                await _donvihuongdanService.GetDonViHuongDanByKi(id_ki_thuc_tap);

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

    [HttpPost("DonViHuongDan")]
    public async Task<IActionResult> AddDonViHuongDan(
        [FromBody] AddDonViHuongDanDTO request)
    {
        try
        {
            int id =
                await _donvihuongdanService.AddDonViHuongDan(request);

            return Ok(new
            {
                message = "Thêm ĐVHD thành công.",
                id_don_vi_hd = id
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

    [HttpPut("DonViHuongDan/{id_don_vi_hd}")]
    public async Task<IActionResult> UpdateDonViHuongDan(
        int id_don_vi_hd,
        [FromBody] UpdateDonViHuongDanDTO request)
    {
        try
        {
            int affectedRows =
                await _donvihuongdanService.UpdateDonViHuongDan(
                    id_don_vi_hd,
                    request);

            if (affectedRows == 0)
            {
                return NotFound(new
                {
                    message = "Không tìm thấy ĐVHD."
                });
            }

            return Ok(new
            {
                message = "Cập nhật ĐVHD thành công."
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

    [HttpDelete("DonViHuongDan/{id_don_vi_hd}")]
    public async Task<IActionResult> DeleteDonViHuongDan(int id_don_vi_hd)
    {
        try
        {
            int affectedRows =
                await _donvihuongdanService.DeleteDonViHuongDan(id_don_vi_hd);

            if (affectedRows == 0)
            {
                return NotFound(new
                {
                    message = "Không tìm thấy ĐVHD."
                });
            }

            return Ok(new
            {
                message = "Xóa ĐVHD thành công."
            });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new
            {
                message = ex.Message
            });
        }
        catch
        {
            return BadRequest(new
            {
                message = "Không thể xóa ĐVHD vì đang có sinh viên thực tập liên kết."
            });
        }
    }

    [HttpPost("ImportSVThucTap")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> ImportSVThucTap(
        [FromForm] ImportSinhVienThucTapRequestDTO request)
    {
        try
        {
            var result =
                await _khoaService.ImportSinhVienThucTap(
                    request.File,
                    request.IdKiThucTap);

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

    [HttpPost("PreviewImportSVThucTap")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> PreviewImportSVThucTap(
        [FromForm] ImportSinhVienThucTapRequestDTO request)
    {
        try
        {
            var result =
                await _khoaService.PreviewImportSinhVienThucTap(
                    request.File,
                    request.IdKiThucTap);

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

    [HttpPost("PhanCongGVHD")]
    public async Task<IActionResult> PhanCongGVHD(
        [FromBody] PhanCongGiangVienDTO request)
    {
        try
        {
            string message =
                await _khoaService.PhanCongGiangVien(request);

            return Ok(new
            {
                message
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new
            {
                message = ex.Message
            });
        }
    }

    [HttpDelete("KiThucTap/{id_ki_thuc_tap}/SinhVien/{mssv}")]
    public async Task<IActionResult> DeleteSinhVienThucTap(
        int id_ki_thuc_tap,
        string mssv)
    {
        try
        {
            int affectedRows =
                await _khoaService.DeleteSinhVienThucTap(
                    mssv,
                    id_ki_thuc_tap);

            if (affectedRows == 0)
            {
                return NotFound(new
                {
                    message = "Không tìm thấy sinh viên thực tập để xóa."
                });
            }

            return Ok(new
            {
                message = "Xóa sinh viên thực tập thành công."
            });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new
            {
                message = ex.Message
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new
            {
                message = ex.Message
            });
        }
    }
}
