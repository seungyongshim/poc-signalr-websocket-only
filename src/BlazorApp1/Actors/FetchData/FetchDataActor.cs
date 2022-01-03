using System.Net.Http.Json;
using Boost.Proto.Actor.BlazorWasm;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.SignalR.Client;
using Proto;

namespace BlazorApp1.Actors.FetchData;

public class FetchDataActor : ReduxActor<FetchDataState>, IActor
{
    public FetchDataActor(FetchDataState state, NavigationManager navigationManager, ILoggerProvider loggerProvider)
        : base(state)
    {
        NavigationManager = navigationManager;
        LoggerProvider = loggerProvider;
    }
    public HubConnection HubConnection { get; private set; }
    public NavigationManager NavigationManager { get; }
    public ILoggerProvider LoggerProvider { get; }

    public Task ReceiveAsync(IContext c) => c.Message switch
    {
        ViewInitialized => ChangeStateAsync(c, s => s),
        Started x => HandleAsync(c, x),
        _ => Task.CompletedTask
    };

    private async Task HandleAsync(IContext c, Started x)
    {
        HubConnection = new HubConnectionBuilder()
            .WithUrl(NavigationManager.ToAbsoluteUri("/viewhub"),
                     option =>
                     {
                         option.SkipNegotiation = true;
                         option.Transports = HttpTransportType.WebSockets;
                     })
            .WithAutomaticReconnect()
            .ConfigureLogging(logging =>
            {
                logging.AddProvider(LoggerProvider);
                logging.SetMinimumLevel(LogLevel.Debug);
            })
            .Build();

        HubConnection.KeepAliveInterval = TimeSpan.FromSeconds(3);
        HubConnection.ServerTimeout = TimeSpan.FromSeconds(5);

        HubConnection.Reconnected += HubConnection_Reconnected;
        HubConnection.Closed += async (error) =>
        {
            await Task.Delay(new Random().Next(0, 5) * 1000);
            await HubConnection.StartAsync();
        };

        HubConnection.On<IEnumerable<WeatherForecast>>("Push", m =>
        {
            ChangeStateAsync(c, s => s with
            {
                Forecasts = m
            });
        });

        HubConnection.On<string>("ConnectionId", m =>
        {
            Console.WriteLine(m);
        });

        await HubConnection.StartAsync();


        
    }

    private async Task HubConnection_Reconnected(string? arg)
    {
        await Console.Out.WriteAsync("Reconnected!!!");
    }
}
