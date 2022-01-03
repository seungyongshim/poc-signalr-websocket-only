using BlazorApp1;
using BlazorApp1.Actors.Counter;
using BlazorApp1.Actors.FetchData;
using Boost.Proto.Actor.DependencyInjection;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Proto;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddTransient<CounterState>(sp => new CounterState(0));
builder.Services.AddTransient<FetchDataState>(sp => new FetchDataState(Array.Empty<WeatherForecast>()));

builder.Services.AddProtoActorWasm(_ => _, _ => _, (sp, root) =>
{
    root.SpawnNamed(sp.GetService<IPropsFactory<CounterActor>>().Create(), nameof(CounterActor));
    root.SpawnNamed(sp.GetService<IPropsFactory<FetchDataActor>>().Create(), nameof(FetchDataActor));
});



await builder.Build().RunAsync();
