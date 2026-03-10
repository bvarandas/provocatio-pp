using Challenge.Application.Handlers;
using Challenge.Application.Interfaces;
using Challenge.Application.Services;
using Challenge.Domain.Interfaces;
using Challenge.Infra.Cache;

using Microsoft.AspNetCore.ResponseCompression;

namespace Challenge.Api.Config;

public class NativeInjectorBootStrapper
{
    public static void RegisterServices(IServiceCollection services, IConfiguration config, IWebHostEnvironment webHostEnvironment)
    {
        // Domain - Commands
        services.AddSingleton<INewsCacheHandler, NewsCacheHandler>();
        //services.AddSingleton<IRequestHandler<DeleteNewsCache, Result<bool>>, NewsCacheHandler>();

        //Application
        services.AddSingleton<INewsService, NewsService>();

        //Infra
        services.AddSingleton<INewsCache, NewsCache>();

        //SignalR
        services.AddSignalR();
        services.AddResponseCompression(opts =>
        {
            opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Append("application/octet-stream");
        });

        // Asp .NET HttpContext dependency
        services.AddHttpContextAccessor();

        services.AddCors(options => options.AddPolicy("CorsPolicy", builderc =>
        {
            builderc
            .AllowAnyHeader()
            .AllowAnyMethod()
            .WithOrigins("http://locallhost:4200")
            .AllowCredentials();
            //.SetIsOriginAllowed((host) => true)

        }));

        services.AddHostedService<MessageReceiverWorker>();
        services.AddHostedService<InitialLoadHackNewsWorker>();
    }
}
