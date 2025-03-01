using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
    ""EmptyQuery"": ""áÇ íãßä Ãä íßæä ÇáÇÓÊÚáÇã İÇÑÛğÇ"",
    ""ApiError"": ""ÍÏË ÎØÃ ÃËäÇÁ ãÚÇáÌÉ ØáÈß"",
    ""ServiceUnavailable"": ""ÇáÎÏãÉ ÛíÑ ãÊæİÑÉ ÍÇáíğÇ¡ íÑÌì ÇáãÍÇæáÉ ãÑÉ ÃÎÑì áÇÍŞğÇ"",
    ""InvalidRequest"": ""ÊäÓíŞ ÇáØáÈ ÛíÑ ÕÇáÍ"",
    ""ApiKeyMissing"": ""áã íÊã Êßæíä ãİÊÇÍ API ÈÔßá ÕÍíÍ"",
    ""EndpointMissing"": ""áã íÊã Êßæíä äŞØÉ äåÇíÉ API ÈÔßá ÕÍíÍ"",
    ""UnauthorizedAccess"": ""æÕæá ÛíÑ ãÕÑÍ Èå Åáì ÇáÎÏãÉ"",
    ""FileProcessingError"": ""ÎØÃ İí ãÚÇáÌÉ ãáİ PDF"",
    ""InvalidCredentials"": ""ÇÓã ÇáãÓÊÎÏã Ãæ ßáãÉ ÇáãÑæÑ ÛíÑ ÕÍíÍÉ"",
    ""InvalidToken"": ""ÇáÑãÒ ÛíÑ ÕÇáÍ Ãæ ãäÊåí ÇáÕáÇÍíÉ"",
    ""InvalidRefreshToken"": ""ÑãÒ ÇáÊÍÏíË ÛíÑ ÕÇáÍ Ãæ ãäÊåí ÇáÕáÇÍíÉ"",
    ""GoogleAuthError"": ""ÍÏË ÎØÃ ÃËäÇÁ ÇáãÕÇÏŞÉ ÈÇÓÊÎÏÇã Google"",
    ""GoogleConfigMissing"": ""ÅÚÏÇÏÇÊ ÇáãÕÇÏŞÉ ÈÇÓÊÎÏÇã Google ÛíÑ ãæÌæÏÉ"",
    ""GoogleTokenError"": ""İÔá İí ÇáÍÕæá Úáì ÑãÒ ÇáæÕæá ãä Google"",
    ""GoogleUserInfoError"": ""İÔá İí ÇáÍÕæá Úáì ãÚáæãÇÊ ÇáãÓÊÎÏã ãä Google"",
    ""GoogleUrlCreationError"": ""ÎØÃ İí ÅäÔÇÁ ÑÇÈØ ÊÓÌíá ÇáÏÎæá ÈÇÓÊÎÏÇã Google"",
    ""GoogleLoginError"": ""ÍÏË ÎØÃ ÃËäÇÁ ÊÓÌíá ÇáÏÎæá ÈÇÓÊÎÏÇã Google"",
    ""GoogleCallbackError"": ""ÎØÃ İí ãÚÇáÌÉ ÇÓÊÌÇÈÉ ÇáãÕÇÏŞÉ ãä Google"",
    ""GoogleSignInError"": ""ÎØÃ ÃËäÇÁ ÊÓÌíá ÇáÏÎæá ÈÇÓÊÎÏÇã Google"",
    ""GoogleCodeRequired"": ""ÑãÒ ÇáãÕÇÏŞÉ ãØáæÈ"",
    ""ChatRoomTitleRequired"": ""ÚäæÇä ÛÑİÉ ÇáÏÑÏÔÉ ãØáæÈ"",
    ""PdfFileNotFound"": ""ãáİ PDF '{0}' ÛíÑ ãæÌæÏ"",
    ""ChatRoomNotFound"": ""ÛÑİÉ ÇáÏÑÏÔÉ ÛíÑ ãæÌæÏÉ"",
    ""MessageContentRequired"": ""ãÍÊæì ÇáÑÓÇáÉ ãØáæÈ"",
    ""MessageProcessingError"": ""ÎØÃ İí ãÚÇáÌÉ ÇáÑÓÇáÉ"",
    ""UnauthorizedOperation"": ""ÛíÑ ãÕÑÍ áß ÈÊäİíĞ åĞå ÇáÚãáíÉ"",
    ""ChatRoomDeleteFailed"": ""İÔá İí ÍĞİ ÛÑİÉ ÇáÏÑÏÔÉ"",
    ""ChatRoomCreationError"": ""ÎØÃ İí ÅäÔÇÁ ÛÑİÉ ÇáÏÑÏÔÉ"",
    ""ChatRoomRetrievalError"": ""ÎØÃ İí ÇÓÊÑÌÇÚ ÛÑİÉ ÇáÏÑÏÔÉ"",
    ""ChatRoomsRetrievalError"": ""ÎØÃ İí ÇÓÊÑÌÇÚ ÛÑİ ÇáÏÑÏÔÉ"",
    ""MessageSendingError"": ""ÎØÃ İí ÅÑÓÇá ÇáÑÓÇáÉ"",
    ""ChatRoomDeletionError"": ""ÎØÃ İí ÍĞİ ÛÑİÉ ÇáÏÑÏÔÉ"",
    ""UserIdRequired"": ""ãÚÑİ ÇáãÓÊÎÏã ãØáæÈ"",
    ""RulesRetrievalError"": ""ÎØÃ İí ÇÓÊÑÌÇÚ ÇáŞæÇÚÏ"",
    ""RulesContentRequired"": ""ãÍÊæì ÇáŞæÇÚÏ ãØáæÈ"",
    ""RulesUpdateFailed"": ""İÔá İí ÊÍÏíË ÇáŞæÇÚÏ"",
    ""RulesUpdateError"": ""ÎØÃ İí ÊÍÏíË ÇáŞæÇÚÏ"",
    ""RulesetsRetrievalError"": ""ÎØÃ İí ÇÓÊÑÌÇÚ ãÌãæÚÇÊ ÇáŞæÇÚÏ"",
    ""RulesetNameAndContentRequired"": ""ÇÓã æãÍÊæì ãÌãæÚÉ ÇáŞæÇÚÏ ãØáæÈÇä"",
    ""RulesetAddFailed"": ""İÔá İí ÅÖÇİÉ ãÌãæÚÉ ÇáŞæÇÚÏ"",
    ""RulesetAddError"": ""ÎØÃ İí ÅÖÇİÉ ãÌãæÚÉ ÇáŞæÇÚÏ"",
    ""RulesetNameRequired"": ""ÇÓã ãÌãæÚÉ ÇáŞæÇÚÏ ãØáæÈ"",
    ""RulesetDeleteFailed"": ""İÔá İí ÍĞİ ãÌãæÚÉ ÇáŞæÇÚÏ"",
    ""RulesetDeleteError"": ""ÎØÃ İí ÍĞİ ãÌãæÚÉ ÇáŞæÇÚÏ"",
    ""PdfFilesRetrievalError"": ""ÎØÃ İí ÇÓÊÑÌÇÚ ãáİÇÊ PDF"",
    ""PdfFileInfoRetrievalError"": ""ÎØÃ İí ÇÓÊÑÌÇÚ ãÚáæãÇÊ ãáİ PDF"",
    ""PdfFileInfoUpdateError"": ""ÎØÃ İí ÊÍÏíË ãÚáæãÇÊ ãáİ PDF""
  },
  ""Messages"": {
    ""ProcessingRequest"": ""ãÚÇáÌÉ ØáÈß"",
    ""RequestReceived"": ""Êã ÇÓÊáÇã ÇáØáÈ ÈäÌÇÍ"",
    ""RequestCompleted"": ""ÇßÊãáÊ ãÚÇáÌÉ ÇáØáÈ"",
    ""LoginSuccess"": ""Êã ÊÓÌíá ÇáÏÎæá ÈäÌÇÍ"",
    ""TokenRefreshed"": ""Êã ÊÍÏíË ÇáÑãÒ ÈäÌÇÍ"",
    ""TokenValid"": ""ÇáÑãÒ ÕÇáÍ"",
    ""GoogleLoginSuccess"": ""Êã ÊÓÌíá ÇáÏÎæá ÈäÌÇÍ ÈÇÓÊÎÏÇã Google"",
    ""GoogleLoginUrlCreated"": ""Êã ÅäÔÇÁ ÑÇÈØ ÊÓÌíá ÇáÏÎæá ÈÇÓÊÎÏÇã Google ÈäÌÇÍ"",
    ""ChatRoomCreated"": ""Êã ÅäÔÇÁ ÛÑİÉ ÇáÏÑÏÔÉ ÈäÌÇÍ"",
    ""ChatRoomDeleted"": ""Êã ÍĞİ ÛÑİÉ ÇáÏÑÏÔÉ ÈäÌÇÍ"",
    ""ChatRoomWelcomeMessage"": ""ãÑÍÈğÇ Èß İí {0}! íãßäß ÇáÈÏÁ İí ØÑÍ ÇáÃÓÆáÉ Íæá ãáİÇÊ PDF ÇáãÑÊÈØÉ ÈåĞå ÇáÛÑİÉ."",
    ""RulesUpdatedSuccess"": ""Êã ÊÍÏíË ÇáŞæÇÚÏ ÈäÌÇÍ"",
    ""RulesetAddedSuccess"": ""ÊãÊ ÅÖÇİÉ ãÌãæÚÉ ÇáŞæÇÚÏ ÈäÌÇÍ"",
    ""RulesetDeletedSuccess"": ""Êã ÍĞİ ãÌãæÚÉ ÇáŞæÇÚÏ ÈäÌÇÍ"",
    ""PdfFileInfoUpdated"": ""Êã ÊÍÏíË ãÚáæãÇÊ ãáİ PDF ÈäÌÇÍ""
  }
}");
}

app.Run();