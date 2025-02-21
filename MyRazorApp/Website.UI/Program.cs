var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

// Register IHttpContextAccessor to enable accessing HttpContext
builder.Services.AddHttpContextAccessor();


builder.Services.AddHttpClient("server", client =>
{
    client.BaseAddress = new Uri("http://localhost:5123"); // Adjust port if different
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

// Add session support
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Set session timeout
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

// Enable session middleware
app.UseRouting();
app.UseSession();  // This is the key part for enabling sessions
app.UseAuthorization();

app.MapRazorPages();

app.Run();
