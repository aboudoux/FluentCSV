using System;
using System.Text.RegularExpressions;

namespace FluentCsv.Tests.Results
{
    public class ResultWithDeepValueObject
    {
        public Contact Contact { get; set; } = new Contact();
    }

    public class Contact
    {
        public string Name { get; set; }
        public Phone Phone { get; set; }
    }

    public class Phone
    {
        private string _phone;


        public Phone(string phone)
        {
            if(!Regex.IsMatch(phone, "[0-9]{10}"))
                throw new ArgumentException($"{phone} is not a valid phone number");
            _phone = phone;
        }

        public override string ToString()
        {
            return _phone;
        }
    }
}