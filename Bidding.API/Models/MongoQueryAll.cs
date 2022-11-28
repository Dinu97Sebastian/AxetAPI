using System;
using System.Collections.Generic;
using System.Text;

namespace Bidding.API.Models
{
    public class MongoQueryAll
    {
        public string Name { get; set; }
        public List<MongoQueryElement> QueryElements { get; set; }

        public MongoQueryAll(string name)
        {
            Name = name;
            QueryElements = new List<MongoQueryElement>();
        }

        public override string ToString()
        {
            string qelems = string.Empty;
            foreach (var qe in QueryElements)
                qelems = qelems + qe + ",";
            return String.Format(@"{{ ""{0}"" : {{ $all : [ {1} ] }} }}", this.Name, qelems);
        }
    }

}
