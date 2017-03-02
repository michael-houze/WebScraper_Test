using ScrapySharp.Extensions;
using ScrapySharp.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;

namespace SurvivorStats_Scraper
{
    class Program
    {
        static void Main(string[] args)
        {
            const string SURVIVOR_BASE_URI = "https://survivors.kentuckyderby.com/survivors/";
            const int LOW_RANGE = 2613;
            const int HIGH_RANGE = 2877;

            var browser = new ScrapingBrowser();
            browser.AllowAutoRedirect = true;
            browser.AllowMetaRedirect = true;

            List<KeyValuePair<string, string>> votingResults = new List<KeyValuePair<string, string>>();

            for(int id = LOW_RANGE; id <= HIGH_RANGE; id++)
            {
                string survivor_uri = string.Format($"{SURVIVOR_BASE_URI}{id.ToString()}/");

                try
                {
                    var pageResult = browser.NavigateToPage(new Uri(survivor_uri));

                    if (pageResult.RawResponse.StatusCode == 200)
                    {
                        var votesNode = pageResult.Html.CssSelect(".votes").First();
                        var firstNameNode = pageResult.Html.CssSelect(".first-name").First();
                        var lastNameNode = pageResult.Html.CssSelect(".last-name").First();

                        var votes = votesNode.InnerText;
                        var votesParsed = Regex.Match(votesNode.InnerText, @"\d+").Value.ToString();
                        var lastName = lastNameNode.InnerText;
                        var firstName = firstNameNode.InnerText;
                        var fullName = string.Format($"{lastName.ToUpper().TrimEnd()}, {firstName.ToUpper().TrimEnd()}");

                        votingResults.Add(new KeyValuePair<string, string>(fullName, votesParsed));

                    }
                }
                catch(Exception ex)
                {
                    Console.WriteLine(string.Format($"{survivor_uri} is not a valid URL"));
                }
                
                Thread.Sleep(100);
            }

            var sortedResults = votingResults.OrderByDescending(result => int.Parse(result.Value));
            int place = 1;
            foreach(var kvp in sortedResults)
            {
                Console.WriteLine($"{place.ToString()}. {kvp.Key} {kvp.Value}");
                place++;
            }
            
            Console.ReadLine();
        }
    }
}
