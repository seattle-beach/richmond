using System;
using System.Collections.Generic;

namespace Richmond.Tests
{
    public class MockBusScheduleClient : IBusScheduleClient
    {
        private readonly List<Bus> buses = new List<Bus>();

        public Exception Exception;

        public void Add(Bus bus)
        {
            this.buses.Add(bus);
        }

        IEnumerable<Bus> IBusScheduleClient.Fetch()
        {
            if (this.Exception != null)
            {
                throw this.Exception;
            }
            
            return this.buses;
        }
    }
}