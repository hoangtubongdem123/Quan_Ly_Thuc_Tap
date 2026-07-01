public interface IMinhChungService
{
    Task<Minh_Chung_Model> AddMinhChung(AddMinhChungDTO request);

    Task<List<Minh_Chung_Model>> GetMinhChungByMssv(string mssv);

    Task<Minh_Chung_Model> UpdateMinhChung(int idMinhChung, UpdateMinhChungDTO request);

    Task<int> DeleteMinhChung(int idMinhChung);
}
