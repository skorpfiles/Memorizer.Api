using Autofac;
using Autofac.Extensions.DependencyInjection;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SkorpFiles.Memorizer.Api.Web.Authorization;
using SkorpFiles.Memorizer.Api.BusinessLogic.DependencyInjection;
using SkorpFiles.Memorizer.Api.DataAccess;
using SkorpFiles.Memorizer.Api.DataAccess.DependencyInjection;
using SkorpFiles.Memorizer.Api.DataAccess.Mapping;
using SkorpFiles.Memorizer.Api.Web.Mapping;
using StackExchange.Redis;
using Autofac.Core;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(
        options =>
        {
            options.RequireHttpsMetadata = false;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                // укзывает, будет ли валидироваться издатель при валидации токена
                ValidateIssuer = true,
                // строка, представляющая издателя
                ValidIssuer = AuthOptions.ISSUER,

                // будет ли валидироваться потребитель токена
                ValidateAudience = true,
                // установка потребителя токена
                ValidAudience = AuthOptions.AUDIENCE,
                // будет ли валидироваться время существования
                ValidateLifetime = true,

                // установка ключа безопасности
                IssuerSigningKey = AuthOptions.GetSymmetricSecurityKey(builder.Configuration),
                // валидация ключа безопасности
                ValidateIssuerSigningKey = true,
            };
        });

string connectionString = builder.Configuration["DatabaseConnectionString"]!;

builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlServer(connectionString));

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();

var multiplexer = ConnectionMultiplexer.Connect(builder.Configuration["RedisConnectionString"]);
builder.Services.AddSingleton<IConnectionMultiplexer>(multiplexer);

const string frontendUrl = "http://localhost:3000"; //todo move to config

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
    builder =>
    {
        builder.WithOrigins(frontendUrl)
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

builder.Services.AddSingleton<IAuthorizationMiddlewareResultHandler, AuthorizationWithCheckTokenMiddlewareResultHandler>();

var mapperConfig = new MapperConfiguration(mc =>
{
    mc.AddProfile(new RequestsMappingProfile());
    mc.AddProfile(new ResponsesMappingProfile());
    mc.AddProfile(new ApiEntitiesMappingProfile());

    mc.AddProfile(new DataAccessMappingProfile());
});

IMapper mapper = mapperConfig.CreateMapper();
builder.Services.AddSingleton(mapper);

builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory())
    .ConfigureContainer<ContainerBuilder>(containerBuilder =>
    {
        var opt = new DbContextOptionsBuilder<ApplicationDbContext>();
        opt.UseSqlServer(connectionString);
        containerBuilder.RegisterInstance(new ApplicationDbContext(opt.Options)).Keyed<ApplicationDbContext>("DbContext");
        containerBuilder.RegisterModule(new DataAccessModule());
        containerBuilder.RegisterModule(new BusinessLogicModule());
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Use(async (context, next) =>
{
    var methodvalue = context.Request.Method;
    if (!string.IsNullOrEmpty(methodvalue))
    {

        if (methodvalue == HttpMethods.Options || methodvalue == HttpMethods.Head)
        {
            context.Response.Headers.Add("Access-Control-Allow-Origin", frontendUrl);
            context.Response.Headers.Add("Access-Control-Allow-Headers", "Access-Control-Allow-Origin,Access-Control-Allow-Headers,content-type");
            await next();
        }
        else
        {
            await next();
        }
    }
});

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseCors();

app.MapControllers();

app.Run();