using MySql.Data.MySqlClient;

public class MinhChungRepository : IMinhChungRepository
{
    private readonly string _connectionString;

    public MinhChungRepository(IConfiguration config)
    {
        _connectionString = config.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Missing DefaultConnection connection string.");
    }

    public async Task<int> AddMinhChung(Minh_Chung_Model minhChung)
    {
        using MySqlConnection conn =
            new MySqlConnection(_connectionString);

        await conn.OpenAsync();

        string sql = @"
            INSERT INTO minh_chung
            (
                ten_minh_chung,
                mssv,
                path
            )
            VALUES
            (
                @ten_minh_chung,
                @mssv,
                @path
            );

            SELECT LAST_INSERT_ID();
        ";

        using MySqlCommand cmd =
            new MySqlCommand(sql, conn);

        AddParameters(cmd, minhChung);

        return Convert.ToInt32(await cmd.ExecuteScalarAsync());
    }

    public async Task<List<Minh_Chung_Model>> GetMinhChungByMssv(string mssv)
    {
        using MySqlConnection conn =
            new MySqlConnection(_connectionString);

        await conn.OpenAsync();

        string sql = @"
            SELECT
                id_minh_chung,
                ten_minh_chung,
                mssv,
                path
            FROM minh_chung
            WHERE mssv = @mssv
            ORDER BY id_minh_chung DESC
        ";

        using MySqlCommand cmd =
            new MySqlCommand(sql, conn);

        cmd.Parameters.AddWithValue("@mssv", mssv);

        using var reader =
            await cmd.ExecuteReaderAsync();

        List<Minh_Chung_Model> result = new();

        while (await reader.ReadAsync())
        {
            result.Add(ReadMinhChung(reader));
        }

        return result;
    }

    public async Task<Minh_Chung_Model?> GetMinhChung(int idMinhChung)
    {
        using MySqlConnection conn =
            new MySqlConnection(_connectionString);

        await conn.OpenAsync();

        string sql = @"
            SELECT
                id_minh_chung,
                ten_minh_chung,
                mssv,
                path
            FROM minh_chung
            WHERE id_minh_chung = @id_minh_chung
            LIMIT 1
        ";

        using MySqlCommand cmd =
            new MySqlCommand(sql, conn);

        cmd.Parameters.AddWithValue("@id_minh_chung", idMinhChung);

        using var reader =
            await cmd.ExecuteReaderAsync();

        if (await reader.ReadAsync())
        {
            return ReadMinhChung(reader);
        }

        return null;
    }

    public async Task<int> UpdateMinhChung(Minh_Chung_Model minhChung)
    {
        using MySqlConnection conn =
            new MySqlConnection(_connectionString);

        await conn.OpenAsync();

        string sql = @"
            UPDATE minh_chung
            SET ten_minh_chung = @ten_minh_chung,
                path = @path
            WHERE id_minh_chung = @id_minh_chung
        ";

        using MySqlCommand cmd =
            new MySqlCommand(sql, conn);

        AddParameters(cmd, minhChung);
        cmd.Parameters.AddWithValue("@id_minh_chung", minhChung.IdMinhChung);

        return await cmd.ExecuteNonQueryAsync();
    }

    public async Task<int> DeleteMinhChung(int idMinhChung)
    {
        using MySqlConnection conn =
            new MySqlConnection(_connectionString);

        await conn.OpenAsync();

        string sql = @"
            DELETE FROM minh_chung
            WHERE id_minh_chung = @id_minh_chung
        ";

        using MySqlCommand cmd =
            new MySqlCommand(sql, conn);

        cmd.Parameters.AddWithValue("@id_minh_chung", idMinhChung);

        return await cmd.ExecuteNonQueryAsync();
    }

    public async Task<bool> SinhVienExists(string mssv)
    {
        using MySqlConnection conn =
            new MySqlConnection(_connectionString);

        await conn.OpenAsync();

        string sql = @"
            SELECT COUNT(*)
            FROM sinh_vien
            WHERE mssv = @mssv
        ";

        using MySqlCommand cmd =
            new MySqlCommand(sql, conn);

        cmd.Parameters.AddWithValue("@mssv", mssv);

        return Convert.ToInt32(await cmd.ExecuteScalarAsync()) > 0;
    }

    private static void AddParameters(
        MySqlCommand cmd,
        Minh_Chung_Model minhChung)
    {
        cmd.Parameters.AddWithValue("@ten_minh_chung", minhChung.TenMinhChung);
        cmd.Parameters.AddWithValue("@mssv", minhChung.Mssv);
        cmd.Parameters.AddWithValue("@path", minhChung.Path);
    }

    private static Minh_Chung_Model ReadMinhChung(System.Data.Common.DbDataReader reader)
    {
        return new Minh_Chung_Model
        {
            IdMinhChung = Convert.ToInt32(reader["id_minh_chung"]),
            TenMinhChung = reader["ten_minh_chung"].ToString() ?? "",
            Mssv = reader["mssv"].ToString() ?? "",
            Path = reader["path"].ToString() ?? ""
        };
    }
}
