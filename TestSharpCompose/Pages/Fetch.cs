using System.Dynamic;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using SharpCompose.Base;

namespace BlazorApp2.Pages;

public static class Fetch
{
    public class WeatherForecast
    {
        public DateTime Date { get; set; }

        public int TemperatureC { get; set; }

        public string? Summary { get; set; }

        public int TemperatureF => 32 + (int) (TemperatureC / 0.5556);
    }

    // private WeatherForecast[]? forecasts;

    // [Inject] public HttpClient HttpClient { get; set; } = default!;
    //
    // protected override void BuildRenderTree(RenderTreeBuilder builder)
    // {
    //     // RenderTreeBuilderContext.Start(builder);
    //     FetchData();
    // }
    //
    // protected override async Task OnInitializedAsync()
    // {
    //     await Task.Delay(1000);
    //     forecasts = await HttpClient.GetFromJsonAsync<WeatherForecast[]>("sample-data/weather.json");
    //     StateHasChanged();
    // }

    public static void FetchData(HttpClient httpClient)
    {
        var forecasts = Remember.Get(Array.Empty<WeatherForecast>);
        Remember.LaunchedEffect(async () =>
        {
            forecasts.Value = await httpClient.GetFromJsonAsync<WeatherForecast[]>("sample-data/weather.json") ??
                              Array.Empty<WeatherForecast>();
        });
        H1(child: () => Text("Weather forecast"));

        P(child: () => Text("This component demonstrates fetching data from the server."));

        if (forecasts.Value.Length == 0)
            Loading();
        else
            TableFetch(forecasts);
    }

    private static void Loading()
    {
        P(child: () => { Em(child: () => Text("Loading...")); });
    }

    private static void TableFetch(Remembered.ValueRemembered<WeatherForecast[]> forecasts)
    {
        Table(b => b.Class("table"), () =>
        {
            Thead(child: () =>
            {
                Tr(child: () =>
                {
                    Th(child: () => Text("Date"));
                    Th(child: () => Text("Temp. (C)"));
                    Th(child: () => Text("Temp. (F)"));
                    Th(child: () => Text("Summary"));
                });
            });

            Tbody(child: () =>
            {
                foreach (var forecast in forecasts.Value)
                {
                    Tr(child: () =>
                    {
                        Td(child: () => Text(forecast.Date.ToShortDateString()));
                        Td(child: () => Text(forecast.TemperatureC.ToString()));
                        Td(child: () => Text(forecast.TemperatureF.ToString()));
                        Td(child: () => Text(forecast.Summary ?? ""));
                    });
                }
            });
        });
    }
}