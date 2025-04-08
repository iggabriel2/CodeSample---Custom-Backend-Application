using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdTool.Entities.Edit
{


    public class SourcedKeyword
    {
        public int KeywordId { get; set; }

        public string Keyword { get; set; }

        public int? AuthorId { get; set; }

        public bool IsExact { get; set; }

        public bool IsPhrase { get; set; }

        public bool IsBroad { get; set; }

        public bool IsNegativeExact { get; set; }

        public bool IsNegativePhrase { get; set; }

        public decimal? ExactBid { get; set; }

        public decimal? BroadBid { get; set; }

        public decimal? PhraseBid { get; set; }

        public bool IsExactActive { get; set; }

        public bool IsBroadActive { get; set; }

        public bool IsPhraseActive { get; set; }

        public string Notes { get; set; }


    }

}
