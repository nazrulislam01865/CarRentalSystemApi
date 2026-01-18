//using BLL.Services;
//using DAL;
//using DAL.EF;
//using DAL.Repos;
//using Microsoft.EntityFrameworkCore;

//var builder = WebApplication.CreateBuilder(args);

//// Add services to the container.

//builder.Services.AddControllers();
//// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();
//builder.Services.AddScoped<UserService>();
//builder.Services.AddScoped<DataAccessFactory>();
//builder.Services.AddScoped<UserRepo>();
//builder.Services.AddScoped<CarRepo>();
//builder.Services.AddScoped<BookingRepo>();
//builder.Services.AddScoped<PaymentRepo>();
//builder.Services.AddScoped<CarService>();
//builder.Services.AddScoped<PaymentService>();
//builder.Services.AddScoped<BookingService>();
//builder.Services.AddDbContext<UMSContext>(opt => {
//    opt.UseSqlServer(builder.Configuration.GetConnectionString("DbConn"));
//});

//var app = builder.Build();

//// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}

//app.UseHttpsRedirection();

//app.UseAuthorization();

//app.MapControllers();

//app.Run();


using BLL.Services;
using DAL;
using DAL.EF;
using DAL.Repos;
using Microsoft.EntityFrameworkCore;

// ✅ ADDED (for Basic Auth + Swagger auth UI)
using AppLayer.Auth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ✅ ADDED (keep AddSwaggerGen() unchanged, configure it separately)
builder.Services.Configure<SwaggerGenOptions>(c =>
{
    c.AddSecurityDefinition("basic", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "basic",
        In = ParameterLocation.Header,
        Description = "Basic Authorization header using the Basic scheme (email:password)."
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "basic"
                }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<DataAccessFactory>();
builder.Services.AddScoped<UserRepo>();
builder.Services.AddScoped<CarRepo>();
builder.Services.AddScoped<BookingRepo>();
builder.Services.AddScoped<PaymentRepo>();
builder.Services.AddScoped<CarService>();
builder.Services.AddScoped<PaymentService>();
builder.Services.AddScoped<BookingService>();
builder.Services.AddDbContext<UMSContext>(opt => {
    opt.UseSqlServer(builder.Configuration.GetConnectionString("DbConn"));
});

// ✅ ADDED (Authentication + Authorization policy)
builder.Services.AddAuthentication("Basic")
    .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("Basic", null);

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOrStaff", policy => policy.RequireRole("Admin", "Staff"));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// ✅ ADDED (must be before UseAuthorization)
app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
