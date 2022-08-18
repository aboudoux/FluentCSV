

# FluentCSV 3.0
### Motivation
On 2021-01-18, *Joel Verhagen* wrote a [blog post](https://www.joelverhagen.com/blog/2020/12/fastest-net-csv-parsers) to test different Csv .NET parser performance.

In this test, FluentCSV is very very far behind with a very bad score of 57s for parsing 1 000 000 lines where the others take only few seconds for do the job.

Although the main purpose of FluentCSV is to make your code super clean and readable by providing a fluent API for reading your CSV files, this is not a reason to have performance issue like this, it's why we have worked on an upgrade to fix it!

### Action plan
FluentCSV contains two parser :
- A Simple parser which work as a simple `string.Split`
- A Rfc4180 Parser that is clearly the weak point

So, the goal is not to be the best parser of the world, but to reduce the processing time of the Rfc4180 so that it is as close as possible as the Simple parser.

For that, we have migrated FluentCSV to .NET Standard 2.1 to benefit of the `Span<T>` structure and rewrite the parser with it.

Also we found another optimization possibility by parallelizing the processing of our file, which saves us a little extra time.

let's see what it gives

### Benchmark
For this benchmark, we read a CSV file of 160Mo with 1 000 000 lines and 10 columns on a Surface book PRO - Intel(R) Core(TM) i7-6600U CPU @ 2.60GHz   2.81 GHz - 16Go RAM, 1To SSD, by using [BenchmarkDotNet](https://benchmarkdotnet.org/)

----
#### Result for FluentCSV 2.0.1
|            Method |     Mean |    Error |   StdDev |
|------------------ |---------:|---------:|---------:|
|           Rfc4180 | **48.775 s** | 0.7324 s | 0.6116 s |
| SimpleParsingMode |  **6.482 s** | 0.1254 s | 0.2130 s |
----
#### Result for FluentCSV 3.0
|            Method |    Mean |    Error |   StdDev |
|------------------ |--------:|---------:|---------:|
|           Rfc4180 | **5.919 s** | 0.1161 s | 0.1140 s |
| SimpleParsingMode | **5.204 s** | 0.0865 s | 0.0722 s |
---
#### Diagram
![benchmark diagram](https://github.com/aboudoux/FluentCSV/blob/master/Benchmark/graph.PNG)

### Conclusions
- The `Rfc4180` parser of FluentCSV 3 is now **8** more faster than FluentCSV 2!
- The `SimpleParsingMode` of FluentCSV 3 gains **20%** of performance thanks to parallelization (I guess the more core you have on your machine, the faster the parser works)
- The new `Rfc4180` parser is faster than `SimpleParsingMode` of the previous version of fluentCSV!
- The difference between `SimpleParsingMode` and `Rfc4180` is largely reduced (only 700ms). The main objective of this new version is therefore achieved.
 
### post-scriptum
We have run the benchmark made by _Joel_ in our machine with FluentCSV 3. here's the result :

![new diagram](https://github.com/aboudoux/FluentCSV/blob/master/Benchmark/total.PNG)

So, it's better now.
