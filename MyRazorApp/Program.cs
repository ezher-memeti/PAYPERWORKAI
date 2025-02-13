using MyRazorApp.Services;
using MyRazorApp.Services.KlingAPI;

var builder = WebApplication.CreateBuilder(args);

 var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();

    
// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddHttpClient("KlingAPI", client =>
{
    client.BaseAddress = new Uri("https://api.klingai.com"); 
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

builder.Services.AddSingleton<JWTtoken>();
builder.Services.AddSingleton<IConfiguration>(configuration);
builder.Services.AddSingleton<VideoGenerationService>();
builder.Services.AddControllers();


// Add session support
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Session timeout
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

app.UseRouting();


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

/*app.MapGet("/token", (JWTtoken jwt) => 
{
    string token = jwt.Sign();
    return Results.Ok(new { Token = token });
});*/


app.UseHttpsRedirection();
app.UseStaticFiles();
app.MapControllers();

app.UseAuthorization();

// Enable session middleware
app.UseSession();


app.MapRazorPages();





app.Run();