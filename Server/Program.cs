using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using cognify.Server.Data;
using cognify.Server.Services;
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddScoped<TextLoaderService>(); //Added this so that TypeRacerController would work :)
builder.Services.AddScoped<GameResultService>(); // This one for adding the GameResults
builder.Services.AddSingleton<ActivePlayerService>();


builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

var app = builder.Build();

// Apply pending migrations at startup
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.Migrate(); // Applies any pending migrations
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseRouting();

app.UseCors(builder =>
    builder.WithOrigins("https://black-ocean-02025cd03.4.azurestaticapps.net/", "http://localhost:5296").AllowAnyHeader().AllowAnyMethod());

app.MapRazorPages();
app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();