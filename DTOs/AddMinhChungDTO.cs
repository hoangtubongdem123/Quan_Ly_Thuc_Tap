public class AddMinhChungDTO
{
    public string TenMinhChung { get; set; } = string.Empty;

    public string MSSV { get; set; } = string.Empty;

    public IFormFile File { get; set; } = default!;
}
