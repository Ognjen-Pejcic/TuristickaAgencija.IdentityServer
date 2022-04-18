using IdentityServer4.Services;
using IdentityServerHost.Quickstart.UI;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Server;
using Server.Data;

var seed = args.Contains("/seed");
if (seed)
{
    args = args.Except(new[] { "/seed" }).ToArray();
}

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var defaultConnString = builder.Configuration.GetConnectionString("DefaultConnection");
var assembly = typeof(Program).Assembly.GetName().ToString();

if (seed)
{
    SeedData.EnsureSeedData(defaultConnString);
}

builder.Services.AddDbContext<AspDotNetDbContext>(options =>
{
    options.UseSqlServer(defaultConnString, b => b.MigrationsAssembly(assembly));
});

builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<AspDotNetDbContext>();

builder.Services.AddIdentityServer()
    .AddAspNetIdentity<IdentityUser>()
    .AddConfigurationStore(options =>
    {
        options.ConfigureDbContext = b =>
        b.UseSqlServer(defaultConnString, opt => opt.MigrationsAssembly(assembly));
    }).AddOperationalStore(options =>
    {
        options.ConfigureDbContext = b =>
       b.UseSqlServer(defaultConnString, opt => opt.MigrationsAssembly(assembly));
    }).AddDeveloperSigningCredential();


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddSingleton<ICorsPolicyService>((container) => {
    var logger = container.GetRequiredService<ILogger<DefaultCorsPolicyService>>();
    return new DefaultCorsPolicyService(logger)
    {
        AllowAll = true
    };
});


var app = builder.Build();
app.UseStaticFiles();
app.UseRouting();
app.UseIdentityServer();


app.UseCors(options => options.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

app.UseAuthorization();
//app.UseEndpoints(endpoints =>
//{
//    endpoints.MapDefaultControllerRoute();
//});
// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
app.UseSwagger();
app.UseSwaggerUI();
//}

app.UseHttpsRedirection();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run();

