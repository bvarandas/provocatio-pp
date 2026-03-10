using Challenge.Api.Config;
using Challenge.Api.Controllers;
using Challenge.Infra.Client;
using Challenge.Infra.CrossCutting.Hubs;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Hack News API", Version = "v1" });
});

var config = new ConfigurationBuilder()
.SetBasePath(Directory.GetCurrentDirectory())
    //.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables()
    .Build();

var apiUrl = builder.Configuration["ApiUrl"] ?? throw new Exception("ApiUrl is not set");

builder.Services.AddHttpClient<HackNewsClient>(client => client.BaseAddress = new Uri(apiUrl));

IWebHostEnvironment webHostEnvironment = builder.Environment;

NativeInjectorBootStrapper.RegisterServices(builder.Services, config, webHostEnvironment);

var app = builder.Build();

app.UseResponseCompression();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "HackNews API V1");
        c.RoutePrefix = "";
    });
}

//app.UseStatusCodePages(async statusCodeContext
//    => await Results.Problem(statusCode: statusCodeContext.HttpContext.Response.StatusCode)
//                 .ExecuteAsync(statusCodeContext.HttpContext));

NewsMap.ExposeMaps(app);

app.UseCors("CorsPolicy");
app.MapHub<BrokerHub>("/hubs/brokerhub");
app.MapControllers();
app.Run();