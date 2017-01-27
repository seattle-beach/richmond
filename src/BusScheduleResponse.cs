/*
{\"buses\":
    [{\"shortName\":\"99\",
    \"longName\":\"Belltown Via 1st Ave\", 
    \"eta\":\"-2\", 
    \"status\": \"scheduled\"},]}"
*/

using System.Collections.Generic;

namespace Richmond
{
    public class BusScheduleResponse
    {
        public IEnumerable<Bus> Buses;
    }
}