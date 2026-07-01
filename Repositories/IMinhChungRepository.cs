public interface IMinhChungRepository
{
    Task<int> AddMinhChung(Minh_Chung_Model minhChung);

    Task<List<Minh_Chung_Model>> GetMinhChungByMssv(string mssv);

    Task<Minh_Chung_Model?> GetMinhChung(int idMinhChung);

    Task<int> UpdateMinhChung(Minh_Chung_Model minhChung);

    Task<int> DeleteMinhChung(int idMinhChung);

    Task<bool> SinhVienExists(string mssv);
}
