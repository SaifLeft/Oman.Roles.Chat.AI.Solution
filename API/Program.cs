using API.Services;
using Data.Structure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Services;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
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
builder.Services.AddSingleton<ILocalizationService, LocalizationService>();
builder.Services.AddHttpClient<IDeepSeekService, DeepSeekService>();
builder.Services.AddScoped<IDeepSeekService, DeepSeekService>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddHttpClient<IGoogleAuthService, GoogleAuthService>();
builder.Services.AddScoped<IGoogleAuthService, GoogleAuthService>();
builder.Services.AddSingleton<IChatRulesService, ChatRulesService>();
builder.Services.AddSingleton<IPdfService, PdfService>();
builder.Services.AddScoped<IChatService, ChatService>();

// Add configuration
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
builder.Configuration.AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true);
builder.Configuration["Localization:ResourcesPath"] = resourcesPath;
builder.Configuration["ChatSettings:PdfBasePath"] = pdfBasePath;
builder.Configuration["ChatSettings:PdfInfoPath"] = pdfInfoPath;
builder.Configuration["ChatSettings:DataPath"] = chatDataPath;
builder.Configuration["ChatSettings:RulesPath"] = chatRulesPath;

// Configure JWT Authentication
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

// Create initial localization files if they don't exist
if (!File.Exists(Path.Combine(resourcesPath, "en.json")))
{
    File.WriteAllText(Path.Combine(resourcesPath, "en.json"), @"{
  ""Errors"": {
    ""EmptyQuery"": ""Query cannot be empty"",
    ""ApiError"": ""An error occurred while processing your request"",
    ""ServiceUnavailable"": ""The service is currently unavailable, please try again later"",
    ""InvalidRequest"": ""Invalid request format"",
    ""ApiKeyMissing"": ""API key is not configured properly"",
    ""EndpointMissing"": ""API endpoint is not configured properly"",
    ""UnauthorizedAccess"": ""Unauthorized access to the service"",
    ""FileProcessingError"": ""Error processing PDF file"",
    ""InvalidCredentials"": ""Invalid username or password"",
    ""InvalidToken"": ""Invalid or expired token"",
    ""InvalidRefreshToken"": ""Invalid or expired refresh token"",
    ""GoogleAuthError"": ""Error during Google authentication"",
    ""GoogleConfigMissing"": ""Google authentication configuration is missing"",
    ""GoogleTokenError"": ""Failed to get access token from Google"",
    ""GoogleUserInfoError"": ""Failed to get user information from Google"",
    ""GoogleUrlCreationError"": ""Error creating Google login URL"",
    ""GoogleLoginError"": ""Error during Google login"",
    ""GoogleCallbackError"": ""Error processing Google authentication response"",
    ""GoogleSignInError"": ""Error during Google sign-in"",
    ""GoogleCodeRequired"": ""Authentication code is required"",
    ""ChatRoomTitleRequired"": ""Chat room title is required"",
    ""PdfFileNotFound"": ""PDF file '{0}' not found"",
    ""ChatRoomNotFound"": ""Chat room not found"",
    ""MessageContentRequired"": ""Message content is required"",
    ""MessageProcessingError"": ""Error processing message"",
    ""UnauthorizedOperation"": ""You are not authorized to perform this operation"",
    ""ChatRoomDeleteFailed"": ""Failed to delete chat room"",
    ""ChatRoomCreationError"": ""Error creating chat room"",
    ""ChatRoomRetrievalError"": ""Error retrieving chat room"",
    ""ChatRoomsRetrievalError"": ""Error retrieving chat rooms"",
    ""MessageSendingError"": ""Error sending message"",
    ""ChatRoomDeletionError"": ""Error deleting chat room"",
    ""UserIdRequired"": ""User ID is required"",
    ""RulesRetrievalError"": ""Error retrieving rules"",
    ""RulesContentRequired"": ""Rules content is required"",
    ""RulesUpdateFailed"": ""Failed to update rules"",
    ""RulesUpdateError"": ""Error updating rules"",
    ""RulesetsRetrievalError"": ""Error retrieving rulesets"",
    ""RulesetNameAndContentRequired"": ""Ruleset name and content are required"",
    ""RulesetAddFailed"": ""Failed to add ruleset"",
    ""RulesetAddError"": ""Error adding ruleset"",
    ""RulesetNameRequired"": ""Ruleset name is required"",
    ""RulesetDeleteFailed"": ""Failed to delete ruleset"",
    ""RulesetDeleteError"": ""Error deleting ruleset"",
    ""PdfFilesRetrievalError"": ""Error retrieving PDF files"",
    ""PdfFileInfoRetrievalError"": ""Error retrieving PDF file information"",
    ""PdfFileInfoUpdateError"": ""Error updating PDF file information""
  },
  ""Messages"": {
    ""ProcessingRequest"": ""Processing your request"",
    ""RequestReceived"": ""Request received successfully"",
    ""RequestCompleted"": ""Request processing completed"",
    ""LoginSuccess"": ""Login successful"",
    ""TokenRefreshed"": ""Token refreshed successfully"",
    ""TokenValid"": ""Token is valid"",
    ""GoogleLoginSuccess"": ""Successfully logged in with Google"",
    ""GoogleLoginUrlCreated"": ""Google login URL created successfully"",
    ""ChatRoomCreated"": ""Chat room created successfully"",
    ""ChatRoomDeleted"": ""Chat room deleted successfully"",
    ""ChatRoomWelcomeMessage"": ""Welcome to {0}! You can start asking questions about the PDF files associated with this room."",
    ""RulesUpdatedSuccess"": ""Rules updated successfully"",
    ""RulesetAddedSuccess"": ""Ruleset added successfully"",
    ""RulesetDeletedSuccess"": ""Ruleset deleted successfully"",
    ""PdfFileInfoUpdated"": ""PDF file information updated successfully""
  }
}");
}

