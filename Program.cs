using System.Net.Http.Json;
using System.Text.Json;

namespace CustomWeatherClientTool
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            await GetCurrentWeather();
        }

        private static async Task GetCurrentWeather()
        {
            Console.Clear();
            Console.WriteLine("Choose an option:");
            Console.WriteLine("1) Type any Indian city name to get the current weather.");
            Console.WriteLine("2) Type Exit");
            Console.Write("\r\nSelect an option: ");
            string cityOption = Console.ReadLine();

            StreamReader r = new("in.json");
            string json = r.ReadToEnd();
            List<CityDetail> items = JsonSerializer.Deserialize<List<CityDetail>>(json);

            if (cityOption.ToUpper() != "EXIT")
            {
                CityDetail cityDetails = items.Where(x => x.city.ToUpper().Contains(cityOption.ToUpper())).FirstOrDefault();

                if (cityDetails != null)
                {
                    Console.WriteLine("City selected by you is:" + cityDetails.city);
                    Console.WriteLine("Please wait while we are fetching the weather details:\n");

                    using (HttpClient client = new())
                    {
                        var customWeather =
                        await client.GetFromJsonAsync<CustomWeather>("https://api.open-meteo.com/v1/forecast?latitude=" + cityDetails.lat + "&longitude=" + cityDetails.lng + "&current_weather=true");

                        if (customWeather != null)
                        {
                            Console.WriteLine("Current Weather of : " + cityDetails.city);
                            Console.WriteLine("===========================================");

                            Console.WriteLine("Temperature: " + customWeather.current_weather.temperature);
                            Console.WriteLine("Wind Speed: " + customWeather.current_weather.windspeed);
                            Console.WriteLine("Wind Direction: " + customWeather.current_weather.winddirection);
                        }
                    }
                }
                else
                {
                    Console.WriteLine("\nCould not find any city named \""+ cityOption + "\" in our database.");
                }
                Console.WriteLine("\nDo you want to continue? (Y/N): ");

                string isContinue = Console.ReadLine();

                if (isContinue.ToUpper() == "Y")
                    await GetCurrentWeather();
                else
                    return;
            }
            else
                return;
        }

        public class CustomWeather
        {
            public decimal latitude { get; set; }
            public decimal longitude { get; set; }
            public decimal generationtime_ms { get; set; }
            public int utc_offset_seconds { get; set; }
            public string timezone { get; set; }
            public string timezone_abbreviation { get; set; }
            public decimal elevation { get; set; }
            public CurrentWeather current_weather { get; set; }

        }

        public class CurrentWeather
        {
            public decimal temperature { get; set; }
            public decimal windspeed { get; set; }
            public decimal winddirection { get; set; }
            public int weathercode { get; set; }
            public string time { get; set; }

        }

        public class CityDetail
        {
            public string city { get; set; }
            public string lat { get; set; }
            public string lng { get; set; }
            public string country { get; set; }
            public string admin_name { get; set; }
            public string capital { get; set; }
            public string population { get; set; }
            public string population_proper { get; set; }
        }


    }
}