using Microsoft.EntityFrameworkCore;
using API_AchadosEPerdidos.Shared.Infrastructure.Data;
using API_AchadosEPerdidos.Shared.Infrastructure.Email;
using API_AchadosEPerdidos.Shared.Infrastructure.Sockets;
using API_AchadosEPerdidos.Shared.Security;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

//config MediatR, localizando os handlers do vsla automaticamente
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//pega seção do email settings do appsettings e atribui os valores a class
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddScoped<EmailService>();

//token autenticação
builder.Services.AddScoped<TokenService>();

//background service do canal socket
builder.Services.AddHostedService<NexusSocketServer>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();