if (!File.Exists(Path.Combine(resourcesPath, "ar.json")))
{
    File.WriteAllText(Path.Combine(resourcesPath, "ar.json"), @"{
  ""Errors"": {
    ""EmptyQuery"": ""·« Ì„ﬂ‰ √‰ ÌﬂÊ‰ «·«” ⁄·«„ ›«—€«"",
    ""ApiError"": ""ÕœÀ Œÿ√ √À‰«¡ „⁄«·Ã… ÿ·»ﬂ"",
    ""ServiceUnavailable"": ""«·Œœ„… €Ì— „ Ê›—… Õ«·Ì«° Ì—ÃÏ «·„Õ«Ê·… „—… √Œ—Ï ·«Õﬁ«"",
    ""InvalidRequest"": "" ‰”Ìﬁ «·ÿ·» €Ì— ’«·Õ"",
    ""ApiKeyMissing"": ""·„ Ì „  ﬂÊÌ‰ „› «Õ API »‘ﬂ· ’ÕÌÕ"",
    ""EndpointMissing"": ""·„ Ì „  ﬂÊÌ‰ ‰ﬁÿ… ‰Â«Ì… API »‘ﬂ· ’ÕÌÕ"",
    ""UnauthorizedAccess"": ""Ê’Ê· €Ì— „’—Õ »Â ≈·Ï «·Œœ„…"",
    ""FileProcessingError"": ""Œÿ√ ›Ì „⁄«·Ã… „·› PDF"",
    ""InvalidCredentials"": ""«”„ «·„” Œœ„ √Ê ﬂ·„… «·„—Ê— €Ì— ’ÕÌÕ…"",
    ""InvalidToken"": ""«·—„“ €Ì— ’«·Õ √Ê „‰ ÂÌ «·’·«ÕÌ…"",
    ""InvalidRefreshToken"": ""—„“ «· ÕœÌÀ €Ì— ’«·Õ √Ê „‰ ÂÌ «·’·«ÕÌ…"",
    ""GoogleAuthError"": ""ÕœÀ Œÿ√ √À‰«¡ «·„’«œﬁ… »«” Œœ«„ Google"",
    ""GoogleConfigMissing"": ""≈⁄œ«œ«  «·„’«œﬁ… »«” Œœ«„ Google €Ì— „ÊÃÊœ…"",
    ""GoogleTokenError"": ""›‘· ›Ì «·Õ’Ê· ⁄·Ï —„“ «·Ê’Ê· „‰ Google"",
    ""GoogleUserInfoError"": ""›‘· ›Ì «·Õ’Ê· ⁄·Ï „⁄·Ê„«  «·„” Œœ„ „‰ Google"",
    ""GoogleUrlCreationError"": ""Œÿ√ ›Ì ≈‰‘«¡ —«»ÿ  ”ÃÌ· «·œŒÊ· »«” Œœ«„ Google"",
    ""GoogleLoginError"": ""ÕœÀ Œÿ√ √À‰«¡  ”ÃÌ· «·œŒÊ· »«” Œœ«„ Google"",
    ""GoogleCallbackError"": ""Œÿ√ ›Ì „⁄«·Ã… «” Ã«»… «·„’«œﬁ… „‰ Google"",
    ""GoogleSignInError"": ""Œÿ√ √À‰«¡  ”ÃÌ· «·œŒÊ· »«” Œœ«„ Google"",
    ""GoogleCodeRequired"": ""—„“ «·„’«œﬁ… „ÿ·Ê»"",
    ""ChatRoomTitleRequired"": ""⁄‰Ê«‰ €—›… «·œ—œ‘… „ÿ·Ê»"",
    ""PdfFileNotFound"": ""„·› PDF '{0}' €Ì— „ÊÃÊœ"",
    ""ChatRoomNotFound"": ""€—›… «·œ—œ‘… €Ì— „ÊÃÊœ…"",
    ""MessageContentRequired"": ""„Õ ÊÏ «·—”«·… „ÿ·Ê»"",
    ""MessageProcessingError"": ""Œÿ√ ›Ì „⁄«·Ã… «·—”«·…"",
    ""UnauthorizedOperation"": ""€Ì— „’—Õ ·ﬂ » ‰›Ì– Â–Â «·⁄„·Ì…"",
    ""ChatRoomDeleteFailed"": ""›‘· ›Ì Õ–› €—›… «·œ—œ‘…"",
    ""ChatRoomCreationError"": ""Œÿ√ ›Ì ≈‰‘«¡ €—›… «·œ—œ‘…"",
    ""ChatRoomRetrievalError"": ""Œÿ√ ›Ì «” —Ã«⁄ €—›… «·œ—œ‘…"",
    ""ChatRoomsRetrievalError"": ""Œÿ√ ›Ì «” —Ã«⁄ €—› «·œ—œ‘…"",
    ""MessageSendingError"": ""Œÿ√ ›Ì ≈—”«· «·—”«·…"",
    ""ChatRoomDeletionError"": ""Œÿ√ ›Ì Õ–› €—›… «·œ—œ‘…"",
    ""UserIdRequired"": ""„⁄—› «·„” Œœ„ „ÿ·Ê»"",
    ""RulesRetrievalError"": ""Œÿ√ ›Ì «” —Ã«⁄ «·ﬁÊ«⁄œ"",
    ""RulesContentRequired"": ""„Õ ÊÏ «·ﬁÊ«⁄œ „ÿ·Ê»"",
    ""RulesUpdateFailed"": ""›‘· ›Ì  ÕœÌÀ «·ﬁÊ«⁄œ"",
    ""RulesUpdateError"": ""Œÿ√ ›Ì  ÕœÌÀ «·ﬁÊ«⁄œ"",
    ""RulesetsRetrievalError"": ""Œÿ√ ›Ì «” —Ã«⁄ „Ã„Ê⁄«  «·ﬁÊ«⁄œ"",
    ""RulesetNameAndContentRequired"": ""«”„ Ê„Õ ÊÏ „Ã„Ê⁄… «·ﬁÊ«⁄œ „ÿ·Ê»«‰"",
    ""RulesetAddFailed"": ""›‘· ›Ì ≈÷«›… „Ã„Ê⁄… «·ﬁÊ«⁄œ"",
    ""RulesetAddError"": ""Œÿ√ ›Ì ≈÷«›… „Ã„Ê⁄… «·ﬁÊ«⁄œ"",
    ""RulesetNameRequired"": ""«”„ „Ã„Ê⁄… «·ﬁÊ«⁄œ „ÿ·Ê»"",
    ""RulesetDeleteFailed"": ""›‘· ›Ì Õ–› „Ã„Ê⁄… «·ﬁÊ«⁄œ"",
    ""RulesetDeleteError"": ""Œÿ√ ›Ì Õ–› „Ã„Ê⁄… «·ﬁÊ«⁄œ"",
    ""PdfFilesRetrievalError"": ""Œÿ√ ›Ì «” —Ã«⁄ „·›«  PDF"",
    ""PdfFileInfoRetrievalError"": ""Œÿ√ ›Ì «” —Ã«⁄ „⁄·Ê„«  „·› PDF"",
    ""PdfFileInfoUpdateError"": ""Œÿ√ ›Ì  ÕœÌÀ „⁄·Ê„«  „·› PDF""
  },
  ""Messages"": {
    ""ProcessingRequest"": ""„⁄«·Ã… ÿ·»ﬂ"",
    ""RequestReceived"": "" „ «” ·«„ «·ÿ·» »‰Ã«Õ"",
    ""RequestCompleted"": ""«ﬂ „·  „⁄«·Ã… «·ÿ·»"",
    ""LoginSuccess"": "" „  ”ÃÌ· «·œŒÊ· »‰Ã«Õ"",
    ""TokenRefreshed"": "" „  ÕœÌÀ «·—„“ »‰Ã«Õ"",
    ""TokenValid"": ""«·—„“ ’«·Õ"",
    ""GoogleLoginSuccess"": "" „  ”ÃÌ· «·œŒÊ· »‰Ã«Õ »«” Œœ«„ Google"",
    ""GoogleLoginUrlCreated"": "" „ ≈‰‘«¡ —«»ÿ  ”ÃÌ· «·œŒÊ· »«” Œœ«„ Google »‰Ã«Õ"",
    ""ChatRoomCreated"": "" „ ≈‰‘«¡ €—›… «·œ—œ‘… »‰Ã«Õ"",
    ""ChatRoomDeleted"": "" „ Õ–› €—›… «·œ—œ‘… »‰Ã«Õ"",
    ""ChatRoomWelcomeMessage"": ""„—Õ»« »ﬂ ›Ì {0}! Ì„ﬂ‰ﬂ «·»œ¡ ›Ì ÿ—Õ «·√”∆·… ÕÊ· „·›«  PDF «·„— »ÿ… »Â–Â «·€—›…."",
    ""RulesUpdatedSuccess"": "" „  ÕœÌÀ «·ﬁÊ«⁄œ »‰Ã«Õ"",
    ""RulesetAddedSuccess"": "" „  ≈÷«›… „Ã„Ê⁄… «·ﬁÊ«⁄œ »‰Ã«Õ"",
    ""RulesetDeletedSuccess"": "" „ Õ–› „Ã„Ê⁄… «·ﬁÊ«⁄œ »‰Ã«Õ"",
    ""PdfFileInfoUpdated"": "" „  ÕœÌÀ „⁄·Ê„«  „·› PDF »‰Ã«Õ""
  }
}");
}

app.Run();