using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TestCsvHelper
{
    public class ParentClass
    {
        public int Id { get; set; }
        public bool IsChild { get; set; }
        public DateTime DateTime { get; set; }
    }

    public class A : ParentClass
    {
        public string Name { get; set; }
    }

    public class B : ParentClass
    {
        public string Title { get; set; }
    }

    public class Constants
    {
        public static readonly string DATE_FORMAT = "yyyy/MM/dd";
    }

    public sealed class AMap : ClassMap<A>
    {
        public AMap()
        {
            Map(a => a.Id).Index(0).Name("Id");
            Map(a => a.IsChild).Index(1).Name("IsChild");
            Map(a => a.DateTime).Index(2).Name("DateTime").TypeConverterOption.Format(Constants.DATE_FORMAT).Default(null);          
            Map(a => a.Name).Index(3).Name("Name");
        }
    }

    public sealed class BMap : ClassMap<B>
    {
        public BMap()
        {
            AutoMap();
            Map(b => b.DateTime).Name("DateTime").TypeConverterOption.Format(Constants.DATE_FORMAT).Default(null);
            Map(b => b.IsChild).Name("IsChild");
        }
    }

    public sealed class SuperMap<T> : ClassMap<T> where T: ParentClass
    {
        public SuperMap()
        {
            var properties = typeof(T)
                .GetProperties();
            foreach (var property in properties)
            {
                if (property.PropertyType == typeof(DateTime) || property.PropertyType == typeof(DateTime?))
                {
                    Map(typeof(T), property).Name(property.Name).TypeConverterOption.Format(Constants.DATE_FORMAT).Default(null);
                }
                else if (property.PropertyType == typeof(bool) || property.PropertyType == typeof(bool?))
                {
                    Map(typeof(T), property).Name(property.Name);
                }
                else
                {
                    Map(typeof(T), property).Name(property.Name).Default(null);
                }
            }
        }
    }

    public class Program
    {
        public IEnumerable<A> ReadFileA(string fileName)
        {
            var csv = new CsvReader(File.OpenText(fileName));
            csv.Configuration.RegisterClassMap<AMap>();
            var records = csv.GetRecords<A>();
            return records;
        }

        public IEnumerable<T> ReadFileParent<T>(string fileName)
        {
            var csv = new CsvReader(File.OpenText(fileName));
            csv.Configuration.RegisterClassMap<AMap>();
            var records = csv.GetRecords<T>();
            return records;
        }

        static void Main(string[] args)
        {
            var program = new Program();
            var contents = program.ReadFileA("TestData/A.csv").ToList();
            Console.WriteLine(contents[0].Id);
        }
    }
}
