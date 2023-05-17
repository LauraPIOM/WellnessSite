﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using WellnessSite.Data;
using Microsoft.AspNetCore.Identity;
using MessagePack.Resolvers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages(options =>
{
    options.Conventions.AuthorizeFolder("/Admin", "Admin");
    options.Conventions.AuthorizeFolder("/Admin", "OrgAdmin");
});

builder.Services.AddDbContext<WellnessSiteContext>(options => {
    options.UseSqlServer(builder.Configuration.GetConnectionString("WellnessSiteContext") ?? throw new InvalidOperationException("Connection string 'WellnessSiteContext' not found."));
    options.EnableSensitiveDataLogging();
    });

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<WellnessSiteContext>()
    .AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.AccessDeniedPath = "/auth/Access-Denied";
    options.LoginPath = "/auth/Login";
    options.LogoutPath = "/auth/Logout";
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Admin", policy =>
    {
        policy.RequireRole("Admin");
    });
    options.AddPolicy("OrgAdmin", policy =>
    {
        policy.RequireRole("OrgAdmin");
    });
});

var app = builder.Build();


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

//using (var scope = app.Services.CreateScope())
//{
//    var services = scope.ServiceProvider;
//    var context = services.GetRequiredService<WellnessSiteContext>();
//    context.Database.EnsureCreated();
//}

    app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();
app.UseAuthentication();

app.MapRazorPages();

app.Run();
