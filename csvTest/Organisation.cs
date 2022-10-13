using System;
using System.IO;
using Newtonsoft.Json;

namespace POCustomers
{
    public class Organisation
    {
        //Model of the JSON object that catch the information we need.
        public class OrganisationInfo
        {
            public string? organisasjonsnummer { get; set; }
            public string? navn { get; set; }
            public OrgForm? organisasjonsform { get; set; }
            public NaeringCode? naeringskode1 { get; set; }
            public int antallAnsatte { get; set; }
        }
        public class OrgForm
        {
            public string? kode { get; set; }
            public string? beskrivelse { get; set; }
        }

        //Loading 'kode' as string. It can be loaded as a double.
        //This is what I found on 'Naeringskoder': https://www.ssb.no/klass/klassifikasjoner/6
        public class NaeringCode
        {
            public string? beskrivelse { get; set; }
            public string? kode { get; set; }
        }
    }
}