using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bidding.API.Models
{
    public class DatabaseSettings : IDatabaseSettings
    {
        public string SupplierCollectionName { get; set; }
        public string BuyerCollectionName { get; set; }
        public string UserCollectionName { get; set; }
        public string CategoryCollectionName { get; set; }
        public string ProductCollectionName { get; set; }
        public string CountryCollectionName { get; set; }
        public string StateCollectionName { get; set; }
        public string CityCollectionName { get; set; }
        public string RFQCollectionName { get; set; }
        public string CounterCollectionName { get; set; }
        public string CurrencyCollectionName { get; set; }
        public string OtpCollectionName { get; set; }
        public string UserRoleCollectionName { get; set; }
        public string TermsAndConditionsCollectionName { get; set; }
        public string FeedbackMasterCollectionName { get; set; }
        public string EmailTemplatesName { get; set; }
        public string AxetActionName { get; set; }
        public string AxetNotification { get; set; }
        public string EmailRemindersCollectionName { get; set; }
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
    }

    public interface IDatabaseSettings
    {
        string SupplierCollectionName { get; set; }
        string BuyerCollectionName { get; set; }
        string UserCollectionName { get; set; }
        string CategoryCollectionName { get; set; }
        string ProductCollectionName { get; set; }
        string CountryCollectionName { get; set; }
        string StateCollectionName { get; set; }
        string CityCollectionName { get; set; }
        string RFQCollectionName { get; set; }
        string CounterCollectionName { get; set; }
        string CurrencyCollectionName { get; set; }
        string OtpCollectionName { get; set; }
        string UserRoleCollectionName { get; set; }
        string FeedbackMasterCollectionName { get; set; }
        string TermsAndConditionsCollectionName { get; set; }
        string EmailTemplatesName { get; set; }
        string AxetActionName { get; set; }
        string AxetNotification { get; set; }
        string EmailRemindersCollectionName { get; set; }
        string ConnectionString { get; set; }
        string DatabaseName { get; set; }
    }
}
