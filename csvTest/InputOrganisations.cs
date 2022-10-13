using System;
using System.IO;
using Newtonsoft.Json;
using CsvHelper.Configuration;

namespace POCustomers
{
    //Use the classmap to make the header for our csv file.
    public class CustomerInfoClassMap : ClassMap<Customer>
    {
        public CustomerInfoClassMap()
        {
            Map(c => c.OrgNo).Name("OrgNo");
            Map(c => c.OrgName).Name("OrgName");
            Map(c => c.Employees).Name("Employees");
            Map(c => c.NaeringCode).Name("NaeringCode");
            Map(c => c.OrgForm).Name("OrgForm");
        }
    }
    
    //Defines the attributes of our customer.
    public class Customer
    {
        public string OrgNo, OrgName;
        public int Employees;
        public string NaeringCode, OrgForm;


        //Sets the values we get from the initial csv file. These values are changed after HTTP request.
        public Customer(string CSVrowData)
        {
            string[] customerInfo = CSVrowData.Split(';');     
            this.OrgNo = customerInfo[0];
            this.OrgName = customerInfo[1];
            this.Employees = 0;
            this.NaeringCode = "";
            this.OrgForm = "";  
        }

    }
}