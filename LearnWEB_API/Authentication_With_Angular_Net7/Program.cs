using Authentication_With_Angular_Net7.Configurations;
using Authentication_With_Angular_Net7.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// add config
builder.Services.Configure<JWTConfig>(builder.Configuration.GetSection(key: "JWTConfig"));
// add dbconnection
builder.Services.AddDbContext<ApiDbContext>(options => options.UseSqlite(builder.Configuration.GetConnectionString(name: "DefaultConnection")));
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// add identity( danh tinh)
builder.Services.AddDefaultIdentity<IdentityUser>(option => option.SignIn.RequireConfirmedAccount = false).AddEntityFrameworkStores<ApiDbContext>();
// add authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(jwt =>
    {
        var key = Encoding.ASCII.GetBytes(builder.Configuration.GetSection(key: "JWTConfig:secret").Value);
        jwt.SaveToken = true;
        jwt.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = true,
            ValidateAudience = false,
            RequireExpirationTime = false,
            ValidateLifetime = true
        };
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
// use app authentication
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
