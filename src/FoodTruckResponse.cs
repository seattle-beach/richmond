using System.Collections.Generic;

namespace Richmond
{
    public class FoodTruckResponse
    {
        public struct FoodTruck
        {
            public string Name { get; set; }
            public string Type { get; set; }


            public override string ToString()
            {
                return $"<FoodTruck Name={Name}, Type={Type}>";
            }
        }

        public IList<FoodTruck> FoodTrucks { get; set; }

        public string Date { get; set; }

        public string DayOfWeek { get; set; }
    }
}