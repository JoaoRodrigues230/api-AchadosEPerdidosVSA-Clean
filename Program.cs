using Microsoft.EntityFrameworkCore;
using API_AchadosEPerdidos.Shared.Infrastructure.Data;
using API_AchadosEPerdidos.Shared.Infrastructure.Email;
using API_AchadosEPerdidos.Shared.Infrastructure.Sockets;
using API_AchadosEPerdidos.Shared.Infrastructure.Storage;
using API_AchadosEPerdidos.Shared.Security;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("ProductionCors", policy =>
    {
        policy.WithOrigins(
                "http://localhost:5173",    
                "https://achados-perdidos-six.vercel.app" 
               )
              .AllowAnyMethod()  
              .AllowAnyHeader(); 
    });
});

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

//config MediatR, localizando os handlers do vsla automaticamente
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

builder.Services.AddControllers()
    .AddApplicationPart(typeof(API_AchadosEPerdidos.Controllers.ItemController).Assembly);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//pega seção do email settings do appsettings e atribui os valores a class
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddScoped<EmailService>();

//token autenticação
builder.Services.AddScoped<TokenService>();

//background service do canal socket
builder.Services.AddHostedService<AchadosSocketServer>();

builder.Services.AddScoped<IStorageService, CloudflareR2Service>();

var app = builder.Build();

//if (app.Environment.IsDevelopment())
//{
    app.UseSwagger();
    app.UseSwaggerUI();
//}

if (!app.Environment.IsProduction())
{
    app.UseHttpsRedirection();
}

app.UseRouting();

app.UseCors("ProductionCors");
app.UseAuthorization();

app.MapControllers();

var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
app.Run($"http://*:{port}");