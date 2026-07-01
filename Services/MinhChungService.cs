public class MinhChungService : IMinhChungService
{
    private static readonly HashSet<string> AllowedExtensions =
        new(StringComparer.OrdinalIgnoreCase)
        {
            ".pdf",
            ".doc",
            ".docx",
            ".xls",
            ".xlsx",
            ".png",
            ".jpg",
            ".jpeg"
        };

    private readonly IMinhChungRepository _repository;
    private readonly IWebHostEnvironment _environment;

    public MinhChungService(
        IMinhChungRepository repository,
        IWebHostEnvironment environment)
    {
        _repository = repository;
        _environment = environment;
    }

    public async Task<Minh_Chung_Model> AddMinhChung(AddMinhChungDTO request)
    {
        await ValidateAddMinhChung(request);

        string uploadRoot =
            Path.Combine(_environment.ContentRootPath, "Minhchung");

        Directory.CreateDirectory(uploadRoot);

        string extension =
            Path.GetExtension(request.File.FileName);

        string fileName =
            $"{request.MSSV.Trim()}_{Guid.NewGuid():N}{extension}";

        string physicalPath =
            Path.Combine(uploadRoot, fileName);

        await using (var stream = File.Create(physicalPath))
        {
            await request.File.CopyToAsync(stream);
        }

        string relativePath =
            $"/Minhchung/{fileName}";

        Minh_Chung_Model minhChung = new()
        {
            TenMinhChung = request.TenMinhChung.Trim(),
            Mssv = request.MSSV.Trim(),
            Path = relativePath
        };

        try
        {
            minhChung.IdMinhChung =
                await _repository.AddMinhChung(minhChung);
        }
        catch
        {
            if (File.Exists(physicalPath))
            {
                File.Delete(physicalPath);
            }

            throw;
        }

        return minhChung;
    }

    public async Task<List<Minh_Chung_Model>> GetMinhChungByMssv(string mssv)
    {
        if (string.IsNullOrWhiteSpace(mssv))
        {
            throw new ArgumentException("Thieu MSSV.");
        }

        return await _repository.GetMinhChungByMssv(mssv.Trim());
    }

    public async Task<Minh_Chung_Model> UpdateMinhChung(
        int idMinhChung,
        UpdateMinhChungDTO request)
    {
        Minh_Chung_Model? minhChung =
            await _repository.GetMinhChung(idMinhChung);

        if (minhChung == null)
        {
            throw new ArgumentException("Minh chung khong ton tai.");
        }

        if (string.IsNullOrWhiteSpace(request.TenMinhChung))
        {
            throw new ArgumentException("Ten minh chung khong duoc de trong.");
        }

        string? newPhysicalPath = null;
        string? oldPhysicalPath = null;

        if (request.File is { Length: > 0 })
        {
            ValidateFile(request.File);

            string uploadRoot =
                Path.Combine(_environment.ContentRootPath, "Minhchung");

            Directory.CreateDirectory(uploadRoot);

            string extension =
                Path.GetExtension(request.File.FileName);

            string fileName =
                $"{minhChung.Mssv}_{Guid.NewGuid():N}{extension}";

            newPhysicalPath =
                Path.Combine(uploadRoot, fileName);

            await using (var stream = File.Create(newPhysicalPath))
            {
                await request.File.CopyToAsync(stream);
            }

            oldPhysicalPath =
                Path.Combine(
                    uploadRoot,
                    Path.GetFileName(minhChung.Path));

            minhChung.Path =
                $"/Minhchung/{fileName}";
        }

        minhChung.TenMinhChung =
            request.TenMinhChung.Trim();

        try
        {
            await _repository.UpdateMinhChung(minhChung);
        }
        catch
        {
            if (!string.IsNullOrWhiteSpace(newPhysicalPath)
                && File.Exists(newPhysicalPath))
            {
                File.Delete(newPhysicalPath);
            }

            throw;
        }

        if (!string.IsNullOrWhiteSpace(oldPhysicalPath)
            && File.Exists(oldPhysicalPath))
        {
            File.Delete(oldPhysicalPath);
        }

        return minhChung;
    }

    public async Task<int> DeleteMinhChung(int idMinhChung)
    {
        Minh_Chung_Model? minhChung =
            await _repository.GetMinhChung(idMinhChung);

        if (minhChung == null)
        {
            throw new ArgumentException("Minh chung khong ton tai.");
        }

        int rows =
            await _repository.DeleteMinhChung(idMinhChung);

        string fileName =
            Path.GetFileName(minhChung.Path);

        string physicalPath =
            Path.Combine(_environment.ContentRootPath, "Minhchung", fileName);

        if (File.Exists(physicalPath))
        {
            File.Delete(physicalPath);
        }

        return rows;
    }

    private async Task ValidateAddMinhChung(AddMinhChungDTO request)
    {
        if (string.IsNullOrWhiteSpace(request.TenMinhChung))
        {
            throw new ArgumentException("Ten minh chung khong duoc de trong.");
        }

        if (string.IsNullOrWhiteSpace(request.MSSV))
        {
            throw new ArgumentException("Thieu MSSV.");
        }

        if (request.File == null || request.File.Length == 0)
        {
            throw new ArgumentException("File minh chung khong hop le.");
        }

        ValidateFile(request.File);

        bool sinhVienExists =
            await _repository.SinhVienExists(request.MSSV.Trim());

        if (!sinhVienExists)
        {
            throw new ArgumentException("Sinh vien khong ton tai.");
        }
    }

    private static void ValidateFile(IFormFile file)
    {
        string extension =
            Path.GetExtension(file.FileName);

        if (!AllowedExtensions.Contains(extension))
        {
            throw new ArgumentException("Dinh dang file minh chung khong duoc ho tro.");
        }
    }
}
