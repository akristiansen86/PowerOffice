using System;
using System.IO;
using System.Globalization;
using Newtonsoft.Json;
using CsvHelper;

namespace POCustomers
{
    class Program
    {  
        HttpClient client = new HttpClient();

        static async Task Main(string[] args)
        {   
            //Create file and returns the filename of the current Error-log.
            string filename1 = "Error-log";
            string filetype1 = "txt";
            string logfile = CreateFile(filename1, filetype1);
            
            //Reading all lines from inputfile. Using our Customer class to 
            //create Customer objects wi can work with.
            string[] csvLines = File.ReadAllLines(@"po-kunder.csv");
            var customers = new List<Customer>();

            //Start on line 1 to remove headers.
            for (int i = 1; i < csvLines.Length; i++)
            {
                Customer customer = new Customer(csvLines[i]);
                customers.Add(customer);
            }
        
            Program program = new Program();           
            
            //Possible threading!
            for (int i = 0; i < customers.Count; i++)
            {   
                string url = CreateURL(customers[i].OrgNo);
                var response = await program.client.GetAsync(url);
                if((int)response.StatusCode == 200)
                {
                    var responseContent = response.Content.ReadAsStringAsync().Result;
                    var orgData = JsonConvert.DeserializeObject<Organisation.OrganisationInfo>(responseContent);
                    
                    //Check if orgData is null and write warning to Error-log.
                    if (orgData == null)
                    {
                        string err_msg = "Warning! Status kode 200SUCCESS given, but HTTP response empty for Customer (OrgNo, OrgName): "+ customers[i].OrgNo +", "+ customers[i].OrgName;
                        LoggingError(logfile, err_msg);
                    }
                    else
                    {
                        if (orgData.navn == null)
                        {
                            string err_msg = "Warning! Customer name is missing in response for Customer (OrgNo, OrgName): "+ customers[i].OrgNo +", "+ customers[i].OrgName;
                            LoggingError(logfile, err_msg);    
                        }
                        else
                        {
                            //Update customer name if they are different, ignoring case! 
                            if (!customers[i].OrgName.Equals(orgData.navn, StringComparison.InvariantCultureIgnoreCase))
                            {
                                //Writing to logfile if customer name is updated.                                
                                string err_msg = "Update! Name has been updated. Old Customer (OrgNo, OrgName): "+ customers[i].OrgNo +", "+ customers[i].OrgName +". New Customer (OrgNo, OrgName): "+ orgData.organisasjonsnummer +", "+ orgData.navn;
                                LoggingError(logfile, err_msg);
                                customers[i].OrgName = orgData.navn;
                            }
                        }
                        
                        //Loading number of employees inn customer object.
                        customers[i].Employees = orgData.antallAnsatte;

                        if(orgData.naeringskode1 == null)
                        {   
                            //Writing to logfile if naeringskode is missing from respons.                            
                            string err_msg = "Warning! Naeringskode is empty for Customer (OrgNo, OrgName): "+ customers[i].OrgNo +", "+ customers[i].OrgName;
                            LoggingError(logfile, err_msg);
                        }
                        else
                        {
                            if (orgData.naeringskode1.kode == null)
                            {
                                //Writing to logfile if naeringskode.kode is missing from respons.                                
                                string err_msg = "Warning! Customer naeringskode is missing in response for Customer (OrgNo, OrgName): "+ customers[i].OrgNo +", "+ customers[i].OrgName;
                                LoggingError(logfile, err_msg); 
                            }
                            else
                            {
                                //Loading naeringskode to customer object.
                                customers[i].NaeringCode = orgData.naeringskode1.kode;
                            }
                        }
                        if(orgData.organisasjonsform == null)
                        {   
                            //Writing to logfile if organisasjonsform is missing from respons.
                            string err_msg = "Warning! Organisationform not given for Customer (OrgNo, OrgName): "+ customers[i].OrgNo +", "+ customers[i].OrgName;
                            LoggingError(logfile, err_msg);
                        }
                        else
                        {
                            if (orgData.organisasjonsform.kode == null)
                            {
                                //Writing to logfile if organisasjonsform.kode is missing from respons.
                                string err_msg = "Warning! Customer organisationform is missing in response for Customer (OrgNo, OrgName): "+ customers[i].OrgNo +", "+ customers[i].OrgName;
                                LoggingError(logfile, err_msg); 
                            }
                            else
                            {
                                //Loading organisasjonsform to customer object.
                                customers[i].OrgForm = orgData.organisasjonsform.kode;
                            }
                        }
                    }   
                }
                else
                {
                    //Writing Statuscode to logfile if 404 or 410 is given.
                    string err_msg = $"Error! Status Code {(int)response.StatusCode} {response.StatusCode}. For Customer (OrgNo, OrgName): "+ customers[i].OrgNo +", "+ customers[i].OrgName;
                    LoggingError(logfile, err_msg);
                }
            }
            
            //Create file and returns the filename of the current csv-file.
            string filename2 = "po-kunder_more_info";
            string filetype2 = "csv";
            string poCustomers_moreInfo = CreateFile(filename2, filetype2);
            CreateCSV(poCustomers_moreInfo, customers);
        }

        //Create csv from the list of customers.
        static void CreateCSV(string filename, List<Customer> customerList)
        {   
            using (var streamWriter = new StreamWriter(filename))
            {
                using(var csvWriter = new CsvWriter(streamWriter, CultureInfo.InvariantCulture))
                {
                    csvWriter.Context.RegisterClassMap<CustomerInfoClassMap>();
                    csvWriter.WriteRecords(customerList);
                }
            }
            
        }

        //Create the URL for requesting data on customer.
        public static string CreateURL(string OrganizationNumber)
        {
            string url = $"https://data.brreg.no/enhetsregisteret/api/enheter/{ OrganizationNumber }";
            return url;
        }

        //Create a file of certain extention and return the file's filename.
        static string CreateFile(string fileName, string fileType)
        {
            var date = DateTime.Now;
            string filename = @""+fileName+"_"+date.Month+"_"+date.Day+"_"+date.Year+"."+fileType;
            if (!File.Exists(filename))
            {
                FileInfo file = new FileInfo(filename);
                StreamWriter stenographer = file.CreateText();
                stenographer.Close();
            }
            return filename;
        }

        //Writes error message to file. This is time consuming if many errors occure
        //Can be optimised by systemizing the Flush() command.
        static void LoggingError(string filename, string err_msg)
        {
            StreamWriter stenographer = new StreamWriter(filename, true);
            stenographer.WriteLine(err_msg);
            stenographer.Flush();
            stenographer.Close(); 
        }       
    }
}