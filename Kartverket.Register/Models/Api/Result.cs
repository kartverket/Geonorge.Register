using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Kartverket.Register.Models.Api
{
    public class Result
    {

        public Result(FilterParameters filter, int count)
        {
            if (filter != null)
            {
                Offset = filter.Offset;
                Limit = filter.Limit;
            }
            Count = count;
        }

        public Result()
        {
        }


        [DataMember]
        public int Count { get; set; }
        [DataMemberAttribute]
        public int Offset { get; set; }
        [DataMemberAttribute]
        public int Limit { get; set; }
        [DataMemberAttribute]
        public int Total { get; set; }
    }
}