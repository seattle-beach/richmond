using System.Collections.Generic;

namespace Richmond
{
    public class BusResponse
    {
        public IEnumerable<Bus> Buses;

        public class Bus
        {
            public string ShortName;
            public string LongName;
            public int Eta;
            public string Status;
        }
    }
}