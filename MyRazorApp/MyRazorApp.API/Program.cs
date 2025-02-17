using MyRazorApp.Services;
using MyRazorApp.Services.KlingAPI;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddHttpClient("KlingAPI", client =>
{
    client.BaseAddress = new Uri("https://api.klingai.com"); 
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

builder.Services.AddSingleton<JWTtoken>();
builder.Services.AddSingleton<IConfiguration>(builder.Configuration);
builder.Services.AddSingleton<VideoGenerationService>();
builder.Services.AddScoped<VideoQueryService>();

builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();

// Add session support
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Session timeout
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.MapControllers();
app.UseRouting();
app.UseSession();
app.UseAuthorization();

// Map Razor Pages
app.MapRazorPages();

app.Run();
