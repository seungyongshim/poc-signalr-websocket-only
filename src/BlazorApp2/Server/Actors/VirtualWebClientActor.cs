using Proto;

namespace BlazorApp1.Server.Actors;

public class VirtualWebClientActor : IActor
{
    public Task ReceiveAsync(IContext ctx) => ctx.Message switch
    {
        _ => Task.CompletedTask,
    };
}
