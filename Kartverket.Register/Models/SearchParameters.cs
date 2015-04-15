using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kartverket.Register.Models
{
    public class SearchParameters
    {
        public SearchParameters()
        {
            Offset = 1;
            Limit = 50;
        }

        public string Owner { get; set; }
        public string Register { get; set; }
        public string Text { get; set; }
        public int Limit { get; set; }
        public int Offset { get; set; }
        public string OrderBy { get; set; }


    }
}