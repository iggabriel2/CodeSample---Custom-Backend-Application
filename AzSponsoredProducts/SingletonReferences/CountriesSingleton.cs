using AdTool.AzSponsoredProducts.Data;
using AdTool.Entities.AzSp.General;
using Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdTool.AzSponsoredProducts.SingletonReferences
{
    public sealed class CountriesSingleton
    {
        private CountriesSingleton() 
        {
            CountriesList();
        }


        private static readonly object padlock = new object();
        static List<AzApiCountries> _countriesList = new List<AzApiCountries>();



        private static readonly Lazy<CountriesSingleton> lazy = new Lazy<CountriesSingleton>(() => new CountriesSingleton());
        public static CountriesSingleton Instance
        {
            get
            {
                return lazy.Value;
            }
        }



        public static List<AzApiCountries> CountriesList()
        { 
            if (_countriesList == null || _countriesList.Count < 1)
            {
                RetrieveData rd = new RetrieveData();
                _countriesList = rd.GetCountriesSync();
                return _countriesList;
            }
            else
            {
                return _countriesList;
            }
        }

    }
}
