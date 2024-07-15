/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Core;

public static class AppDictionary
{
    public static class Common
    {
        public const string WeatherHttpClient = "WeatherHttpClient";
    }

    public static class WeatherForecast
    {
        public const string CustomerId = "WeatherForecastId";
        public const string CustomerID = "WeatherForecastID";
        public const string Date = "Date";
        public const string Temperature = "Temperature";
        public const string Summary = "Summary";
        public const string InvoiceItemFilterByInvoiceSpecification = "InvoiceItemFilterByInvoiceSpecification";

        public const string WeatherForecastListAPIUrl = "/API/WeatherForecast/GetItems";
        public const string WeatherForecastItemAPIUrl = "/API/WeatherForecast/GetItem";
        public const string WeatherForecastCommandAPIUrl = "/API/WeatherForecast/Command";
    }
}
