using System;
using System.IO;
using CsvHelper.Configuration;

namespace POStatistics
{    
    //Defines the attributes of our customer.
    public class Customer
    {
        public string? OrgNo { get; set; }
        public string? OrgName { get; set; }
        public int Employees { get; set; }
        public string? NaeringCode { get; set; }
        public string? OrgForm { get; set; }
    }
}