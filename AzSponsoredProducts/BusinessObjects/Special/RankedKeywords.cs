using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdTool.AzSponsoredProducts.BusinessObjects.Special
{
    public class Data
    {
        public string api { get; set; }
        public string function { get; set; }
        public string se_type { get; set; }
        public string asin { get; set; }
        public int location_code { get; set; }
        public string language_name { get; set; }
        public int limit { get; set; }
    }

    public class Item
    {
        public string se_type { get; set; }
        public KeywordData keyword_data { get; set; }
        public RankedSerpElement ranked_serp_element { get; set; }
        public Item()
        {
            keyword_data = new KeywordData();
            ranked_serp_element = new RankedSerpElement();
        }
    }

    public class KeywordData
    {
        public string se_type { get; set; }
        public string keyword { get; set; }
        public int location_code { get; set; }
        public string language_code { get; set; }
        public KeywordInfo keyword_info { get; set; }
        public KeywordData()
        {
            keyword_info = new KeywordInfo();
        }
    }

    public class KeywordInfo
    {
        public string se_type { get; set; }
        public string last_updated_time { get; set; }
        public int search_volume { get; set; }
    }

    public class RankedSerpElement
    {
        public string se_type { get; set; }
        public SerpItem serp_item { get; set; }
        public string check_url { get; set; }
        public List<string> serp_item_types { get; set; }
        public int? se_results_count { get; set; }
        public string last_updated_time { get; set; }
        public object previous_updated_time { get; set; }
        public RankedSerpElement()
        {
            serp_item_types = new List<string>();
        }
    }

    public class Rating
    {
        public string rating_type { get; set; }
        public int? value { get; set; }
        public int? votes_count { get; set; }
        public int? rating_max { get; set; }
    }

    public class Result
    {
        public string se_type { get; set; }
        public string asin { get; set; }
        public int location_code { get; set; }
        public string language_code { get; set; }
        public int? total_count { get; set; }
        public int? items_count { get; set; }
        public List<Item> items { get; set; }
        public Result()
        {
            items = new List<Item>();
        }
    }

    public class RankedKeywords
    {
        public string version { get; set; }
        public int status_code { get; set; }
        public string status_message { get; set; }
        public string time { get; set; }
        public decimal? cost { get; set; }
        public int? tasks_count { get; set; }
        public int? tasks_error { get; set; }
        public List<Task> tasks { get; set; }
        public RankedKeywords()
        {
            tasks = new List<Task>();
        }
    }

    public class SerpItem
    {
        public string se_type { get; set; }
        public string type { get; set; }
        public int? rank_group { get; set; }
        public int? rank_absolute { get; set; }
        public string position { get; set; }
        public string xpath { get; set; }
        public string domain { get; set; }
        public string title { get; set; }
        public string url { get; set; }
        public string asin { get; set; }
        public object image_url { get; set; }
        public decimal? price_from { get; set; }
        public decimal? price_to { get; set; }
        public string currency { get; set; }
        public object special_offers { get; set; }
        public bool is_best_seller { get; set; }
        public bool is_amazon_choice { get; set; }
        public Rating rating { get; set; }
        public object delivery_info { get; set; }
        public SerpItem()
        {
            rating = new Rating();
        }
    }

    public class Task
    {
        public string id { get; set; }
        public int status_code { get; set; }
        public string status_message { get; set; }
        public string time { get; set; }
        public decimal cost { get; set; }
        public int result_count { get; set; }
        public List<string> path { get; set; }
        public Data data { get; set; }
        public List<Result> result { get; set; }
        public Task()
        {
            path = new List<string>();
            data = new Data();
            result = new List<Result>();
        }
    }


}
