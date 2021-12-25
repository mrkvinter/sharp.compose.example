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

        AppCompose.MainLayout(NavigationManager, uri, () =>
            Div(child: () =>
            {
                RouterCompose.Router(Logger, uri, NavigationManager, path => path switch
                {
                    "" => Home,
                    "counter" => Counter,
                    // "forecast" => () => Fetch.FetchData(HttpClient),
                    _ => Error
                });
            }));
    }

    private static void Home() =>
        Div(child: () =>
        {
            H1(child: () => Text("Hello from Compose#"));
            Div(child: () => P(child: () => Text("It's simple sharp.compose application")));
        });

    private static void Counter() =>
        Div(child: () =>
        {
            var counter = Remember.Get(0);

            H1(child: () => Text("Counter"));

            Div(child: () =>
            {
                Button(attributes: attr =>
                    {
                        attr.Class("btn", "btn-primary");
                        attr.OnClick(() => counter.Value++);
                    },
                    child: () => Text($"Counter: {counter.Value}"));
            });
        });

    private static void CounterSimple() =>
        Div(child: () =>
        {
            var counter = Remember.Get(0);
            Text($"Current count: {counter.Value}");

            Button(() => counter.Value++, child: () => Text("+"));
            Button(() => counter.Value--, child: () => Text("-"));
        });

    [Compose]
    private static void Error() =>
        P(child: () => Text("Sorry, there's nothing at this address."));
}