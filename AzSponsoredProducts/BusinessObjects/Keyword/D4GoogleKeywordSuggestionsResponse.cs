using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdTool.AzSponsoredProducts.BusinessObjects.Keyword.Google
{
    //public class AvgBacklinksInfo
    //{
    //    public string se_type { get; set; }
    //    public double? backlinks { get; set; }
    //    public double? dofollow { get; set; }
    //    public double? referring_pages { get; set; }
    //    public double? referring_domains { get; set; }
    //    public double? referring_main_domains { get; set; }
    //    public double? rank { get; set; }
    //    public double? main_domain_rank { get; set; }
    //    public string last_updated_time { get; set; }
    //}

    //public class Data
    //{
    //    public string api { get; set; }
    //    public string function { get; set; }
    //    public string se_type { get; set; }
    //    public string keyword { get; set; }
    //    public string language_name { get; set; }
    //    public int? location_code { get; set; }
    //    public int? limit { get; set; }
    //}

    //public class ImpressionsInfo
    //{
    //    public string se_type { get; set; }
    //    public string last_updated_time { get; set; }
    //    public int? bid { get; set; }
    //    public string match_type { get; set; }
    //    public double? ad_position_min { get; set; }
    //    public int? ad_position_max { get; set; }
    //    public double? ad_position_average { get; set; }
    //    public double? cpc_min { get; set; }
    //    public double? cpc_max { get; set; }
    //    public double? cpc_average { get; set; }
    //    public double? daily_impressions_min { get; set; }
    //    public double? daily_impressions_max { get; set; }
    //    public double? daily_impressions_average { get; set; }
    //    public double? daily_clicks_min { get; set; }
    //    public double? daily_clicks_max { get; set; }
    //    public double? daily_clicks_average { get; set; }
    //    public double? daily_cost_min { get; set; }
    //    public double? daily_cost_max { get; set; }
    //    public double? daily_cost_average { get; set; }
    //}

    public class Item
    {
        //public string se_type { get; set; }
        public string keyword { get; set; }
        //public int? location_code { get; set; }
        //public string language_code { get; set; }
        //public KeywordInfo keyword_info { get; set; }
        //public KeywordProperties keyword_properties { get; set; }
        //public ImpressionsInfo impressions_info { get; set; }
        //public object serp_info { get; set; }
        //public AvgBacklinksInfo avg_backlinks_info { get; set; }
        //public SearchIntentInfo search_intent_info { get; set; }
        public Item()
        {
            //avg_backlinks_info = new AvgBacklinksInfo();
            //search_intent_info = new SearchIntentInfo();
            //impressions_info = new ImpressionsInfo();
            //keyword_properties = new KeywordProperties();
            //keyword_info = new KeywordInfo();
        }
    }

    //public class KeywordInfo
    //{
    //    public string se_type { get; set; }
    //    public string last_updated_time { get; set; }
    //    public double? competition { get; set; }
    //    public string competition_level { get; set; }
    //    public double? cpc { get; set; }
    //    public int? search_volume { get; set; }
    //    public double? low_top_of_page_bid { get; set; }
    //    public double? high_top_of_page_bid { get; set; }
    //    public List<int> categories { get; set; }
    //    public List<MonthlySearch> monthly_searches { get; set; }
    //    public KeywordInfo() { 
    //        monthly_searches = new List<MonthlySearch>();
    //        categories = new List<int>();
    //    }
    //}

    //public class KeywordProperties
    //{
    //    public string se_type { get; set; }
    //    public string core_keyword { get; set; }
    //    public int? keyword_difficulty { get; set; }
    //    public string detected_language { get; set; }
    //    public bool is_another_language { get; set; }
    //}

    //public class MonthlySearch
    //{
    //    public int? year { get; set; }
    //    public int? month { get; set; }
    //    public int? search_volume { get; set; }
    //}

    public class Result
    {
        //public string se_type { get; set; }
        //public string seed_keyword { get; set; }
        //public object seed_keyword_data { get; set; }
        //public int? location_code { get; set; }
        //public string language_code { get; set; }
        public int? total_count { get; set; }
        //public int? items_count { get; set; }
        //public int? offset { get; set; }
        //public string offset_token { get; set; }
        public List<Item> items { get; set; }
        public Result()
        {
            items = new List<Item>();
        }
    }

    public class D4GoogleKeywordSuggestionsResponse
    {
        //public string version { get; set; }
        //public int? status_code { get; set; }
        //public string status_message { get; set; }
        //public string time { get; set; }
        //public double? cost { get; set; }
        public int? tasks_count { get; set; }
        public int? tasks_error { get; set; }
        public List<Task> tasks { get; set; }
        public D4GoogleKeywordSuggestionsResponse()
        {
            tasks = new List<Task>();
        }
    }

    //public class SearchIntentInfo
    //{
    //    public string se_type { get; set; }
    //    public string main_intent { get; set; }
    //    public List<string> foreign_intent { get; set; }
    //    public string last_updated_time { get; set; }
    //    public SearchIntentInfo()
    //    {
    //        foreign_intent = new List<string>();
    //    }
    //}

    public class Task
    {
        //public string id { get; set; }
        //public int? status_code { get; set; }
        //public string status_message { get; set; }
        //public string time { get; set; }
        //public double? cost { get; set; }
        //public int? result_count { get; set; }
        //public List<string> path { get; set; }
        //public Data data { get; set; }
        public List<Result> result { get; set; }
        public Task()
        {
            result = new List<Result>();
            //path = new List<string>();
            //data = new Data();
        }
    }


}
