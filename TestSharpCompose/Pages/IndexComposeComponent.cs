using BlazorApp2.Utils;
using Microsoft.AspNetCore.Components;
using SharpCompose.Base;

namespace BlazorApp2.Pages;

public class IndexComposeComponent : ComposeComponentBase
{
    [Inject] public NavigationManager NavigationManager { get; set; } = null!;
    [Inject] public HttpClient HttpClient { get; set; } = null!;

    protected override void SetContent()
    {
        var uri = Remember.Get(() => NavigationManager.ToBaseRelativePath(NavigationManager.Uri));
        var globalCounter = Remember.Get(0);

        AppCompose.MainLayout(NavigationManager, uri, () =>
            Div(child: () =>
            {
                RouterCompose.Router(Logger, uri, NavigationManager, path => path switch
                {
                    "" => Home,
                    "counter" => Counter,
                    "counter/global" => () => GlobalCounter(globalCounter),
                    "forecast" => () => Fetch.FetchData(HttpClient),
                    _ => Error
                });
            }));
    }

    private static void Home() =>
        Div(child: () =>
        {
            H1(child: () => Text("Hello from Sharp.Compose"));
            Div(child: () => P(child: () => Text("It's simple sharp.compose application")));
        });

    private static void Counter() =>
        Div(child: () =>
        {
            var counter = Remember.Get(0);

            H1(child: () => Text("Counter"));

            Div(child: () =>
            {
                Button(() => counter.Value++,
                    attr => attr.Class("btn", "btn-primary"),
                    () => Text($"Counter: {counter.Value}"));
            });
        });

    private static void GlobalCounter(ValueRemembered<int> globalCounter) =>
        Div(child: () =>
        {
            H1(child: () => Text("Global Counter"));

            Div(child: () =>
            {
                Button(() => globalCounter.Value++,
                    attr => attr.Class("btn", "btn-primary"),
                    () => Text($"Counter: {globalCounter.Value}"));
            });
        });

    [Compose]
    private static void Error() =>
        P(child: () => Text("Sorry, there's nothing at this address."));
}