using Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Entities;
using WebApp.Helpers;
using Infrastructure.Repositories;
using Infrastructure.Services;
using WebApp.Services;
using WebApp.Hubs;


var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRouting(x => x.LowercaseUrls = true);

builder.Services.AddControllersWithViews()
    .AddRazorRuntimeCompilation();

builder.Services.AddDbContext<DataContext>(x => x.UseSqlServer(builder.Configuration.GetConnectionString("SqlServer")));
builder.Services.AddScoped<AddressRepository>();
builder.Services.AddScoped<AddressService>();

builder.Services.AddScoped<RideRepository>();
builder.Services.AddScoped<SearchRequestRepository>();
builder.Services.AddScoped<RideService>();
builder.Services.AddScoped<AccountService>();
builder.Services.AddScoped<BookingRepository>();
builder.Services.AddScoped<BookingService>();
builder.Services.AddScoped< OpenRouteService > ();
builder.Services.AddScoped<SearchService>();

builder.Services.AddHttpClient();



builder.Services.AddDefaultIdentity<UserEntity>(x => {
    x.User.RequireUniqueEmail = true;
    x.SignIn.RequireConfirmedAccount = false;
    x.Password.RequiredLength = 8;

}).AddEntityFrameworkStores<DataContext>();

builder.Services.ConfigureApplicationCookie(x =>
{
    x.Cookie.HttpOnly = true;
    x.LoginPath = "/signin";
    x.LogoutPath = "/signout";
    x.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    x.ExpireTimeSpan = TimeSpan.FromMinutes(60);
    x.SlidingExpiration = true;
});

builder.Services.AddAuthentication().AddFacebook(x =>
{
    x.AppId = "2959151207556497";
    x.AppSecret = "46e72e1affb7b4d0cf8e7e0eca1f4537";
    x.Fields.Add("first_name");
    x.Fields.Add("last_name");

});

builder.Services.AddAuthentication().AddGoogle(x =>
{
    x.ClientId = "1042689527093-td0lbn8c93e52u562eka48n8djj40ui7.apps.googleusercontent.com";
    x.ClientSecret = "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";

});


builder.Services.AddHttpClient<OpenRouteService>();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials()
              .SetIsOriginAllowed(_ => true); // ?? for testing only
    });
});



builder.Logging.ClearProviders();
builder.Logging.AddConsole();

builder.Services.AddSignalR();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<DataContext>();
    db.Database.Migrate();
}

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/error", "?statusCode={0}");
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseCors();

app.UseAuthentication();
app.UseUserSessionValidation();
app.UseAuthorization();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapHub<ChatHub>("/chathub");

app.MapHub<SearchChatHub>("/searchChatHub");

app.Run();
