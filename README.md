

# Fluent CSV 
 A .NET library for read your csv files in a fluent way.
 
## Benefits

 - Written in .NET Standard (can be used in all your .NET projects including .NET Core, Xamarin, Mono, and other...)
 - Implements  [RFC 4180](https://tools.ietf.org/html/rfc4180)
 - Free and Open source
 - Ease of reading and writing code (don't just do the right thing, but also says the right thing)
 - Automatically generates an error row report, customizable with your validation rules

## What's new ?
FluentCSV **3.0** is now available with a big performance improvement!

- The speed of the `Rfc4180` parser is multiplied by **8**.
- The `SimpleParsingMode` gains **20%** of performance.

Thus, with this new version, you can read a CSV file of 1 000 000 lines in just a few seconds!

[See more details here](https://github.com/aboudoux/FluentCSV/blob/master/Benchmark/README.MD).

## Adding FluentCSV to your project

You can use NuGet to quickly add FluentCSV to your project. Just search for 'FluentCSV' and install the package, or type the following command into 'Package Manager Console'

`PM> Install-Package FluentCsv`
 
##  Basic usage

Imagine a csv file named "sample1.csv"

    Name;Age
    Smith;43
    Robin;38
    Test;NA

This is how to read your file using Fluent CSV

```c#
var csv = Read.Csv.FromFile("sample1.csv")
	.ThatReturns.ArrayOf<(string Name, int Age)>()
	.Put.Column("name").Into(a => a.Name)
	.Put.Column("age").As<int>().Into(a => a.Age)
	.GetAll();

Console.WriteLine("CSV DATA");
csv.ResultSet.ForEach(r=> Console.WriteLine($"Name : {r.Name} - Age : {r.Age}"));

Console.WriteLine("ERRORS");
csv.Errors.ForEach(e => Console.WriteLine($"Error at line {e.LineNumber} column index {e.ColumnZeroBasedIndex} : {e.ErrorMessage}"));
```
        
Output

    CSV DATA
    Name : Smith - Age : 43
    Name : Robin - Age : 38
    ERRORS
    Error at line 4 column index 1 : The format of the input string is incorrect.
    
 And VOILA !
## API Structure

### Read.Csv.

> Choose the encoding of your data.
> This method is optional.
> Default encoding is UTF8

| Method| Argument(s) | 
|--|--|
| EncodedIn | [[Encoding]](https://docs.microsoft.com/en-gb/dotnet/api/system.text.encoding?view=netcore-3.1) |
 
> Select a way to read your csv data.
> You can choose only one of the following methods

| Method| Argument(s) | Comment |
|--|--|--|
| FromString  | [CsvString] | Read a csv directly from a string. |
| FromFile | [FilePath] | Read a csv from a file.
| FromAssemblyResource | [ResourceName] (Assembly) | Read a csv from an assembly resource. If assembly is not defined, the API get the resource from the calling assembly.
| FromStream | [Stream] | Read a csv from a stream.
| FromBytes | [ByteArray] | Read a csv from a byte array.
### With.

> Define optional CSV configuration which can be combined with the `And` keyword

|Method | Argument(s)  | Comment | Usage | Default value |
|--|--|--|--|--|
| ColumnsDelimiter | [DelimiterString] |Define the string used  as columns delimiter. | Optional | ;
| EndOfLineDelimiter | [DelimiterString] | Define the string used as new line delimiter. | Optional | Environment.NewLine
| Header | (CaseMode) |Declare the first line as header. You can specify if  you treat it as case sensitive or case insensitive when putting columns in properties.   | Optional | CaseInsensitive |
| SimpleParsingMode | no argument | By default, FluentCsv use the RFC 4180 for parse your csv file. You can use SimpleParsingMode if your file doesn't respect it and you notice some problem with double quote. | Optional | NC
| CultureInfo | [[CultureCode]](https://msdn.microsoft.com/en-us/library/hh441729.aspx) | Some data like date or decimal numbers are not the same according to the culture. For example french people use a comma as decimal separator while US people use a dot character. By defining the culture code, you can tel to FluentCsv how to parse this types. | Optional | CurrentCulture

### ThatReturns.

> Define what kind of object to returns as resultset and how to populate them.  
> The type must be a class with a parameterless constructor, and contains properties with public getter and setter, or a tuple like `(string a,int b)`.  
> Due to a limitation of the `ValueTuple<>` generic type, use a POCO instead of a tuple if your result contains more than 7 members.  

> You must choose one of the following methods.

|Method| Argument(s) | Comment |
|--|--|--|
| ArrayOf<`T`> | [Type of class used to store csv data] | Define the result set as a T[] array |
| DictionaryOf<`T`> | [Type of class used to store csv data] [Member to use as a Key] | Define the result set as a Dictionary<object,T>. The Key will be the member passed as argument. |

### Put.

> Define how to map each column with a property.

|Method| Argument(s) | Comment | Usage | 
|--|--|--|--|
| Column | [Index or HeaderName] | When a column is defined by HeaderName, FluentCsv consider implicitly that the first line is a header. If all columns are defined by index and you want to skip the first line, you can use the `With.Header()` method to do it. | Mandatory |

##### SubMethods

|Method| Argument(s)  | Comment | Usage | Default Value |
|--|--|--|--|--|
| MakingSureThat | [ValidationFunction] | Set a function which validate the format of the column data. Using this method is more readable and increase performance for data validation instead of throwing exceptions. |Optional | `Data.Valid`
| As<`T`>  | [DestinationType] | Define the destination type. | Optional | string
| InThisWay | [ConversionFunction] | Set how to convert the csv column string data into the destination type. If not defined, a default conversion depending on culture info is applied. | Optional | `Convert.ChangeType`
| Into | [TargetProperty] | Set in which property to put the converted data. The property must be the same type of `As<T>` and have a public setter. Also, you can define a property localised in a subclass of your result providing that it's correctly instantiated. (see example A) | Mandatory | NC

### GetAll()

> Execute the csv parsing and return two collections

- `ResultSet` : Contains all lines of your csv that are correctly parsed.
- `Errors` : Contains all errors while parsing your file, including line number, column zero base index, column name (if csv contains header line) and exception message. 


You can call the `SaveErrorsInFile(filePath)` methods to write all errors in a simple file report.
The output is a csv file structured as follow  :

    Line;ColumnZeroBaseIndex;ColumnName;Message
    1;0;"test";"number ""1"" is invalid"

## Some scenarios

### Example A - Populate POCO with hierarchical objects

- A csv file with contact information
- BirthDate is in US format (Month/day/year)
- We want to store Firstname,Lastname and Birthdate in a subclass named `Contact`
- We want to store Street, City and ZipCode in an other subclass named `Address`

#### Csv file

    FirstName;LastName;BirthDate;Street;City;ZipCode
    Rosemary;Berube;04/10/1983;2435 Leverton Cove Road;Springfield;01103
    Richard;Lewis;12/11/1944;1509 Big Elm;New York;64110
    Michael;Lindquist;12/21/1939;1334 Clark Street;Chicago;60607

#### POCO

```c#
public class CsvInfos
{
	public Contact Contact { get; } = new Contact();
	public Address Address { get; } = new Address();

	public override string ToString() => $"{Contact} - {Address}";
}

public class Contact
{
	public string Firstname { get; set; }
	public string Lastname { get; set; }
	public DateTime BirthDate { get; set; }

	public override string ToString() => $"{Firstname} {Lastname} ({BirthDate:d})";
}

public class Address
{
	public string Street { get; set; }
	public string City { get; set; }
	public string ZipCode { get; set; }

	public override string ToString() => $"City : {City}";
}
```

#### API Call

```c#
var csv = Read.Csv.FromFile("exampleA.csv")
     .With.CultureInfo("en-US")
     .ThatReturns.ArrayOf<CsvInfos>()
     .Put.Column("FirstName").Into(p => p.Contact.Firstname)
     .Put.Column("LastName").Into(p => p.Contact.Lastname)
     .Put.Column("BirthDate").As<DateTime>().Into(p => p.Contact.BirthDate)
     .Put.Column("Street").Into(p => p.Address.Street)
     .Put.Column("City").Into(p => p.Address.City)
     .Put.Column("ZipCode").Into(p => p.Address.ZipCode)
     .GetAll();

Console.WriteLine("CSV DATA");
csv.ResultSet.ForEach(Console.WriteLine);
```

#### Output

    CSV DATA
    Rosemary Berube (10/04/1983) - City : Springfield
    Richard Lewis (11/12/1944) - City : New York
    Michael Lindquist (21/12/1939) - City : Chicago

---

### Example B - Parse a csv file that look crazy
- A csv file where columns are separated with '-' and lines end with '#'.
- There are two columns where headers have same name (but not same case)
- Some data have a double quote, and we want to get it

#### Csv file

    NAME-name#Smith-Bob#Rob"in-Wiliam



#### API Call

```C#
var csv = Read.Csv.FromFile("exampleB.csv")
	.With.EndOfLineDelimiter("#")
	.And.ColumnsDelimiter("-")
	.And.Header(As.CaseSensitive)
	.And.SimpleParsingMode()
	.ThatReturns.ArrayOf<(string FirstName, string LastName)>()
	.Put.Column("NAME").Into(a => a.FirstName)
	.Put.Column("name").Into(a => a.LastName)
	.GetAll();

Console.WriteLine("CSV DATA");
csv.ResultSet.ForEach(a=>Console.WriteLine($"{a.FirstName} {a.LastName}"));
```

#### Output

    CSV DATA
    Smith Bob
    Rob"in Wiliam

---
### Example C - Data validation

- A csv file without header that contains information with validation rules and structured data
- The file is a csv generated by excel, with columns separated by tabulation
- First column is an Id, second a phone number, third an enumeration and fourth a multiline address
- The id is a number
- Phone numbers that not contains 10 digits are not valid
- The enumeration is a set of letters (A, B or F) separated by pipes character '|'. All other letters are invalid

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

## First possibility : using POCOs, Value objects and Exceptions

#### POCOs

```C#
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
```

#### API Call

```C#
var csv = Read.Csv.FromFile("exampleC.csv")
      .With.ColumnsDelimiter("\t")
      .ThatReturns.ArrayOf<LineExampleC>()
      .Put.Column(0).As<int>().Into(a => a.Id)
      .Put.Column(1).As<PhoneNumber>().InThisWay(s => new PhoneNumber(s)).Into(a => a.PhoneNumber)
      .Put.Column(2).As<CustomEnum>().InThisWay(s => new CustomEnum(s)).Into(a => a.CustomEnum)
      .Put.Column(3).Into(a => a.Address)
      .GetAll();

Console.WriteLine("CSV DATA");
csv.ResultSet.ForEach(Console.WriteLine);

Console.WriteLine("ERRORS");
csv.Errors.ForEach(e => Console.WriteLine($"Error at line {e.LineNumber} column index {e.ColumnZeroBasedIndex} : {e.ErrorMessage}"));
```

## Second possibility (since version 2.0) : using MakingSureThat and Tuple
#### API Call
```C#
 var csv = Read.Csv.FromFile(file)
    .With.ColumnsDelimiter("\t")
    .ThatReturns.ArrayOf<(int Id, string PhoneNumber, string CustomEnum, string Address)>()
    .Put.Column(0).As<int>().Into(a => a.Id)
    .Put.Column(1).MakingSureThat(PhoneNumberIsValid).Into(a => a.PhoneNumber)
    .Put.Column(2).MakingSureThat(EnumIsValid).InThisWay(a=>a.Replace("|"," and ")).Into(a => a.CustomEnum)
    .Put.Column(3).Into(a => a.Address)
    .GetAll();

Console.WriteLine("CSV DATA");
csv.ResultSet.ForEach(a=>Console.WriteLine($"id = {a.Id}, Phone = {a.PhoneNumber}, Enum = {a.CustomEnum}, Address = {a.Address}"));

Console.WriteLine("ERRORS");
csv.Errors.ForEach(e => Console.WriteLine($"Error at line {e.LineNumber} column index {e.ColumnZeroBasedIndex} : {e.ErrorMessage}"));

Data PhoneNumberIsValid(string phone)
    =>  Regex.IsMatch(phone, "[0-9]{10}")
            ? Data.Valid 
            : Data.Invalid("Phone number is invalid");

Data EnumIsValid(string @enum)
    => @enum.Split('|').ToList().TrueForAll(a => a == "A" || a == "B" || a == "F")
        ? Data.Valid
        : Data.Invalid("Invalid enum character");
```

#### Output (in both case)

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
	
# Change log

### 3.0.0 - 2021/06/12
- [Improvement] Upgrade to .Net Standard 2.1
- [Improvement] Rewriting for best performances ([see the benchmark](https://github.com/aboudoux/FluentCSV/blob/master/Benchmark/README.MD))

### 2.0.1 - 2021/03/03
- [BugFix] Error on linux when reading last column due to default line delimiter

### 2.0.0 - 2020/10/23
- [Feature] MakingSureThat
- [Improvement] Accepting Tuple as resultset

### 1.1.0 - 2018/09/05

- [Feature] Read.Csv.EncodedIn
- [Feature] Read.Csv.FromBytes
- [Feature] Read.Csv.FromStream
- [Feature] ThatReturns.DictionaryOf<`T`>(d => d.KeyMember)
- [Feature] SaveErrorsInFile method.

### 1.0.2 - 2018/08/24
- [BugFix] Bad parsing for quadruple quotes

### 1.0.1 - 2018/08/22
- [BugFix] Incorrect parsing if string start with UTF8 BOM 
- [BugFix] Error when using a pipe as column delimiter

### 1.0.0 - 2018/08/17
- First release of the library


# License
FluentCSV is licensed under MIT License, which means it is free for all of your products, including commercial software.

