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
        private readonly HttpMessageHandler _httpMessageHandler;
        private readonly IDateProvider _dateProvider;
        private readonly string _foodTruckURI;

        public FoodTruckRepository(IDateProvider dateProvider, HttpMessageHandler httpMessageHandler, string foodTruckURI)
        {
            _dateProvider = dateProvider ?? new DateProvider();
            _httpMessageHandler = httpMessageHandler ?? new HttpClientHandler();
            _foodTruckURI = foodTruckURI ?? System.Environment.GetEnvironmentVariable("FOODTRUCK_PATH");
        }

        public FoodTruckRepository(IDateProvider dateProvider) : this(dateProvider, null, null)
        {}

        public FoodTruckResponse ParseFoodTruckSite(string html)
        {

            var parser = new HtmlParser();
            var document = parser.Parse(html);

            var schedule = document.QuerySelector("body > div.l-canvas.sidebar_right.type_wide.titlebar_default > div.l-main > div > div > section.l-section.wpb_row.height_small > div > div > div > div.wpb_text_column > div > div > div > dl");
            var response  = GetResponseForDay(schedule, GetTargetDayOfWeek());
            return response;
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
            var task = client.GetAsync(_foodTruckURI);
            var result = task.Result;
            return result.Content.ReadAsStringAsync().Result;
        }

        private DayOfWeek GetTargetDayOfWeek()
        {
            DayOfWeek currentDay = _dateProvider.Now().DayOfWeek;
            TimeSpan currentTime = _dateProvider.Now().TimeOfDay;
            DayOfWeek targetDay = currentDay;
            TimeSpan twoPm = new TimeSpan(14,0,0);
            switch (currentDay)
            {
                case DayOfWeek.Monday:
                case DayOfWeek.Tuesday:
                case DayOfWeek.Wednesday:
                case DayOfWeek.Thursday:
                    if (currentTime > twoPm)
                    {
                        targetDay = currentDay + 1;
                    }
                    break;
                case DayOfWeek.Saturday:
                case DayOfWeek.Sunday:
                    targetDay = DayOfWeek.Monday;
                    break;
            }
            return targetDay;
        }

        private FoodTruckResponse GetResponseForDay(IElement schedule, DayOfWeek targetDay)
        {
            var dateIndex = 0;

            while (dateIndex < schedule.Children.Length)
            {
                var date = schedule.Children[dateIndex].Children[0];
                var dayOfWeek = date.Children[0].InnerHtml;
                var trimmedDayOfWeek = dayOfWeek.Substring(0, dayOfWeek.Length - 1);
                var targetDayString = Enum.GetName(typeof(DayOfWeek), targetDay);
                if (trimmedDayOfWeek.Equals(targetDayString))
                {
                    var dayOfMonth = date.Children[1].InnerHtml;
                    var month = date.Children[2].InnerHtml;
                    var year = date.Children[3].InnerHtml;

                    var foodTruckList = schedule.Children[dateIndex + 1].Children[0];

                    IList<FoodTruckResponse.FoodTruck> foodTrucks = foodTruckList.Children.Select(ToFoodTruck).ToList();
                    var dateString = String.Join(" ", new string[] { dayOfWeek, dayOfMonth, month, year });

                    return new FoodTruckResponse { FoodTrucks = foodTrucks, Date = dateString, DayOfWeek = targetDayString };
                }
                dateIndex = dateIndex + 2;
            }
            return null;
        }
    }
}