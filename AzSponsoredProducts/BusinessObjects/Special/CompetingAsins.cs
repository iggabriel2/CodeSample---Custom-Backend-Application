using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdTool.AzSponsoredProducts.BusinessObjects.Special.Compete
{
    public class AmazonPaid
    {
        public int? pos_1 { get; set; }
        public int? pos_2_3 { get; set; }
        public int? pos_4_10 { get; set; }
        public int? pos_11_100 { get; set; }
        public int? count { get; set; }
        public int? search_volume { get; set; }
    }

    public class AmazonSerp
    {
        public int? pos_1 { get; set; }
        public int? pos_2_3 { get; set; }
        public int? pos_4_10 { get; set; }
        public int? pos_11_100 { get; set; }
        public int? count { get; set; }
        public int? search_volume { get; set; }
    }

    public class CompetitorMetrics
    {
        public AmazonSerp amazon_serp { get; set; }
        public AmazonPaid amazon_paid { get; set; }
        public CompetitorMetrics()
        {
            amazon_serp = new AmazonSerp();
            amazon_paid = new AmazonPaid();
        }
    }

    public class Data
    {
        public string api { get; set; }
        public string function { get; set; }
        public string se_type { get; set; }
        public string asin { get; set; }
        public int? location_code { get; set; }
        public string language_name { get; set; }
        public int? limit { get; set; }
    }

    public class FullMetrics
    {
        public AmazonSerp amazon_serp { get; set; }
        public AmazonPaid amazon_paid { get; set; }
        public FullMetrics()
        {
            amazon_serp = new AmazonSerp();
            amazon_paid = new AmazonPaid();
        }
    }

    public class Item
    {
        public string se_type { get; set; }
        public string asin { get; set; }
        public double avg_position { get; set; }
        public int? sum_position { get; set; }
        public int? intersections { get; set; }
        public CompetitorMetrics competitor_metrics { get; set; }
        public FullMetrics full_metrics { get; set; }
        public Item()
        {
            competitor_metrics = new CompetitorMetrics();
            full_metrics = new FullMetrics();
        }
    }

    public class Result
    {
        public string se_type { get; set; }
        public string asin { get; set; }
        public int? location_code { get; set; }
        public string language_code { get; set; }
        public int? total_count { get; set; }
        public int? items_count { get; set; }
        public List<Item> items { get; set; }
        public Result()
        {
            items = new List<Item>();
        }
    }

    public class CompetingAsins
    {
        public string version { get; set; }
        public int? status_code { get; set; }
        public string status_message { get; set; }
        public string time { get; set; }
        public decimal cost { get; set; }
        public int? tasks_count { get; set; }
        public int? tasks_error { get; set; }
        public List<Task> tasks { get; set; }
        public CompetingAsins()
        {
            tasks = new List<Task>();
        }
    }

    public class Task
    {
        public string id { get; set; }
        public int? status_code { get; set; }
        public string status_message { get; set; }
        public string time { get; set; }
        public decimal cost { get; set; }
        public int? result_count { get; set; }
        public List<string> path { get; set; }
        public Data data { get; set; }
        public List<Result> result { get; set; }
        public Task()
        {
            data =  new Data();
            result = new List<Result>();
            path = new List<string>();
        }
    }


}
