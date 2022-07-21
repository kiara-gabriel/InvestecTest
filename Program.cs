using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NewsArticles
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var newsArticles = GetNewsArticles();

            List<GroupedArticles> groupedArticles = new List<GroupedArticles>();
            var gropedBySite = newsArticles.GroupBy(x => x.NewsSite)
                                           .ToList();

            groupedArticles.ForEach(x => groupedArticles.Add(new GroupedArticles
            {
                NewsSite = x.NewsSite,
                Month = x.Month,
                Year = x.Year,
                Count = 
            }));

            List<IGrouping<int, Article>> groupedByMonth = new List<IGrouping<int, Article>>();
            foreach (var group in gropedBySite)
            {
                groupedByMonth = group.GroupBy(x => x.Month).ToList();
                groupedArticles.AddRange(new GroupedArticles
                {
                    NewsSite = group.Key,
                    Month = 
                });
            }



        }

        private static List<Article> GetNewsArticles()
        {
            List<Article> articles = new List<Article>();

            var client = new RestClient("https://api.spaceflightnewsapi.net/v3/articles?_limit=100");
            var request = new RestRequest();
            var response = client.Get(request);
            var result = JsonConvert.DeserializeObject<List<RawData>>(response.Content);

            result.ForEach(x => articles.Add(Article.Map(x.newsSite, x.publishedAt)));

            return articles;
        }
    }

    public class RawData
    {
        public string newsSite { get; set; }
        public string publishedAt { get; set; }
    }

    public class Article
    {
        public string NewsSite { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public static Article Map(string newsSite, string publishedAt)
        {
            var date = publishedAt.Split('-');
            var year = Convert.ToInt32(date[0]);
            var month = Convert.ToInt32(date[1]);

            return new Article
            {
                NewsSite = newsSite,
                Month = month,
                Year = year
            };

        }

    }

    public class GroupedArticles
    {
        public string NewsSite { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public int Count { get; set; }
    }
}
