using System.Collections.Generic;
using AngleSharp.Dom;
using AngleSharp.Parser.Html;
using System.Linq;
using System.Net.Http;

namespace Richmond
{

    public interface IFoodTruckRepository
    {

        string RequestFoodTruckWebsite();
        IList<FoodTruckResponse.FoodTruck> ParseFoodTruckSite(string html);
    }

    public class FoodTruckRepository : IFoodTruckRepository
    {
        private const string foodTruckURI = "http://www.seattlefoodtruck.com/schedule/occidental-park-food-truck-pod/";

        private readonly HttpMessageHandler _httpMessageHandler;

        public FoodTruckRepository()
        {
            _httpMessageHandler = new HttpClientHandler();
        }

        public FoodTruckRepository(HttpMessageHandler httpMessageHandler)
        {
            _httpMessageHandler = httpMessageHandler;
        }

        public IList<FoodTruckResponse.FoodTruck> ParseFoodTruckSite(string html)
        {
            var parser = new HtmlParser();
            var document = parser.Parse(html);
            var schedule = document.QuerySelector("body > div.l-canvas.sidebar_right.type_wide.titlebar_default > div.l-main > div > div > section.l-section.wpb_row.height_small > div > div > div > div.wpb_text_column > div > div > div > dl > dd.simcal-weekday-3.simcal-past.simcal-day.simcal-day-has-events.simcal-day-has-1-events.simcal-events-calendar-5736.simcal-day-has-events.simcal-day-has-2-events.simcal-events-calendar-5736.simcal-day-has-events.simcal-day-has-3-events.simcal-events-calendar-5736.simcal-day-has-events.simcal-day-has-4-events.simcal-events-calendar-5736 > ul");

            return schedule.Children
                .Select(ToFoodTruck)
                .ToList();
        }

        private FoodTruckResponse.FoodTruck ToFoodTruck(IElement element)
        {
            var name = element.QuerySelector(".simcal-event-title").InnerHtml;
            var type = element.QuerySelector(".simcal-event-description p").InnerHtml.Split(new[] { "<br>" }, System.StringSplitOptions.None)[0];
            return new FoodTruckResponse.FoodTruck { Name = name, Type = type };
        }

        public string RequestFoodTruckWebsite()
        {
            HttpClient client = new HttpClient(_httpMessageHandler);
            var task = client.GetAsync(foodTruckURI);
            var result = task.Result;
            return result.Content.ReadAsStringAsync().Result;
        }

    }
}