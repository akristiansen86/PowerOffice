using System;
using System.IO;
using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;

namespace POStatistics
{
    class UIProgram
    {  
        static void Main(string[] args)
        {  
            //Variables
            var customers = new List<Customer>();
            int ENK = 0; 
            int AS0_4 = 0;
            int AS5_10 = 0; 
            int AS10pluss = 0;
            int others = 0;
            decimal ENKPro = 0.0M; 
            decimal AS0_4Pro = 0.0M;
            decimal AS5_10Pro = 0.0M; 
            decimal AS10plussPro = 0.0M;
            decimal othersPro = 0.0M;

            //Reading CSV file with our Customer objects.
            using (var streamReader = new StreamReader(@"po-kunder_more_info_10_8_2022.csv"))
            {
                using(var csvReader = new CsvReader(streamReader, CultureInfo.InvariantCulture))
                {
                    customers = csvReader.GetRecords<Customer>().ToList();
                    
                }
            }
            
            //Checking if customer is ENK, AS or other.
            for(int i = 0; i < customers.Count; i++)
            {
                if(customers[i].OrgForm == "ENK")
                {
                    ENK++;
                }
                else if(customers[i].OrgForm == "AS")
                {
                    //Checking number of employees
                    if(customers[i].Employees <= 4)
                    {
                        AS0_4++;
                    }
                    else if(customers[i].Employees <= 10)
                    {
                        AS5_10++;
                    }
                    else
                    {
                        AS10pluss++;
                    }
                }
                else
                {
                    others++;
                }
            }
            
            //Calculating prosent value of the different organisation forms.
            ENKPro = Math.Round(100*ENK/Convert.ToDecimal(customers.Count), 2, MidpointRounding.ToEven);
            AS0_4Pro = Math.Round(100*AS0_4/Convert.ToDecimal(customers.Count), 2, MidpointRounding.ToEven);
            AS5_10Pro = Math.Round(100*AS5_10/Convert.ToDecimal(customers.Count), 2, MidpointRounding.ToEven);
            AS10plussPro = Math.Round(100*AS10pluss/Convert.ToDecimal(customers.Count), 2, MidpointRounding.ToEven);
            othersPro = Math.Round(100*others/Convert.ToDecimal(customers.Count), 2, MidpointRounding.ToEven);
            
            //Printing to screen
            Console.WriteLine("Statistics");
            Console.WriteLine("__________________________________________________________________________________");
            Console.WriteLine("|       | ENK   | Andre   | AS 0-4 ansatte | AS 5-10 ansatte | AS over 10 ansatte |");
            Console.WriteLine("|_______|_______|_________|________________|_________________|____________________|");
            Console.WriteLine("|Antall | "+ENK+"    | "+others+"      | "+AS0_4+"            | "+AS5_10+"             | "+AS10pluss+"                 |");
            Console.WriteLine("|_______|_______|_________|________________|_________________|____________________|");
            Console.WriteLine("|Prosent| "+ENKPro+"% | "+othersPro+"%   | "+AS0_4Pro+"%         | "+AS5_10Pro+"%          | "+AS10plussPro+"%              |");
            Console.WriteLine("|_______|_______|_________|________________|_________________|____________________|");
            Console.WriteLine();
        }

    } 
}