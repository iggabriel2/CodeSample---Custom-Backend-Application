using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdTool.AzSponsoredProducts.BusinessObjects.Keyword
{
    //public class Data
    //{
    //    public string api { get; set; }
    //    public string function { get; set; }
    //    public string se_type { get; set; }
    //    public string keyword { get; set; }
    //    public string language_name { get; set; }
    //    public int location_code { get; set; }
    //    public int limit { get; set; }
    //    public int depth { get; set; }
    //}

    public class Item
    {
        //public string se_type { get; set; }
        public KeywordData keyword_data { get; set; }
        //public int depth { get; set; }
        public List<string> related_keywords { get; set; }
        public Item()
        {
            related_keywords = new List<string>();
        }
    }

    public class KeywordData
    {
        //public string se_type { get; set; }
        public string keyword { get; set; }
        //public int location_code { get; set; }
        //public string language_code { get; set; }
        //public KeywordInfo keyword_info { get; set; }
        public KeywordData()
        {
            //keyword_info = new KeywordInfo();
        }
    }

    //public class KeywordInfo
    //{
    //    public string se_type { get; set; }
    //    public string last_updated_time { get; set; }
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
        public int? items_count { get; set; }
        public List<Item> items { get; set; }
        public Result()
        {
            items = new List<Item>();
        }
    }

    public class D4AzRelatedKeywordResponse
    {
        //public string version { get; set; }
        //public int? status_code { get; set; }
        //public string status_message { get; set; }
        //public string time { get; set; }
        //public double cost { get; set; }
        public int? tasks_count { get; set; }
        public int? tasks_error { get; set; }
        public Task[] tasks { get; set; }
     
    }

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
            //data = new Data();
            //path = new List<string>();
        }
    }

}
