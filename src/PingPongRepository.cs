using System;
using System.Collections.Generic;
using AngleSharp.Parser.Html;

namespace Richmond
{
    public interface IPingPongRepository
    {
        List<string> ParsePingPongWebsite(string html);
        string RequestPingPongWebsite();
    }

    public class PingPongRepository : IPingPongRepository
    {
        private const string pingPongLadderURI = "http://seattle-pong.cfapps.io/";

        public List<string> ParsePingPongWebsite(string html)
        {
            var parser = new HtmlParser();
            var document = parser.Parse(html);

            List<string> response = new List<string>();
            var rankings = document.QuerySelector("#page > div.rankings > ol");
            foreach (var ranking in rankings.Children)
            {
                var name = ranking.QuerySelector("a").TextContent.Replace("\n","");
                response.Add(name);
            }

            return response;
        }

        public string RequestPingPongWebsite()
        {
            throw new NotImplementedException();
        }
    }
}