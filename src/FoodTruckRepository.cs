using System.Collections.Generic;
using AngleSharp.Dom;
using AngleSharp.Parser.Html;
using Newtonsoft.Json;
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

        public FoodTruckResponse ParseFoodTruckSite(string json)
        {

            dynamic dynObj = JsonConvert.DeserializeObject(json);
            var schedule = dynObj.events;
            var date = GetTargetDate();
            var response  = GetResponseForDay(schedule, date);
            
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
        private string ParseFoodTruckJson(string json)
        {
            dynamic dynObj = JsonConvert.DeserializeObject(json);
            var truck = dynObj.events[0].bookings[0].truck.ToString();
            return truck;
        }
        private DateTime GetTargetDate()
        {
            DateTime targetDate = _dateProvider.Now();
            DayOfWeek currentDay = targetDate.DayOfWeek;
            TimeSpan currentTime = targetDate.TimeOfDay;;
            TimeSpan twoPm = new TimeSpan(14,0,0);
            switch (currentDay)
            {
                case DayOfWeek.Monday:
                case DayOfWeek.Tuesday:
                case DayOfWeek.Wednesday:
                case DayOfWeek.Thursday:
                    if (currentTime > twoPm)
                    {
                        targetDate = targetDate.AddDays(1);
                    }
                    break;
                case DayOfWeek.Saturday:
                    targetDate = targetDate.AddDays(2);
                    break;
                case DayOfWeek.Sunday:
                    targetDate = targetDate.AddDays(1);
                    break;
            }
            return targetDate.Date;
        }

        private FoodTruckResponse GetResponseForDay(Newtonsoft.Json.Linq.JArray events, DateTime targetDate)
        {
            var dateIndex = 0;

            while (dateIndex < events.Count)
            {
                var date = DateTime.Parse(events[dateIndex].SelectToken("start_time").ToString());

                if (date.Date.Equals(targetDate))
                {
                    var dayOfMonth = date.Day.ToString();
                    var month = date.ToString("MMMM");
                    var year = date.Year.ToString();

                    var bookings = events[dateIndex].SelectToken("bookings");
                    IList<FoodTruckResponse.FoodTruck> foodTrucks = new List<FoodTruckResponse.FoodTruck>();
                    foreach (Newtonsoft.Json.Linq.JToken booking in bookings) {
                        var truck = booking.SelectToken("truck");
                        var type = "None";
                        if (truck.SelectToken("food_categories").Count() > 0) {
                            type = truck.SelectToken("food_categories")[0].ToString();
                        }
                        FoodTruckResponse.FoodTruck foodTruck = new FoodTruckResponse.FoodTruck{ Name = truck.SelectToken("name").ToString(), Type = type };
                        foodTrucks.Add(foodTruck);
                    }

                    var dayOfWeek = date.DayOfWeek.ToString();
                    var targetDayString = Enum.GetName(typeof(DayOfWeek), targetDate.DayOfWeek);
                    var dateString = String.Join(" ", new string[] { dayOfWeek + ",", dayOfMonth, month, year });

                    return new FoodTruckResponse { FoodTrucks = foodTrucks, Date = dateString, DayOfWeek = targetDayString };
                }
                dateIndex = dateIndex + 1;
            }
            return null;
        }
    }
}