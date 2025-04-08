using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace AdTool.AzSponsoredProducts.Utils
{
    public class GeneralUtils
    {
        public async Task<string> CapName(string rawName)
        {
            string combinedName = "";

            string name = rawName.Replace(".", "");

            if (string.IsNullOrEmpty(name))
            {
                return string.Empty;
            }

            string[] words = name.Split(' ');

            foreach (var word in words)
            {
                try
                {
                    //this should never happen, but we'll leave it anyway in case we go back to it
                    if (word.Contains("."))
                    {
                        string[] innerWords = word.Split('.');

                        foreach (var innerWord in innerWords)
                        {
                            if (innerWord != "" && innerWord != " ")
                            {
                                combinedName += innerWord[0].ToString().ToUpper() + innerWord.Substring(1) + " ";
                            }
                        }
                    }
                    else
                    {
                        combinedName += word[0].ToString().ToUpper() + word.Substring(1) + " ";
                    }
                }
                catch(Exception ex) 
                {
                    combinedName += word[0].ToString().ToUpper() + word.Substring(1) + " ";
                }
              
            }

            return combinedName.Trim();
        }
    }
}
