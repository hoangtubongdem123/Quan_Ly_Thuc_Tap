public class ImportSinhVienThucTapRequestDTO
{
    public int IdKiThucTap { get; set; }

    public IFormFile File { get; set; } = default!;
}
