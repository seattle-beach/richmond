using System.Collections.Generic;

namespace Richmond
{
    public class BusServiceResponse
    {
        public IEnumerable<Bus> Data;

        public class Bus
        {
            public string RouteShortName;
            public string Headsign;
            public ulong PredictedTime;
            public ulong ScheduledTime;
        }
    }
}