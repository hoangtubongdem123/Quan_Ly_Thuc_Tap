public class GiangVienService : IGiangVienService
{
    private readonly IGiangVienRepository _GiangVienRepository;

    public GiangVienService(
        IGiangVienRepository GiangVienRepository
    )
    {
        _GiangVienRepository = GiangVienRepository;
    }

    public async Task<List<Giang_Vien_Model>> GetAll()
    {
        return await _GiangVienRepository.GetAll();
    }
    public async Task<Giang_Vien_Model?> GetGiangVien(string msgv)
    {
        return await _GiangVienRepository.GetGiangVien(msgv);
    }

    public async Task<Giang_Vien_Model?> GetGiangVienByGmail(string gmail)
    {
        return await _GiangVienRepository.GetGiangVienByGmail(gmail);
    }

    public async Task<int> AddGiangVien(Giang_Vien_Model request)
    {
        ValidateGiangVien(request);

        return await _GiangVienRepository.AddGiangVien(request);
    }

    public async Task<int> UpdateGiangVien(
        string maSoGiangVien,
        Giang_Vien_Model request)
    {
        if (string.IsNullOrWhiteSpace(maSoGiangVien))
        {
            throw new ArgumentException("Ma so giang vien khong duoc de trong.");
        }

        request.MaSoGiangVien = maSoGiangVien;
        ValidateGiangVien(request);

        return await _GiangVienRepository.UpdateGiangVien(
            maSoGiangVien,
            request);
    }

    public async Task<int> DeleteGiangVien(string maSoGiangVien)
    {
        if (string.IsNullOrWhiteSpace(maSoGiangVien))
        {
            throw new ArgumentException("Ma so giang vien khong duoc de trong.");
        }

        return await _GiangVienRepository.DeleteGiangVien(maSoGiangVien);
    }

    public async Task<List<GiangVienPhuTrachCountDTO>> CountSinhVienDangHuongDanByGiangVien()
    {
        return await _GiangVienRepository.CountSinhVienDangHuongDanByGiangVien();
    }

    public async Task<List<SinhVienPhuTrachGiangVienDTO>> GetSinhVienPhuTrach(
        string maSoGiangVien)
    {
        if (string.IsNullOrWhiteSpace(maSoGiangVien))
        {
            throw new ArgumentException("Thieu ma so giang vien.");
        }

        return await _GiangVienRepository.GetSinhVienPhuTrach(
            maSoGiangVien.Trim());
    }

    private static void ValidateGiangVien(Giang_Vien_Model request)
    {
        if (string.IsNullOrWhiteSpace(request.MaSoGiangVien))
        {
            throw new ArgumentException("Ma so giang vien khong duoc de trong.");
        }

        if (request.IdKhoa <= 0)
        {
            throw new ArgumentException("Ma khoa khong hop le.");
        }

        if (string.IsNullOrWhiteSpace(request.TenGiangVien))
        {
            throw new ArgumentException("Ten giang vien khong duoc de trong.");
        }

        if (string.IsNullOrWhiteSpace(request.GmailGiangVien))
        {
            throw new ArgumentException("Gmail giang vien khong duoc de trong.");
        }

        if (string.IsNullOrWhiteSpace(request.PasswordGiangVien))
        {
            throw new ArgumentException("Mat khau giang vien khong duoc de trong.");
        }
    }
}
    












