using Data.Structure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Services;
using Services.Common;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<MuhamiContext>(
        options => options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
// Configure localization resources path
var resourcesPath = Path.Combine(builder.Environment.ContentRootPath, "Resources");
Directory.CreateDirectory(resourcesPath); // Ensure the directory exists

// Configure PDF files path
var pdfBasePath = Path.Combine(builder.Environment.ContentRootPath, "PdfFiles");
Directory.CreateDirectory(pdfBasePath); // Ensure the directory exists

// Configure PDF info path
var pdfInfoPath = Path.Combine(builder.Environment.ContentRootPath, "PdfInfo");
Directory.CreateDirectory(pdfInfoPath); // Ensure the directory exists

// Configure chat data path
var chatDataPath = Path.Combine(builder.Environment.ContentRootPath, "ChatData");
Directory.CreateDirectory(chatDataPath); // Ensure the directory exists

// Configure chat rules path
var chatRulesPath = Path.Combine(builder.Environment.ContentRootPath, "ChatRules");
Directory.CreateDirectory(chatRulesPath); // Ensure the directory exists

// Register our services
builder.Services.AddHttpClient<IDeepSeekService, DeepSeekService>();
builder.Services.AddScoped<IDeepSeekService, DeepSeekService>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddHttpClient<IGoogleAuthService, GoogleAuthService>();
builder.Services.AddScoped<IGoogleAuthService, GoogleAuthService>();
builder.Services.AddScoped<IChatRulesService, ChatRulesService>();
builder.Services.AddScoped<IPdfService, PdfService>();
builder.Services.AddScoped<IConversationTrackingService, ConversationTrackingService>();
builder.Services.AddScoped<ISubscriptionService, SubscriptionService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<ILocalizationService, LocalizationService>();
builder.Services.AddScoped<IChatRulesService, ChatRulesService>();
builder.Services.AddScoped<IPdfExtractionService, PdfExtractionService>();
builder.Services.AddScoped<IPdfService, PdfService>();
builder.Services.AddScoped<IPdfSourceManagementService, PdfSourceManagementService>();
// Register Thawani Payment Service
builder.Services.AddHttpClient<IThawaniPaymentService, ThawaniPaymentService>();
builder.Services.AddScoped<IThawaniPaymentService, ThawaniPaymentService>();

// Add configuration
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
builder.Configuration.AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true);
builder.Configuration["Localization:ResourcesPath"] = resourcesPath;
builder.Configuration["ChatSettings:PdfBasePath"] = pdfBasePath;
builder.Configuration["ChatSettings:PdfInfoPath"] = pdfInfoPath;
builder.Configuration["ChatSettings:DataPath"] = chatDataPath;
builder.Configuration["ChatSettings:RulesPath"] = chatRulesPath;

// ����� ���� ��������
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var key = Encoding.ASCII.GetBytes(jwtSettings["Secret"] ?? throw new InvalidOperationException("JWT Secret is not configured"));

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidateAudience = true,
        ValidAudience = jwtSettings["Audience"],
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAutoMapper(typeof(MappingProfile));
builder.Services.AddAutoMapper(typeof(SubscriptionMappingProfile));

// ����� �������
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"));
    options.AddPolicy("RequireUserRole", policy => policy.RequireRole("User"));
});

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Use CORS
app.UseCors("AllowAll");

app.UseHttpsRedirection();

// Enable authentication and authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
