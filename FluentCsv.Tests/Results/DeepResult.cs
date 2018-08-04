using System;

namespace FluentCsv.Tests.Results
{
    public class DeepResult
    {
        public ContactResult Contact { get; set; } = new ContactResult();
        public AddressResult Address { get; set; } 
    }

    public class ContactResult
    {
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public int Age { get; set; }
        public DateTime BirthDate { get; set; }

        public ContactInfos Infos { get; set; } = new ContactInfos();
    }

    public class ContactInfos
    {
        public string Comment { get; set; }
    }

    public class AddressResult
    {
        public string City { get; set; }
        public int RoadNumber { get; set; }
        public string ZipCode { get; set; }
    }
}