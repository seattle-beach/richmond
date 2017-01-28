using Richmond.BusClient;
using System;
using System.Collections.Generic;

namespace Richmond.Tests
{
    public class MockBusScheduleClient : IBusScheduleClient
    {
        private readonly List<WeatherBusDatum> buses = new List<WeatherBusDatum>();

        public Exception Exception;

        public void Add(WeatherBusDatum bus)
        {
            this.buses.Add(bus);
        }

        public WeatherBusResult Fetch()
        {
            if (this.Exception != null)
            {
                throw this.Exception;
            }

            return new WeatherBusResult { Data = this.buses };
        }
    }
}