using BlazorApp1.Actors.FetchData;
using Microsoft.AspNetCore.Components;
using Proto;

namespace BlazorApp1.Pages;

public partial class FetchData
{
    [Inject]
    private IRootContext Context { get; set; }

    private Action<object> Command { get; set; }

    private FetchDataState State { get; set; }

    protected override void OnInitialized()
    {
        Command = c => Context.Send(new PID(Context.System.Address, nameof(FetchDataActor)), c);

        Command(new ViewInitialized());

        var a = Context.System.EventStream.Subscribe<FetchDataState>(s =>
        {
            State = s;
            StateHasChanged();
        });
    }
}
