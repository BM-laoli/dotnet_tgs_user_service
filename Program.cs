using Microsoft.EntityFrameworkCore;
using Rabbit.Zookeeper;
using Serilog;
using user_service_api.Middleware;
using user_service_api.Models;
using System;
using System.IO;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using user_service_api.Dto;
using user_service_api.Extensions;
using user_service_api.Repository;
using user_service_api.Service;
using user_service_api.Tasks;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();
builder.Services.AddEndpointsApiExplorer();

// 日志
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug() // 设置日志记录的最低级别
    .WriteTo.File("logs/myapp.log", rollingInterval: RollingInterval.Day) // 每天创建一个新的日志文件
    .CreateLogger();
builder.Host.UseSerilog(); // 将 Serilog 用作日志提供程序

// zk 
builder.Services.AddSingleton<IZookeeperService>(sp =>
    new ZookeeperService(new ZookeeperClientOptions(builder.Configuration.GetConnectionString("zookeeper"))
{
    BasePath = "/",
    ConnectionTimeout = TimeSpan.FromSeconds(10),
    SessionTimeout = TimeSpan.FromSeconds(20),
    OperatingTimeout = TimeSpan.FromSeconds(60),
    ReadOnly = false,
    SessionId = 0,
    SessionPasswd = null,
    EnableEphemeralNodeRestore = true,
}));

IZookeeperService zkService = builder.Services.BuildServiceProvider().GetRequiredService<IZookeeperService>();
MysqlConfig mysqlConfig = await zkService.GetAsync<MysqlConfig>("/tgs_config/service/user_service/mysql.json");
JwtSettings jwtSettings = await zkService.GetAsync<JwtSettings>("/tgs_config/service/user_service/jwt.json");
RedisSettings redisSettings = await zkService.GetAsync<RedisSettings>("/tgs_config/service/user_service/redis.json");
RabbitMQSettings mqSettings = await zkService.GetAsync<RabbitMQSettings>("/tgs_config/service/user_service/mq.json");

// mysql
builder.Services.AddDbContext<ApplicationDbContext>(opt =>
    opt.UseMySql(
        mysqlConfig.DefaultConnection,
        ServerVersion.AutoDetect(mysqlConfig.DefaultConnection))
);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key))
        };
    });
builder.Services.AddAuthorization();

// redis
builder.Services.AddSingleton<IRedisService>(sp =>
    new RedisService(redisSettings.RedisConnection));

// mq
builder.Services.AddSingleton<IRabbitMQService>(sp => new RabbitMQService(mqSettings.MQConnection));
builder.Services.AddSingleton<MQTask>();
var messageReceiver = builder.Services.BuildServiceProvider().GetRequiredService<MQTask>();
messageReceiver.Start();

// 注入实际业务
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddSingleton<PasswordManager>();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });

    // 添加 JWT 验证的安全方案定义
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "请输入 JWT 令牌，格式: Bearer {token}",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey
    });

    // 为需要验证的 API 应用安全方案
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });

    // 其他 Swagger 配置...
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.RequestLoggingMiddleware();
app.UseHttpsRedirection();
app.UseRequestCulture();
app.ExceptionHandlingMiddleware();


app.MapControllers();

app.Run();
