using HotelBooking.webapp.Components;
using MudBlazor.Services;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http.Headers;
using HotelBooking.Client;
using MudBlazor;
using Microsoft.AspNetCore.SignalR;
using Radzen;

var builder = WebApplication.CreateBuilder(args);

// add service blazor

// Add services to the container.
builder.Services.AddRazorPages();

builder.Services.AddServerSideBlazor()
.AddCircuitOptions(options =>
{
    // Hiển thị lỗi chi tiết để dễ debug
    options.DetailedErrors = true;

    // Giữ trạng thái của Owner trong 10 phút nếu lỡ bị rớt mạng/Wifi chập chờn
    // Giúp họ không bị mất dữ liệu đang nhập dở khi kết nối lại
    options.DisconnectedCircuitRetentionPeriod = TimeSpan.FromMinutes(10);
});

// --- 2. CẤU HÌNH SIGNALR (QUAN TRỌNG CHO UPLOAD ẢNH) ---
builder.Services.AddSignalR(e =>
{
    // Tăng giới hạn gói tin lên 100MB (Mặc định chỉ 32KB -> Upload ảnh là sập ngay)
    // 100MB đủ sức chứa cho 5-10 ảnh chất lượng cao
    e.MaximumReceiveMessageSize = 100 * 1024 * 1024;

    // Tăng thời gian chờ Client phản hồi lên 5 phút
    // Hữu ích khi máy Owner yếu hoặc đang xử lý nén ảnh lâu
    e.ClientTimeoutInterval = TimeSpan.FromMinutes(5);

    // Gửi tín hiệu "ping" giữ kết nối mỗi 15 giây
    e.KeepAliveInterval = TimeSpan.FromSeconds(15);
});

// add service http client
builder.Services.AddHttpClient("HotelBookingAPI", client =>
{
    // client.BaseAddress = new Uri("http://localhost:5083/api/");
    var apiBaseUrl = builder.Configuration["ApiBaseUrl"] ?? "http://localhost:5083/api/";
    client.BaseAddress = new Uri(apiBaseUrl);
    client.Timeout = TimeSpan.FromMinutes(10);
});

builder.Services.AddMudServices(config =>
{
    config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.TopRight;
    config.SnackbarConfiguration.PreventDuplicates = true;
    config.SnackbarConfiguration.NewestOnTop = false;
    config.SnackbarConfiguration.ShowCloseIcon = true;
    config.SnackbarConfiguration.VisibleStateDuration = 5000;
    config.SnackbarConfiguration.SnackbarVariant = MudBlazor.Variant.Filled;
});

builder.Services.AddRadzenComponents();
// add service blazor local storage
builder.Services.AddBlazoredLocalStorage();

// Add Authentication & Authorization services
builder.Services.AddAuthentication();
builder.Services.AddAuthorization();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddScoped<IHotelServices, HotelServices>();
builder.Services.AddScoped<HotelFormState>();
builder.Services.AddScoped<BookingState>();
// builder.Services.AddScoped<IToastService, ToastService>();
builder.Services.AddAuthorizationCore();


var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseRouting(); // chia các components thành page
app.UseStaticFiles(); // wwwroot thư mục tài nguyên

app.UseAuthentication();
app.UseAuthorization();

app.UseHttpsRedirection();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
