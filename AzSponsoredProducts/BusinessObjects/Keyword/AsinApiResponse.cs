using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdTool.AzSponsoredProducts.BusinessObjects.Keyword
{
    //public class AdBlock
    //{
    //    public string campaign_id { get; set; }
    //    public string brand_logo { get; set; }
    //    public string advertiser_id { get; set; }
    //    public string ad_id { get; set; }
    //    public string link { get; set; }
    //    public string title { get; set; }
    //    public string store_link { get; set; }
    //    public string store_id { get; set; }
    //    public string store_name { get; set; }
    //    public List<Product> products { get; set; }
    //    public AdBlock() { 
    //        products = new List<Product>();
    //    }
    //}

    //public class AmazonGlobalStore
    //{
    //    public string name { get; set; }
    //    public string value { get; set; }
    //    public string link { get; set; }
    //    public string refinement_display_name { get; set; }
    //}

    public class Author
    {
        public string name { get; set; }
        public string link { get; set; }
    }

    //public class Author2
    //{
    //    public string name { get; set; }
    //    public string value { get; set; }
    //    public string link { get; set; }
    //    public string refinement_display_name { get; set; }
    //}

    //public class Availability
    //{
    //    public string raw { get; set; }
    //    public string name { get; set; }
    //    public string value { get; set; }
    //    public string link { get; set; }
    //    public string refinement_display_name { get; set; }
    //}

    //public class Bestseller
    //{
    //    public string link { get; set; }
    //    public string category { get; set; }
    //}

    //public class Brand
    //{
    //    public string name { get; set; }
    //    public string value { get; set; }
    //    public string link { get; set; }
    //    public string refinement_display_name { get; set; }
    //}

    //public class Category
    //{
    //    public string name { get; set; }
    //    public string id { get; set; }
    //}

    //public class CategoryInformation
    //{
    //    public bool is_landing_page { get; set; }
    //}

    //public class Character
    //{
    //    public string name { get; set; }
    //    public string value { get; set; }
    //    public string link { get; set; }
    //    public string refinement_display_name { get; set; }
    //}

    //public class ChildCategory
    //{
    //    public string name { get; set; }
    //    public string value { get; set; }
    //    public string link { get; set; }
    //    public string refinement_display_name { get; set; }
    //}

    //public class Condition
    //{
    //    public string name { get; set; }
    //    public string value { get; set; }
    //    public string link { get; set; }
    //    public string refinement_display_name { get; set; }
    //}

    //public class Coupon
    //{
    //    public string badge_text { get; set; }
    //    public string text { get; set; }
    //}

    //public class Department
    //{
    //    public string name { get; set; }
    //    public string value { get; set; }
    //    public string refinement_display_name { get; set; }
    //    public string link { get; set; }
    //}

    //public class Format
    //{
    //    public string name { get; set; }
    //    public string value { get; set; }
    //    public string link { get; set; }
    //    public string refinement_display_name { get; set; }
    //}

    //public class GiftGuide
    //{
    //    public string link { get; set; }
    //}

    //public class InternationalShipping
    //{
    //    public string name { get; set; }
    //    public string value { get; set; }
    //    public string link { get; set; }
    //    public string refinement_display_name { get; set; }
    //}

    //public class Language
    //{
    //    public string name { get; set; }
    //    public string value { get; set; }
    //    public string link { get; set; }
    //    public string refinement_display_name { get; set; }
    //}

    //public class MoodsAndTheme
    //{
    //    public string name { get; set; }
    //    public string value { get; set; }
    //    public string link { get; set; }
    //    public string refinement_display_name { get; set; }
    //}

    //public class NewRelease
    //{
    //    public string name { get; set; }
    //    public string value { get; set; }
    //    public string link { get; set; }
    //    public string refinement_display_name { get; set; }
    //}

    //public class OtherFormat
    //{
    //    public string title { get; set; }
    //    public string link { get; set; }
    //    public string asin { get; set; }
    //}

    //public class Page
    //{
    //    public DateTime created_at { get; set; }
    //    public DateTime processed_at { get; set; }
    //    public double? total_time_taken { get; set; }
    //    public int? page { get; set; }
    //    public string amazon_url { get; set; }
    //    public int? total_results { get; set; }
    //    public int? current_page { get; set; }
    //    public string next_page_link { get; set; }
    //    public int? total_pages { get; set; }
    //}

    //public class Pagination
    //{
    //    public List<Page> pages { get; set; }
    //    public Pagination()
    //    {
    //        pages = new List<Page>();
    //    }
    //}

    //public class Price
    //{
    //    public string symbol { get; set; }
    //    public string value { get; set; }
    //    public string currency { get; set; }
    //    public string raw { get; set; }
    //    public string name { get; set; }
    //    public bool? is_primary { get; set; }
    //    public bool? is_rrp { get; set; }
    //    public string asin { get; set; }
    //    public string link { get; set; }
    //}

    //public class Price2
    //{
    //    public string symbol { get; set; }
    //    public double? value { get; set; }
    //    public string currency { get; set; }
    //    public string raw { get; set; }
    //    public string name { get; set; }
    //    public bool is_primary { get; set; }
    //    public string asin { get; set; }
    //    public string link { get; set; }
    //    public string refinement_display_name { get; set; }
    //}

    //public class Prime
    //{
    //    public string name { get; set; }
    //    public string value { get; set; }
    //    public string link { get; set; }
    //    public string refinement_display_name { get; set; }
    //}

    //public class Product
    //{
    //    public string asin { get; set; }
    //    public string link { get; set; }
    //    public string image { get; set; }
    //    public string title { get; set; }
    //    public double? rating { get; set; }
    //    public int? ratings_total { get; set; }
    //    public bool is_prime { get; set; }
    //    public Price price { get; set; }
    //    public Product()
    //    {
    //        price = new Price();
    //    }
    //}

    //public class Refinements
    //{
    //    public List<Prime> prime { get; set; }
    //    public List<Department> departments { get; set; }
    //    public List<ChildCategory> child_categories { get; set; }
    //    public List<Review> reviews { get; set; }
    //    public List<Price> price { get; set; }
    //    public List<Brand> brand { get; set; }
    //    public List<NewRelease> new_releases { get; set; }
    //    public List<Format> format { get; set; }
    //    public List<MoodsAndTheme> moods_and_themes { get; set; }
    //    public List<Character> characters { get; set; }
    //    public List<Author> author { get; set; }
    //    public List<Language> language { get; set; }
    //    public List<AmazonGlobalStore> amazon_global_store { get; set; }
    //    public List<InternationalShipping> international_shipping { get; set; }
    //    public List<Condition> condition { get; set; }
    //    public List<Availability> availability { get; set; }
    //    public Refinements()
    //    {
    //        prime = new List<Prime>();
    //        departments = new List<Department>();
    //        child_categories = new List<ChildCategory>();
    //        reviews = new List<Review>();
    //        price = new List<Price>();
    //        brand = new List<Brand>();
    //        new_releases = new List<NewRelease>();
    //        format = new List<Format>();
    //        moods_and_themes = new List<MoodsAndTheme>();
    //        characters = new List<Character>();
    //        author = new List<Author>();
    //        language = new List<Language>();
    //        amazon_global_store = new List<AmazonGlobalStore>();
    //        international_shipping = new List<InternationalShipping>();
    //        condition = new List<Condition>();
    //        availability = new List<Availability>();
    //    }
    //}

    //public class RequestInfo
    //{
    //    public bool success { get; set; }
    //    public int? topup_credits_remaining { get; set; }
    //    public int? credits_used_this_request { get; set; }
    //}

    //public class RequestMetadata
    //{
    //    public DateTime created_at { get; set; }
    //    public DateTime processed_at { get; set; }
    //    public double? total_time_taken { get; set; }
    //    public List<Page> pages { get; set; }
    //    public RequestMetadata()
    //    {
    //        pages = new List<Page>();
    //    }
    //}

    //public class RequestParameters
    //{
    //    public string type { get; set; }
    //    public string amazon_domain { get; set; }
    //    public string search_term { get; set; }
    //    public string category_id { get; set; }
    //    public string exclude_sponsored { get; set; }
    //    public string include_html { get; set; }
    //    public string max_page { get; set; }
    //}

    //public class Review
    //{
    //    public string name { get; set; }
    //    public string value { get; set; }
    //    public string link { get; set; }
    //    public string refinement_display_name { get; set; }
    //}

    public class AsinApiResponse
    {
        //public RequestInfo request_info { get; set; }
        //public RequestParameters request_parameters { get; set; }
        //public RequestMetadata request_metadata { get; set; }
        public List<SearchResult> search_results { get; set; }
        //public CategoryInformation category_information { get; set; }
        //public Pagination pagination { get; set; }
        //public Refinements refinements { get; set; }
        //public List<AdBlock> ad_blocks { get; set; }
        //public List<VideoBlock> video_blocks { get; set; }
        public AsinApiResponse()
        {
            search_results = new List<SearchResult>();
            //category_information = new CategoryInformation();
            //pagination = new Pagination();
            //refinements = new Refinements();
            //ad_blocks = new List<AdBlock>();
            //video_blocks = new List<VideoBlock>();
            //request_info = new RequestInfo();
            //request_parameters = new RequestParameters();
            //request_metadata = new RequestMetadata();
        }
    }

    public class SearchResult
    {
        //public int? position { get; set; }
        public string title { get; set; }
        public string asin { get; set; }
        //public string link { get; set; }
        //public List<Category> categories { get; set; }
        //public string image { get; set; }
        public List<Author>? authors { get; set; }
        //public Bestseller? bestseller { get; set; }
        //public List<OtherFormat>? other_formats { get; set; }
        //public double? rating { get; set; }
        //public int? ratings_total { get; set; }
        //public List<Price>? prices { get; set; }
        //public Price? price { get; set; }
        //public int? page { get; set; }
        //public int? position_overall { get; set; }
        //public GiftGuide gift_guide { get; set; }
        //public bool? is_prime { get; set; }
        //public Coupon coupon { get; set; }
        //public bool? kindle_unlimited { get; set; }
        //public Availability availability { get; set; }
        public SearchResult()
        {
            //availability = new Availability();
            //coupon = new Coupon();
            //gift_guide = new GiftGuide();
            //price = new Price();
            //prices = new List<Price>();
            //other_formats = new List<OtherFormat>();
            //bestseller = new Bestseller();
            authors = new List<Author>();
            //categories = new List<Category>();
        }
    }

    //public class VideoBlock
    //{
    //    public string video_link { get; set; }
    //    public string thumbnail_link { get; set; }
    //    public string campaign_id { get; set; }
    //    public string advertiser_id { get; set; }
    //    public bool has_audio { get; set; }
    //    public List<Product> products { get; set; }
    //    public VideoBlock()
    //    {
    //        products = new List<Product>();
    //    }
    //}

}
