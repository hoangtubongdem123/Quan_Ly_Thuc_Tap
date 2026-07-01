public class AuthService
{
    private readonly IKhoaRepository _khoaRepository;

    public AuthService(IKhoaRepository khoaRepository)
    {
        _khoaRepository = khoaRepository;
    }

    public async Task<List<Khoa_Model>> GetAll()
    {
        return await _khoaRepository.GetAll();
    }
}