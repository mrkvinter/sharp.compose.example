using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using SharpCompose.Base;
using SharpCompose.Base.Remember;

namespace BlazorApp2.Utils;

public static class RouterCompose
{
    public static void Router(ILogger logger, ValueRemembered<string> path,
        NavigationManager navigationManager,
        Func<string, Action> child)
    {
        Remember.LaunchedEffect(() => navigationManager.LocationChanged += Route);
        Remember.DisposeEffect(() => navigationManager.LocationChanged -= Route);

        void Route(object? sender, LocationChangedEventArgs args)
        {
            logger.Log(LogLevel.Information, $"Route changed to {args.Location}");
            path.Value = navigationManager.ToBaseRelativePath(args.Location);
        }

        child(path.Value)();
    }
}