using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using SharpCompose.Base;
using SharpCompose.Base.Composers;

namespace BlazorApp2.Pages;

public abstract class ComposeComponentBase : ComponentBase
{
    [Inject] public ILogger<ComposeComponentBase> Logger { get; set; } = null!;

    protected sealed override void OnInitialized()
    {
        base.OnInitialized();

        Logger.Log(LogLevel.Information, "OnInitialized");

        Composer.Instance.RecomposeEvent += async () =>
        {
            while (Composer.Instance.Composing)
            {
                await Task.Yield();
            }
            await SetParametersAsync(ParameterView.Empty);
        };
    }

    protected sealed override void BuildRenderTree(RenderTreeBuilder builder)
    {
        Logger.Log(LogLevel.Warning, "Compose");

        Composer.RootComposer(SetContent);

        var composer = (RenderTreeComposer) Composer.Instance;
        composer.Build(builder);
    }

    protected abstract void SetContent();
}