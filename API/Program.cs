using API.Filters;
using API.Middleware;
using Data.Structure;
// using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Services;
using Services.Common;
using Services.Security;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers(options =>
{
    options.Filters.Add<ValidationFilter>();
});

builder.Services.AddHttpContextAccessor();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Smart Lawyer API",
        Version = "v1",
        Description = "API for the Smart Lawyer legal consultation system - نظام المحامي الذكي للاستشارات القانونية",
        Contact = new OpenApiContact
        {
            Name = "Smart Lawyer Support",
            Email = "support@smartlawyer.om"
        }
    });

    // Add JWT Authentication to Swagger UI
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token in the text input below. Example: 'Bearer 12345abcdef'",
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });

    // Include XML comments if available
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }
});
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
// Uncommented and updated services
builder.Services.AddScoped<ILegalContextService, LegalContextService>();
builder.Services.AddScoped<IPdfSourceManagementService, PdfSourceManagementService>();
// Register DeepSeekService - needed by LegalContextService and others
builder.Services.AddHttpClient<IDeepSeekService, DeepSeekService>();
builder.Services.AddScoped<IDeepSeekService, DeepSeekService>();
// Register ModelService.DeepSeekService for DeepSeekController
builder.Services.AddScoped<Services.ModelService.IDeepSeekService, Services.ModelService.DeepSeekService>();
// Register ChatDbService - needed by ChatAIService
builder.Services.AddScoped<IChatDbService, ChatDbService>();
// Register SMS Service with Twilio integration
builder.Services.AddScoped<ISmsService, SmsService>();
// Register Thawani Payment Service
builder.Services.AddHttpClient<IThawaniPaymentService, ThawaniPaymentService>();
builder.Services.AddScoped<IThawaniPaymentService, ThawaniPaymentService>();
// Uncommented services that were previously commented out
builder.Services.AddScoped<ISubscriptionStatusService, SubscriptionStatusService>();
builder.Services.AddScoped<IAdminAnalyticsService, AdminAnalyticsService>();
builder.Services.AddScoped<IChatSubscriptionService, ChatSubscriptionService>();

// Register new AI services using existing interfaces (not from Interfaces namespace)
builder.Services.AddScoped<Services.IChatAIService, Services.ChatAIService>();

// Register file management service
builder.Services.AddScoped<IFileManagementService, FileManagementService>();

// Register security services
builder.Services.AddScoped<IEncryptionService, EncryptionService>();
builder.Services.AddMemoryCache(); // Required for rate limiting

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

// Add encryption settings if not present
if (!builder.Configuration.GetSection("EncryptionSettings").Exists())
{
    // Generate random keys for development (in production these should be set in environment variables)
    using var aes = System.Security.Cryptography.Aes.Create();
    aes.GenerateKey();
    aes.GenerateIV();

    builder.Configuration["EncryptionSettings:Key"] = Convert.ToBase64String(aes.Key);
    builder.Configuration["EncryptionSettings:IV"] = Convert.ToBase64String(aes.IV);
}

// Add rate limit settings if not present
if (!builder.Configuration.GetSection("RateLimitSettings").Exists())
{
    builder.Configuration["RateLimitSettings:MaxRequests"] = "100";
    builder.Configuration["RateLimitSettings:WindowInSeconds"] = "60";
}

// تسجيل الخدمات في حاوية الحقن
builder.Services.AddScoped<IMessageClassificationService, MessageClassificationService>();
builder.Services.AddScoped<IConversationOrganizationService, ConversationOrganizationService>();

// تسجيل HttpClient للخدمات الخارجية
builder.Services.AddHttpClient("AIServices", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["AIServices:BaseUrl"] ?? "http://localhost:5000");
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Smart Lawyer API v1");
        c.ShowExtensions();
        //c.RoutePrefix = string.Empty; // Set Swagger UI at the root
        //c.DefaultModelsExpandDepth(-1); // Hide the schemas section
        //c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None); // Collapse operations on load
    });
}

// Use CORS
app.UseCors("AllowAll");

app.UseHttpsRedirection();

// Add explicit health check endpoint
app.MapGet("/health", () => Results.Ok(new { status = "Healthy", timestamp = DateTime.Now }));

// Enable authentication and authorization
app.UseAuthentication();
app.UseAuthorization();

// Use security headers middleware
app.UseSecurityHeaders();

// Use rate limiting middleware
app.UseRateLimiting();

app.MapControllers();

app.Run();