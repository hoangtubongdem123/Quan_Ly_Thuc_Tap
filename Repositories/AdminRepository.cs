using MySql.Data.MySqlClient;

public class AdminRepository : IAdminRepository
{
    private readonly string _connectionString;

    public AdminRepository(IConfiguration config)
    {
        _connectionString = config.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Missing DefaultConnection connection string.");
    }

    public async Task<Admin_Model?> GetAdminByTaiKhoan(string taiKhoan)
    {
        using MySqlConnection conn =
            new MySqlConnection(_connectionString);

        await conn.OpenAsync();

        string sql = @"
            SELECT *
            FROM admin
            WHERE taikhoan_admin = @taiKhoan
        ";

        using MySqlCommand cmd =
            new MySqlCommand(sql, conn);

        cmd.Parameters.AddWithValue("@taiKhoan", taiKhoan);

        using var reader =
            await cmd.ExecuteReaderAsync();

        if (!await reader.ReadAsync())
        {
            return null;
        }

        return new Admin_Model
        {
            IdAdmin = reader.GetInt32(reader.GetOrdinal("id_admin")),
            TaiKhoanAdmin = reader.IsDBNull(reader.GetOrdinal("taikhoan_admin"))
                ? string.Empty
                : reader.GetString(reader.GetOrdinal("taikhoan_admin")),
            PasswordAdmin = reader.GetString(reader.GetOrdinal("password_admin"))
        };
    }
}
