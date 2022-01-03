namespace BlazorApp1.Actors.FetchData;

public record FetchDataState(IEnumerable<WeatherForecast> Forecasts)
{

}

public class WeatherForecast
{
    public DateTime Date { get; set; }

    public int TemperatureC { get; set; }

    public string? Summary { get; set; }

    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
