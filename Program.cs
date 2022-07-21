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
            var allGroupedArticles = new GroupedArticles();
            var dtFormat = new System.Globalization.DateTimeFormatInfo();

            var newsArticles = GetNewsArticles();
            var groupedByDate = GroupByDate(newsArticles);

            groupedByDate.ForEach(x => allGroupedArticles.groupedArticleItems
                                                         .AddRange
                                                         (
                                                            GroupByNewsSite(x.NewsSite, x.Month, x.Year)
                                                         ));

            foreach (var groupedArticle in allGroupedArticles.groupedArticleItems)
            {
                Console.WriteLine($"{dtFormat.GetMonthName(groupedArticle.Month)} {groupedArticle.Year} {groupedArticle.NewsSite} {groupedArticle.Count}");
            }
        }

        private static List<Article> GetNewsArticles()
        {
            var client = new RestClient("https://api.spaceflightnewsapi.net/v3/articles?_limit=100");
            var request = new RestRequest();
            var response = client.Get(request);
            var result = JsonConvert.DeserializeObject<List<Article>>(response.Content);

            return result;
        }

        private static List<NewsSitePerMonth> GroupByDate(List<Article> articles)
        {
            var result = articles.GroupBy(x => new { x.PublishedAt.Year, x.PublishedAt.Month })
                                 .Select(y => new NewsSitePerMonth
                                 {
                                     Month = y.Key.Month,
                                     Year = y.Key.Year,
                                     NewsSite = y.Select(x => x.NewsSite).ToList()
                                 }).ToList();

            return result;
        }

        private static List<GroupedArticleItem> GroupByNewsSite(List<string> newsSite, int month, int year)
        {
            var result = newsSite.GroupBy(x => x)
                                 .Select(y => new GroupedArticleItem
                                 {
                                     NewsSite = y.Key,
                                     Year = year,
                                     Month = month,
                                     Count = y.Count()
                                 }).ToList();

            return result;
        }
    }

    public class Article
    {
        public string NewsSite { get; set; }
        public DateTime PublishedAt { get; set; }
    }
    public class NewsSitePerMonth
    {
        public List<string> NewsSite { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
    }

    public class GroupedArticleItem
    {
        public string NewsSite { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public int Count { get; set; }
    }

    public class GroupedArticles
    {
        public List<GroupedArticleItem> groupedArticleItems { get; set; }

        public GroupedArticles()
        {
            groupedArticleItems = new List<GroupedArticleItem>();
        }
    }

}
