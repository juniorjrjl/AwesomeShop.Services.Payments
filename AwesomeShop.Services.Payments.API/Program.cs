using AwesomeShop.Services.Payments.API;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

//builder.Services.AddConsulConfig(builder.Configuration);
builder.Services.AddControllers();
builder.Services.AddPaymentGateway();
builder.Services.AddSubscribers();
builder.Services.AddMongo();
builder.Services.AddRepositories();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerGen(c => c.SwaggerDoc("v1", new OpenApiInfo { Title= "AwesomeShop.Services.Payments.API", Version = "v1" }));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "AwesomeShop.Services.Payments.API v1"));
}

app.UseHttpsRedirection();

app.UseRouting();

app.MapControllers();

app.Run();
