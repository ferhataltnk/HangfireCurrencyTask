using Business.Abstract;
using Business.Concrete;
using DataAccess.Abstract;
using DataAccess.Concrete;
using Hangfire;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

var hangfireConnectionStrings = "Server=NB317493; Database=hangfireCurrencyTaskDb; Trusted_Connection=True";

builder.Services.AddHangfire(x=>
    {
        x.UseSqlServerStorage(hangfireConnectionStrings);
        RecurringJob.AddOrUpdate<CurrencyManager>(j=>j.CurrencyReadXmlWriteSql(),cronExpression: "0 * * * *");
    });

builder.Services.AddHangfireServer();


builder.Services.AddSingleton<ICurrencyService,CurrencyManager>();
builder.Services.AddSingleton<ICurrencyDal, CurrencyDal>();

var app = builder.Build();

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

app.UseHangfireDashboard(pathMatch: "");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
