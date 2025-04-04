using MauiStore.Web.Components;
using MauiStore.Shared.Services;
using MauiStore.Web.Services;
using MudBlazor.Services;
using Blazored.LocalStorage;
using MauiStore.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddMudServices(); // MudBlazor
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddScoped<ClientPreferenceManager>();

// Add device-specific services used by the MauiStore.Shared project
builder.Services.AddSingleton<IFormFactor, FormFactor>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddAdditionalAssemblies(typeof(MauiStore.Shared._Imports).Assembly);

app.Run();
