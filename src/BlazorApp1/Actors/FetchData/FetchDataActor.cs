using System.Net.Http.Json;
using Boost.Proto.Actor.BlazorWasm;
using Proto;

namespace BlazorApp1.Actors.FetchData;

public class FetchDataActor : ReduxActor<FetchDataState>, IActor
{
    public FetchDataActor(FetchDataState state, IServiceProvider sp)
        : base(state)
    {
        CreateScope = () => sp.CreateScope();
    }
    public IServiceProvider Services { get; }
    public Func<IServiceScope> CreateScope { get; }

    public Task ReceiveAsync(IContext c) => c.Message switch
    {
        ViewInitialized => ChangeStateAsync(c, s => s),
        Started => ChangeStateAsync(c, async s =>
        {
            using var scope = CreateScope();
            var httpClient = scope.ServiceProvider.GetService<HttpClient>();
            var ret = await httpClient.GetFromJsonAsync<IEnumerable<WeatherForecast>>("sample-data/weather.json");
            return s with { Forecasts = ret };
        }),
        _ => Task.CompletedTask
    };
}
