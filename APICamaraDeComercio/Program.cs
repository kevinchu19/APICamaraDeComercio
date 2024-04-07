

using APICamaraDeComercio.Exceptions;
using APICamaraDeComercio.Models.ApiKey;
using APICamaraDeComercio.Repositories;
using APICamaraDeComercio.Services.ApiKey;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
//Repositorys
builder.Services.AddScoped<FacturacionRepository>();
builder.Services.AddScoped<ComprobanteRepository>();
builder.Services.AddScoped<LoginRepository>();
builder.Services.AddScoped<ClienteRepository>();
builder.Services.AddScoped<SeguimientoFacturacionRepository>();
builder.Services.AddScoped<DeudaRepository>();
builder.Services.AddScoped<VEPRepository>();
builder.Services.AddScoped<BilleteraRepository>();
builder.Services.AddScoped<UsuarioRepository>();

//ApiKeyValidation
builder.Services.AddTransient<IApiKeyValidation, ApiKeyValidation>();
builder.Services.AddScoped<ApiKeyAuthFilter>();
builder.Services.AddHttpContextAccessor();

//Filtro de Excepcion
builder.Services.AddMvc(Options =>
{
    Options.Filters.Add(typeof(ExceptionFilter));
})
               .AddJsonOptions(options =>
               {
                   options.JsonSerializerOptions.PropertyNamingPolicy = null;
                   options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
               });

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var key = builder.Configuration["key"];
builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = false;
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key)),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();
app.UseAuthentication();
app.UseRouting();
app.UseAuthorization();

app.MapControllers();

app.Run();
