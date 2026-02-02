using Challenge.Application.Commands;
using Challenge.Application.Handlers;
using Challenge.Application.Services;
using Challenge.Domain.Bus;
using Challenge.Domain.Interfaces;
using Challenge.Domain.Notifications;
using Challenge.Infra;
using Challenge.Infra.Cache;
using Challenge.Infra.CrossCutting;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.ResponseCompression;
using System.Reflection;

namespace Challenge.Api.Config;

public class NativeInjectorBootStrapper
{
    public static void RegisterServices(IServiceCollection services, IConfiguration config, IWebHostEnvironment webHostEnvironment)
    {
        // Domain Bus (Mediator)
        services.AddSingleton<IMediatorHandler, InMemoryBus>();

        // Domain - Commands
        services.AddSingleton<IRequestHandler<InsertNewsCache, Result<bool>>, NewsCacheHandler>();
        services.AddSingleton<IRequestHandler<DeleteNewsCache, Result<bool>>, NewsCacheHandler>();

        // Domain - Events
        services.AddSingleton<INotificationHandler<DomainNotification>, DomainNotificationHandler>();

        //Application
        services.AddSingleton<INewsService, NewsService>();

        //Infra
        services.AddSingleton<INewsCache, NewsCache>();

        //DragonflyDB settings
        services.Configure<ConnectionDragonflyDB>(config.GetSection(nameof(ConnectionDragonflyDB)));

        //SignalR
        services.AddSignalR();
        services.AddResponseCompression(opts =>
        {
            opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Append("application/octet-stream");
        });

        // Asp .NET HttpContext dependency
        services.AddHttpContextAccessor();

        // Mediator
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly());
        });

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
