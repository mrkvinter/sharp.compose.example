using System.Net.Http.Json;
using SharpCompose.Base;
using SharpCompose.Base.Remember;

namespace BlazorApp2.Pages;

public static class Fetch
{
    [Serializable]
    public class WeatherForecast
    {
        public DateTime Date { get; set; }

        public int TemperatureC { get; set; }

        public string? Summary { get; set; }

        public int TemperatureF => 32 + (int) (TemperatureC / 0.5556);
    }

    private record struct FetchState(WeatherForecast[]? WeatherForecasts, string? OrderedProperty = null,
        bool Descending = false);

    public static void FetchData(HttpClient httpClient)
    {
        var forecasts = Remember.Get(() => new FetchState());
        Remember.LaunchedEffect(async () =>
        {
            await Task.Delay(1000);
            var data = await httpClient.GetFromJsonAsync<WeatherForecast[]>("sample-data/weather.json") ??
                       Array.Empty<WeatherForecast>();
            forecasts.Value = new FetchState(data);
        });

        H1(child: () => Text("Weather forecast"));

        P(child: () => Text("This component demonstrates fetching data from the server."));

        Div(child: () =>
        {
            if (forecasts.Value.WeatherForecasts == null)
                Loading();
            else
                TableFetch(forecasts);
        });
    }

    private static void Loading() =>
        Div(atr =>
        {
            atr.Class("row", "justify-content-center", "align-items-center");
            atr.Style("height: 200px");
        }, () =>
            Div(atr => atr.Class("spinner-border"), () =>
                Span(atr => atr.Class("visually-hidden"), () =>
                    Text("Loading..."))));

    private static void Sort<TKey>(ValueRemembered<FetchState> fetchState, Func<WeatherForecast, TKey> keySelector,
        string orderProperty)
    {
        bool orderBy;
        if (fetchState.Value.OrderedProperty == orderProperty)
            orderBy = !fetchState.Value.Descending;
        else
            orderBy = false;

        var fetchData = orderBy
            ? fetchState.Value.WeatherForecasts.OrderBy(keySelector).ToArray()
            : fetchState.Value.WeatherForecasts.OrderByDescending(keySelector).ToArray();

        fetchState.Value = new FetchState(fetchData, orderProperty, orderBy);
    }

    private static void TableFetch(ValueRemembered<FetchState> fetchState) =>
        Table(b => b.Class("table"), () =>
        {
            Thead(child: () =>
                Tr(child: () =>
                {
                    HeadElement(fetchState, e => e.Date, "Data", 20);
                    HeadElement(fetchState, e => e.TemperatureC, "TemperatureC", 25);
                    HeadElement(fetchState, e => e.TemperatureF, "TemperatureF", 25);
                    HeadElement(fetchState, e => e.Summary, "Summary", 30);
                }));

            Tbody(child: () =>
            {
                foreach (var forecast in fetchState.Value.WeatherForecasts)
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

    private static void HeadElement<TKey>(ValueRemembered<FetchState> fetchState,
        Func<WeatherForecast, TKey> keySelector, string header, int widthPercent) =>
        Th(atr =>
            {
                atr.OnClick(() => Sort(fetchState, keySelector, header));
                atr.Style($"width:{widthPercent}%; cursor:pointer; user-select: none;");
            },
            () =>
            {
                Text(header);

                if (fetchState.Value.OrderedProperty == header)
                    Arrow(fetchState.Value.Descending);
            });

    private static void Arrow(bool isDown)
    {
        Text(isDown ? " 🠕" : " 🠗");
    }
}