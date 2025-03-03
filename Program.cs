using AljasAuthApi.Services;
using AljasAuthApi.Config;
using AljasAuthApi.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using System.Text;
using StackExchange.Redis;
using ProjectHierarchyApi.Services;

var builder = WebApplication.CreateBuilder(args);

// ✅ Load MongoDB Settings (or use default if not found)
var mongoSettings = builder.Configuration.GetSection("MongoDbSettings").Get<MongoDbSettings>() ?? new MongoDbSettings
{
    ConnectionString = "mongodb://172.16.100.67:27017",
    DatabaseName = "Aljas",
    EmployeesCollectionName = "Employees",
    SubcontractorCollectionName = "Subcontractors"
};

// ✅ Load JWT Settings (or use default if not found)
var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>() ?? new JwtSettings
{
    SecretKey = "a54bc4a85f99f01f4fa91cd540653bbdd06a4cd325e23e1c228cf46531fbfb24",
    Issuer = "AljasIssuer",
    Audience = "AljasAudience",
    TokenExpirationMinutes = 60
};

// ✅ Load Redis Settings (or use default if not found)
var redisSettings = builder.Configuration.GetSection("RedisSettings").Get<RedisSettings>() ?? new RedisSettings
{
    ConnectionString = "43.204.40.208:6380",
    StreamKey = "user-events"
};

// ✅ Register MongoDB Client and Database
var mongoClient = new MongoClient(mongoSettings.ConnectionString);
var mongoDatabase = mongoClient.GetDatabase(mongoSettings.DatabaseName);
builder.Services.AddSingleton<IMongoClient>(mongoClient);
builder.Services.AddSingleton<IMongoDatabase>(mongoDatabase);

// ✅ Register MongoDB Collections
builder.Services.AddSingleton(sp => mongoDatabase.GetCollection<User>("Users"));
builder.Services.AddSingleton(sp => mongoDatabase.GetCollection<Employee>(mongoSettings.EmployeesCollectionName));
builder.Services.AddSingleton(sp => mongoDatabase.GetCollection<SubContractor>(mongoSettings.SubcontractorCollectionName));

// ✅ Register Configuration Settings
builder.Services.AddSingleton(mongoSettings);
builder.Services.AddSingleton(jwtSettings);
builder.Services.AddSingleton(redisSettings);

// ✅ Register Redis Connection
builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(redisSettings.ConnectionString));

// ✅ Register Services
builder.Services.AddSingleton<EmailService>();
builder.Services.AddSingleton<AuthService>();
builder.Services.AddSingleton<TokenService>();
builder.Services.AddSingleton<EmployeeService>();
builder.Services.AddSingleton<SubContractorService>();
builder.Services.AddSingleton<ProjectService>();
builder.Services.AddSingleton<LocationService>();
builder.Services.AddSingleton<CanteenService>();
builder.Services.AddSingleton<UserService>();
builder.Services.AddSingleton<DeviceService>();
builder.Services.AddSingleton<RedisService>();
 // ✅ Added RedisService
builder.Services.AddSingleton<RoleAccessService>();
builder.Services.AddSingleton<VisitorService>();
// ✅ Add Controllersbuilder.Services.AddSingleton<VisitorService>()
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Aljas Authentication API",
        Version = "v1"
    });
});

// ✅ Enable CORS (Allow All Origins, Methods, and Headers)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAnyOrigin", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// ✅ Configure JWT Authentication
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
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey))
        };
    });

var app = builder.Build();

// ✅ Enable Swagger
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Aljas Authentication API V1");
    c.RoutePrefix = string.Empty;
});

// ✅ Middleware Setup
app.UseCors("AllowAnyOrigin");
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

// ✅ Map Controllers
app.MapControllers();

app.Run();
