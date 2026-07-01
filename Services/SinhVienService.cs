public class SinhVienService : ISinhVienService
{

    private readonly IConfiguration _configuration;
    private readonly ISinhVienRepository _SinhVienRepository;

    public SinhVienService(ISinhVienRepository SinhVienRepository,IConfiguration configuration)
    {
        _SinhVienRepository = SinhVienRepository;
        _configuration = configuration;
    }

    public async Task<List<Sinh_Vien_Model>> GetAll()
    {
        return await _SinhVienRepository.GetAll();
    }
    public async Task<Sinh_Vien_Model?> GetSinhVien(string mssv)
    {
        return await _SinhVienRepository.GetSinhVien(mssv);
    }

    public async Task<Sinh_Vien_Model?> GetSinhVienByGmail(string gmail)
    {
        return await _SinhVienRepository.GetSinhVienByGmail(gmail);
    }

    public async Task<int> AddSinhVien(Sinh_Vien_Model request)
    {
        ValidateSinhVien(request);

        return await _SinhVienRepository.AddSinhVien(request);
    }

    public async Task<int> UpdateSinhVien(string mssv, Sinh_Vien_Model request)
    {
        if (string.IsNullOrWhiteSpace(mssv))
        {
            throw new ArgumentException("MSSV khong duoc de trong.");
        }

        request.Mssv = mssv;
        ValidateSinhVien(request);

        return await _SinhVienRepository.UpdateSinhVien(mssv, request);
    }

    public async Task<int> DeleteSinhVien(string mssv)
    {
        if (string.IsNullOrWhiteSpace(mssv))
        {
            throw new ArgumentException("MSSV khong duoc de trong.");
        }

        return await _SinhVienRepository.DeleteSinhVien(mssv);
    }

    public async Task<List<SinhVienKiThucTapDTO>> GetKiThucTapByMssv(string mssv)
    {
        return await _SinhVienRepository.GetKiThucTapByMssv(mssv);
    }

    public async Task<List<Thong_Bao_Model>> GetThongBaoByMssv(string mssv)
    {
        return await _SinhVienRepository.GetThongBaoByMssv(mssv);
    }
    
    public async Task<int> AddSinhVienThuc_Tap(List<AddSinhVienThucTapDTO> listsvthuctap)
    {   

        int idSVThucTap = await _SinhVienRepository.AddSinhVienThucTap(listsvthuctap);

        ThucTapSubject subject = new ThucTapSubject() ;

        subject.Attach(new ThongBaoSinhVienObserver(_configuration)) ;


        foreach(AddSinhVienThucTapDTO sv in listsvthuctap)
        {
            
            await subject.Notify(sv.MSSV.ToString(), "Thông báo thực tập" ,"Đơn ĐK TT đã được duyệt, chờ Khoa phân công GVHD");
        }
        
        
        



        
        return idSVThucTap;
    }

    private static void ValidateSinhVien(Sinh_Vien_Model request)
    {
        if (string.IsNullOrWhiteSpace(request.Mssv))
        {
            throw new ArgumentException("MSSV khong duoc de trong.");
        }

        if (request.IdKhoa <= 0)
        {
            throw new ArgumentException("Ma khoa khong hop le.");
        }

        if (string.IsNullOrWhiteSpace(request.TenSinhVien))
        {
            throw new ArgumentException("Ten sinh vien khong duoc de trong.");
        }

        if (string.IsNullOrWhiteSpace(request.GmailSinhVien))
        {
            throw new ArgumentException("Gmail sinh vien khong duoc de trong.");
        }

        if (string.IsNullOrWhiteSpace(request.PasswordSinhVien))
        {
            throw new ArgumentException("Mat khau sinh vien khong duoc de trong.");
        }
    }
    
    
    
    }







    








    
