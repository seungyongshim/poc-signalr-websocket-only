using BlazorApp1.Actors.Counter;
using Microsoft.AspNetCore.Components;
using Proto;

namespace BlazorApp1.Pages;

public partial class Counter
{
    [Inject]
    private IRootContext Context { get; set; }

    private Action<object> Command { get; set; }

    private CounterState State { get; set; }

    protected override void OnInitialized()
    {
        Command = c => Context.Send(new PID(Context.System.Address, nameof(CounterActor)), c);

        Command(new ViewInitialized());

        var a = Context.System.EventStream.Subscribe<CounterState>(s =>
        {
            State = s;
            StateHasChanged();
        });
    }
}
