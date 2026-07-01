using MySql.Data.MySqlClient;


public class ThongBaoKhoaObserver : IObserver
{
    private readonly string _connectionString;

    public ThongBaoKhoaObserver(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Missing DefaultConnection connection string.");
    }

    public async Task Update(string ma_nguoi_nhan,string tieu_de,string noi_dung)
    {
        using MySqlConnection conn =
            new MySqlConnection(_connectionString);

        await conn.OpenAsync();

        string sql = @"
            INSERT INTO thong_bao
            (
                loai_nguoi_nhan,
                ma_nguoi_nhan,
                tieu_de,
                noi_dung
            )
            VALUES
            (
                'khoa',
                @ma_nguoi_nhan,
                @tieu_de,
                @noi_dung
            )
        ";

        using MySqlCommand cmd =
            new MySqlCommand(sql, conn);




        cmd.Parameters.AddWithValue("@ma_nguoi_nhan", ma_nguoi_nhan);
        cmd.Parameters.AddWithValue("@tieu_de",tieu_de);
        cmd.Parameters.AddWithValue("@noi_dung",noi_dung);

        await cmd.ExecuteNonQueryAsync();
    }
}
