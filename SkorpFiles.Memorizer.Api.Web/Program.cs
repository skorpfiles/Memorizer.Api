using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SkorpFiles.Memorizer.Api.Web.Authorization;
using SkorpFiles.Memorizer.Api.BusinessLogic.DependencyInjection;
using SkorpFiles.Memorizer.Api.DataAccess;
using SkorpFiles.Memorizer.Api.DataAccess.DependencyInjection;
using SkorpFiles.Memorizer.Api.DataAccess.Mapping;
using SkorpFiles.Memorizer.Api.Web.Mapping;
using SkorpFiles.Memorizer.Api.Web.Authorization.TokensCache;
using SkorpFiles.Memorizer.Api.DataAccess.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddApplicationInsightsTelemetry();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthorizationBuilder()
    .AddPolicy("GeneralUser", policy =>
        policy.RequireClaim("TokenPurpose", "General"))
    .AddPolicy("Training", policy =>
        policy.RequireClaim("TokenPurpose", "Training"));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(
        options =>
        {
            options.RequireHttpsMetadata = false;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = AuthOptions.ISSUER,
                ValidateAudience = true,
                ValidAudience = AuthOptions.AUDIENCE,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
            };

            var authenticationTokenCipherKey = builder.Configuration["AuthenticationTokenCipherKey"];
            if (authenticationTokenCipherKey != null)
            {
                options.TokenValidationParameters.IssuerSigningKey = AuthOptions.GetSymmetricSecurityKey(Encoding.ASCII.GetBytes(authenticationTokenCipherKey));
            }    
        });

string connectionString = builder.Configuration["DatabaseConnectionString"]!;

builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlServer(connectionString));

builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();

var cacheType = builder.Configuration["AuthorizationCacheType"];
switch (cacheType)
{
    case "redis":
        RedisTokenCache? tokenCache = new();
        tokenCache.Initialize(builder.Configuration["RedisConnectionString"]!);
        builder.Services.AddSingleton(tokenCache);
        break;
    case "db":
        builder.Services.AddScoped<ITokenCache, DbTokenCache>();
        break;
}

string[] frontendOrigins = builder.Configuration["FrontendOrigins"]!.Split(';', StringSplitOptions.TrimEntries);

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
    builder =>
    {
        builder.WithOrigins(frontendOrigins)
        .AllowAnyHeader()
        .WithMethods("GET", "PUT", "POST", "DELETE", "OPTIONS")
        .AllowCredentials();
    });
});

builder.Services.ConfigureApplicationCookie(options => {
    options.Events.OnRedirectToLogin = context => {
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        return Task.CompletedTask;
    };
});

builder.Services.AddScoped<IAuthorizationMiddlewareResultHandler, AuthorizationWithCheckTokenMiddlewareResultHandler>();

var mapperConfig = new MapperConfiguration(mc =>
{
    mc.AddProfile(new RequestsMappingProfile());
    mc.AddProfile(new ResponsesMappingProfile());
    mc.AddProfile(new ApiEntitiesMappingProfile());

    mc.AddProfile(new DataAccessMappingProfile());
});

IMapper mapper = mapperConfig.CreateMapper();
builder.Services.AddScoped(services => mapperConfig.CreateMapper());

builder.Logging.ClearProviders().AddApplicationInsights();

builder.Services.AddRepositories();
builder.Services.AddBusinessLogic();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseCors();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseExceptionHandler("/error");
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();