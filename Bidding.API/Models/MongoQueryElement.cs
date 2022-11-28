using System;
using System.Collections.Generic;
using System.Text;

namespace Bidding.API.Models
{
    public class MongoQueryElement
    {
        public List<MongoQueryPredicate> QueryPredicates { get; set; }

        public MongoQueryElement()
        {
            QueryPredicates = new List<MongoQueryPredicate>();
        }

        public override string ToString()
        {
            string predicates = "";
            foreach (var qp in QueryPredicates)
            {
                predicates = predicates + qp.ToString() + ",";
            }

            return String.Format(@"{{ ""$elemMatch"" : {{ {0} }} }}", predicates);
        }
    }
}
