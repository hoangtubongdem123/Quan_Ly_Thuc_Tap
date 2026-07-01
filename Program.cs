using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// Add services
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<ISinhVienRepository, SinhVienRepository>();
builder.Services.AddScoped<ISinhVienService, SinhVienService>();

builder.Services.AddScoped<IKhoaRepository, KhoaRepository>();
builder.Services.AddScoped<IKhoaService, KhoaService>();

builder.Services.AddScoped<IGiangVienRepository, GiangVienRepository>();
builder.Services.AddScoped<IGiangVienService, GiangVienService>();

builder.Services.AddScoped<IDonViHuongDanRepository, DonViHuongDanRepository>();
builder.Services.AddScoped<IDonViHuongDanService, DonViHuongDanService>();

builder.Services.AddScoped<IDanhGiaThucTapRepository, DanhGiaThucTapRepository>();
builder.Services.AddScoped<IDanhGiaThucTapService, DanhGiaThucTapService>();

builder.Services.AddScoped<IMinhChungRepository, MinhChungRepository>();
builder.Services.AddScoped<IMinhChungService, MinhChungService>();

builder.Services.AddScoped<IAdminRepository, AdminRepository>();
builder.Services.AddScoped<IAdminService, AdminService>();

var dataProtectionPath =
    Path.Combine(builder.Environment.ContentRootPath, ".aspnet-data-protection");

Directory.CreateDirectory(dataProtectionPath);

builder.Services
    .AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(dataProtectionPath));


builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
});



builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy =>
        {
            policy.WithOrigins(
                    "http://localhost:5173",
                    "http://localhost:5174",
                    "http://127.0.0.1:5173",
                    "http://127.0.0.1:5174",
                    "http://127.0.0.1:5175")
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials();
        });
});






var app = builder.Build();

// Configure HTTP pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors("AllowFrontend");
app.UseHttpsRedirection();
app.UseSession();

var minhChungPath =
    Path.Combine(builder.Environment.ContentRootPath, "Minhchung");

Directory.CreateDirectory(minhChungPath);

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(minhChungPath),
    RequestPath = "/Minhchung"
});

app.MapControllers();

app.Run();
