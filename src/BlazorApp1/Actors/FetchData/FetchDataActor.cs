using System.Net.Http.Json;
using Boost.Proto.Actor.BlazorWasm;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.SignalR.Client;
using Proto;

namespace BlazorApp1.Actors.FetchData;

public class FetchDataActor : ReduxActor<FetchDataState>, IActor
{
    public FetchDataActor(FetchDataState state, NavigationManager navigationManager)
        : base(state)
    {
        NavigationManager = navigationManager;
    }
    public HubConnection HubConnection { get; private set; }
    public NavigationManager NavigationManager { get; }

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
            .Build();

        HubConnection.On<IEnumerable<WeatherForecast>>("Push", m =>
        {
            ChangeStateAsync(c, s => s with
            {
                Forecasts = m
            });
        });

        await HubConnection.StartAsync();
    }
}
