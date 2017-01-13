using System.Collections.Generic;
using AngleSharp.Dom;
using AngleSharp.Parser.Html;
using System.Linq;
using System.Net.Http;
using System;

namespace Richmond
{

    public interface IFoodTruckRepository
    {

        string RequestFoodTruckWebsite();
        FoodTruckResponse ParseFoodTruckSite(string html);
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

        public FoodTruckResponse ParseFoodTruckSite(string html)
        {

            var parser = new HtmlParser();
            var document = parser.Parse(html);

            var schedule = document.QuerySelector("body > div.l-canvas.sidebar_right.type_wide.titlebar_default > div.l-main > div > div > section.l-section.wpb_row.height_small > div > div > div > div.wpb_text_column > div > div > div > dl");

            var date = schedule.Children[2].Children[0];
            var dayOfWeek = date.Children[0].InnerHtml;
            var dayOfMonth = date.Children[1].InnerHtml;
            var month = date.Children[2].InnerHtml;
            var year = date.Children[3].InnerHtml;

            var foodTruckList = schedule.Children[3].Children[0];

            IList<FoodTruckResponse.FoodTruck> foodTrucks = foodTruckList.Children.Select(ToFoodTruck).ToList();
            var dateString = String.Join(" ", new string[] { dayOfWeek, dayOfMonth, month, year });

            return new FoodTruckResponse { FoodTrucks = foodTrucks, Date = dateString };
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