using BlazorApp2.Shared;
using Microsoft.AspNetCore.SignalR;

namespace BlazorApp1.Server;

public class ViewHub : Hub
{
    public ViewHub(IHubContext<ViewHub> hubContext)
    {
        HubContext = hubContext;
    }

    private static readonly string[] Summaries = new[]
       {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    public IHubContext<ViewHub> HubContext { get; }

    public override async Task OnConnectedAsync()
    {
        await base.OnConnectedAsync();

        var a = Context.ConnectionId;

        new Timer(async s => await HubContext.Clients.Clients(a)
                     .SendAsync("Push", Enumerable.Range(1, 20).Select(index => new WeatherForecast
                     {
                         Date = DateTime.Now.AddDays(index),
                         TemperatureC = Random.Shared.Next(-20, 55),
                         Summary = Summaries[Random.Shared.Next(Summaries.Length)]
                     })
                     .ToArray()),
                  null,
                  TimeSpan.FromSeconds(0),
                  TimeSpan.FromSeconds(1));

        await HubContext.Clients.Clients(a).SendAsync("ConnectionId", a);
    }
}
