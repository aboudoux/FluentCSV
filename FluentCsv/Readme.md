**

# Fluent CSV 
 A .NET library for read your csv files in a fluent way.
 
## Benefit

 - Written in .NET Standard 2.0
 - Implements  [RFC 4180](https://tools.ietf.org/html/rfc4180)
 - Open source
 - Ease of reading and writing code
 - Detailled parse error reporting

##  Basic usage

Imagine a csv file named "sample1.csv"

    Name;Age
    Smith;43
    Robin;38
    Test;NA

At first, create a POCO class for represent each csv line

    public class CsvData {
	    public string Name {get; set;}
	    public int Age {get; set;}
    }

then read your file using Fluent CSV

     var csvData = Read.Csv.FromFile("sample1.csv")
                .ThatReturns.ArrayOf<CsvData>()
                .Put.Column("name").Into(a => a.Name)
                .Put.Column("age").As<int>().Into(a => a.Age)
                .GetAll();

            Console.WriteLine("CSV DATA");
            csvData.ResultSet.ForEach(r=> Console.WriteLine($"Name : {r.Name} - Age : {r.Age}"));

            Console.WriteLine("ERRORS");
            csvData.Errors.ForEach(e => Console.WriteLine($"Error at line {e.LineNumber} column index {e.ColumnZeroBasedIndex} : {e.ErrorMessage}"));
        
Output

    CSV DATA
    Name : Smith - Age : 43
    Name : Robin - Age : 38
    ERRORS
    Error at line 4 column index 1 : The format of the input string is incorrect.
    
 And VOILA !
## API Structure



### Read.Csv.

> Select a way to read your csv data

| Method| Arguments | Comment |
|--|--|--|
| `FromString`  | [CsvString] (encoding) | Read a csv directly from a string. |
| `FromFile` | [FilePath] (encoding) | Read a csv from a file.
| `FromAssemblyResource` | [ResourceName] (Assembly) | Read a csv from an assembly resource.

### With.

> Define optionnal CSV configuration which can be combined with the `And` keyword

|Method | Arguments  | Comment
|--|--|--|
| `ColumnsDelimiter` | [DelimiterString] |Define the string used  as columns delimiter |
| `EndOfLineDelimiter` | [DelimiterString] | Define the string used as new line delimiter |
| `Header` | (CaseMode) |Declare the first line as header. You can specify if  you treat it as case sensitive or case insensitive when putting columns in properties.   |
| `SimpleParsingMode` | |

### ThatReturns.

> Define what kind of objects to return as resultset and how to populate them.

|Method| Arguments  | Comment |
|--|--|--|
| `ArrayOf<T>` | Type of class that we must use to store csv data | The class must have a parameterless constructor, and contains properties with get; set;

### Put.

> Define how to map each column with a property

|Method| Arguments | Comment |
|--|--|--|
| Column | [Index or HeaderName] | 

##### SubMethods

|Method| Arguments  |
|--|--|
| As  |  |
| InThisWay |
|Into

### GetAll()

> Execute the csv parsing and return two collections

## Some scenarios

### Example A

- A csv file with contact informations
- BirthDate is encoded as MMddyy
- We want to store Firstname,Lastname and Birthdate in a subclass named `Contact`
- We want to store Address, City and ZipCode in an other subclass named `Address`

#### Csv file

    FirstName;LastName;BirthDate;Address;City;ZipCode
    Rosemary;Berube;100483;2435 Leverton Cove Road;Springfield;01103
    Richard;Lewis;121144;1509 Big Elm;64110
    Michael;Lindquist;122139;1334 Clark Street;Chicago;60607

#### POCO

    public class CsvInfos
    {
        public Contact Contact { get; } = new Contact();
        public Address Address { get; } = new Address();
    }
     
    public class Contact
    {
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public DateTime BirthDate { get; set; }
    }

    public class Address
    {
        public string Street { get; set; }
        public string City { get; set; }
        public string ZipCode { get; set; }
    }

#### API Call

 

    var csvData = Read.Csv.FromFile(file)
                    .ThatReturns.ArrayOf<CsvInfos>()
                    .Put.Column("FirstName").Into(p => p.Contact.Firstname)
                    .Put.Column("LastName").Into(p => p.Contact.Lastname)
                    .Put.Column("BirthDate").As<DateTime>()
                    .InThisWay(a => DateTime.ParseExact(a, "MMddyy", CultureInfo.CurrentCulture))
                    .Into(p => p.Contact.BirthDate)
                    .Put.Column("Address").Into(p => p.Address.Street)
                    .Put.Column("City").Into(p => p.Address.City)
                    .Put.Column("ZipCode").Into(p => p.Address.ZipCode)
                    .GetAll();

---

### Example B
- A csv file where columns are separed with '-' and lines end with '#'.
- There are two columns where headers have same name (but not same case)
- Some data have a double quote, and we want to get it

#### Csv file

    NAME-name#Smith-Bob#Rob"in-Wiliam

#### POCO

    public class CsvName {
	    public string LastName { get; set; }
	    public string FirstName { get; set; }
	}
#### API Call

    var result = Read.Csv.FromFile("example.csv")
                .With.EndOfLineDelimiter("#")
                .And.ColumnsDelimiter("-")
                .And.Header(As.CaseSensitive)
                .And.SimpleParsingMode()
                .ThatReturns.ArrayOf<CsvName>()
                .Put.Column("NAME").Into(a => a.FirstName)
                .Put.Column("name").Into(a => a.LastName)
                .GetAll();
---
### Example C

- A csv file without header that contains informations with validation rules and structured data
- The file is a csv generated by excel, with columns separed by tabulation
- First column is an Id, second a phone number, third an enumeration and fourth a multiline address
- The id is a number
- Phone numbers that not contains 10 digits are not valid
- The enumeration is a set of letters (A, B or F) separed by pipes character '|'. All other letters are invalid

#### Csv file

    1	0309545655	A	"1344 Wright Court
    Birmingham, AL 35209"
    2	0988767454	A|B	"2253 Jadewood Drive
    South Bend, IN 46625"
    A	0166245163	B|F	"2920 Hood Avenue
    San Diego, CA 92103"
    4	0562435562	F	"3066 Maple Avenue
    Coeur D Alene, ID 83814"
    5	0261726323	A|C	"2992 Victoria Court
    Pittsfield, ME 04967"
    6	0493928372	F|B|A	"667 Anthony Avenue
    Abilene, TX 79601"
    7	046768778A	A|F	"2438 Logan Lane
    Lakewood, CO 80214"
    8	0799878757	B|A	"4105 Asylum Avenue
    Bridgeport, CT 06604"
    9	0805546766	B	"252 Hinkle Deegan Lake Road
    Lexington, KY 40505"
    10	0487876876	F	"1081 Brookview Drive
    Beaumont, TX 77701"

#### POCO

    public class LineExampleC
    {
	    public int Id { get; set; }
        public PhoneNumber PhoneNumber { get; set; }
        public CustomEnum CustomEnum { get; set; }
        public string Address { get; set; }
    
        public override string ToString()
            => $"id = {Id}, Phone = {PhoneNumber}, Enum = {CustomEnum}, Address = {Address}";
    }

    public class PhoneNumber
    {
        private readonly string _phoneNumber;

        public PhoneNumber(string phone)
        {
            if(!Regex.IsMatch(phone,"[0-9]{10}"))
                throw new ArgumentException("Phone number is invalid");

            _phoneNumber = phone;
        }

        public override string ToString()
            => _phoneNumber;
    }

    public class CustomEnum
    {
        private List<string> _enumList;

        public CustomEnum(string @enum)
        {
            _enumList = @enum.Split('|').ToList();
            if(!_enumList.TrueForAll(a => a == "A" || a == "B" || a == "F"))
                throw new ArgumentException("Invalid enum character");
        }

        public override string ToString()
            => string.Join(" and ", _enumList);
    }

#### API Call

    var csvData = Read.Csv.FromFile(file)
                    .With.ColumnsDelimiter("\t")
                    .ThatReturns.ArrayOf<LineExampleC>()
                    .Put.Column(0).As<int>().Into(a => a.Id)
                    .Put.Column(1).As<PhoneNumber>().InThisWay(s => new PhoneNumber(s)).Into(a => a.PhoneNumber)
                    .Put.Column(2).As<CustomEnum>().InThisWay(s => new CustomEnum(s)).Into(a => a.CustomEnum)
                    .Put.Column(3).Into(a => a.Address)
                    .GetAll();
    
                Console.WriteLine("CSV DATA");
                csvData.ResultSet.ForEach(Console.WriteLine);
    
                Console.WriteLine("ERRORS");
                csvData.Errors.ForEach(e => Console.WriteLine($"Error at line {e.LineNumber} column index {e.ColumnZeroBasedIndex} : {e.ErrorMessage}"));

#### Output

    CSV DATA
    id = 1, Phone = 0309545655, Enum = A, Address = 1344 Wright Court
    Birmingham, AL 35209
    id = 2, Phone = 0988767454, Enum = A and B, Address = 2253 Jadewood Drive
    South Bend, IN 46625
    id = 4, Phone = 0562435562, Enum = F, Address = 3066 Maple Avenue
    Coeur D Alene, ID 83814
    id = 6, Phone = 0493928372, Enum = F and B and A, Address = 667 Anthony Avenue
    Abilene, TX 79601
    id = 8, Phone = 0799878757, Enum = B and A, Address = 4105 Asylum Avenue
    Bridgeport, CT 06604
    id = 9, Phone = 0805546766, Enum = B, Address = 252 Hinkle Deegan Lake Road
    Lexington, KY 40505
    id = 10, Phone = 0487876876, Enum = F, Address = 1081 Brookview Drive
    Beaumont, TX 77701
    
    ERRORS
    Error at line 3 column index 0 : The format of the input string is incorrect.
    Error at line 5 column index 2 : Invalid enum character
    Error at line 7 column index 1 : Phone number is invalid

# Licensing
FluentCsv is free for use in all your product, including commercial software.