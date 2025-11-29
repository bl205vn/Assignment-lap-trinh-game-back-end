using Cai_San_Thu_Vien.Entity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var chuoiketnoi = builder.Configuration.GetConnectionString("ketnoi");
builder.Services.AddDbContext<MinecraftContext>(x => x.UseSqlServer(chuoiketnoi));

//Đăng ký dịch vụ cho Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add services to the container.
builder.Services.AddControllersWithViews();

//Cấu hình JSON serializer để xử lý circular reference (tham chiếu vòng tròn)
//Khi entity có navigation properties tham chiếu lẫn nhau (ví dụ: Play -> Mode -> Plays -> Mode...)
//sẽ gây lỗi khi serialize JSON. IgnoreCycles sẽ bỏ qua các tham chiếu vòng tròn này
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });

var app = builder.Build();

//Gọi Swagger
app.UseSwagger();
app.UseSwaggerUI();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
