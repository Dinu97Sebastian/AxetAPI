using System;
using System.Collections.Generic;
using System.Text;

namespace Bidding.API.Models
{
   public class MongoQueryPredicate
    {
        public string Name { get; set; }
        public string Operator { get; set; }
        public object Value { get; set; }

        public MongoQueryPredicate(string name, object value,string operation)
        {
            Name = name;
            Value = value;
            Operator = operation;
        }

        public override string ToString()
        {
            if (this.Value is int)
                return String.Format(@" ""{0}"" : {{ ""{1}"":{2} }} ", this.Name, GetOperation(this.Operator), this.Value);

            return String.Format(@" ""{0}"" : {{ ""{1}"":""{2}"" }} ", this.Name, GetOperation(this.Operator), this.Value);

        }

        private string GetOperation(string key)
        {
            string operation = string.Empty;
            if (string.IsNullOrWhiteSpace(key))
                key = "E";
            switch (key.ToUpper())
            {
                case "E":
                    operation= "$eq";
                    break;
                case "NE":
                    operation = "$ne";
                    break;
                case "GT":
                    operation = "$gt";
                    break;
                case "GTE":
                    operation = "$gte";
                    break;
                case "LT":
                    operation = "$lt";
                    break;
                case "LTE":
                    operation = "$lte";
                    break;
                default:
                    operation = "$eq";
                    break;
            }
            return operation;
        }
    }
}
