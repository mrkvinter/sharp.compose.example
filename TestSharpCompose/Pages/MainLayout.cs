using Microsoft.AspNetCore.Components;
using SharpCompose.Base;

namespace BlazorApp2.Pages;

public static class AppCompose
{
    public static void MainLayout(NavigationManager navigationManager, Remembered.ValueRemembered<string> path, Action content)
    {
        Div(atr => atr.Class("page"), () =>
        {
            NavBar(navigationManager, path);

            Main(atr => atr.Class("container"),
                () => Article(atr => atr.Class("content", "px-4"), content));
        });
    }

    private static void NavBar(NavigationManager navigationManager, Remembered.ValueRemembered<string> path)
    {
        Nav(atr => atr.Class("navbar", "navbar-expand-lg", "navbar-light", "bg-light"), () =>
        {
            Div(atr => atr.Class("container-fluid"), () =>
            {
                A(atr =>
                {
                    atr.Class("navbar-brand");
                    atr.OnClick(() => navigationManager.NavigateTo("", false));
                }, () =>
                {
                    Span(atr => atr.Class("text-primary"), () => Text("Sharp."));

                    Text("Compose");
                });

                Div(atr =>
                {
                    atr.Id("navbarNavAltMarkup");
                    atr.Class("collapse", "navbar-collapse");
                }, () =>
                    Div(atr => atr.Class("navbar-nav"), () =>
                    {
                        NavElement(navigationManager, "Home", "", path.Value);
                        NavElement(navigationManager, "Counter", "counter", path.Value);
                        NavElement(navigationManager, "Forecast", "forecast", path.Value);
                    }));
            });
        });
    }

    private static void NavElement(NavigationManager navigationManager, string label, string path, string currentPath)
    {
        A(atr =>
        {
            atr.Class("nav-link", path == currentPath ? "active" : "");
            atr.OnClick(() => navigationManager.NavigateTo(path, false));
        }, () => Text(label));
    }
}