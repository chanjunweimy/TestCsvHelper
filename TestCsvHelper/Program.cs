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
    /*
     * TODO: Implement SuperMapper to map all classes in a ClassMap
    public sealed class SuperMap<T> : ClassMap<T> where T: ParentClass
    {
        public SuperMap()
        {
            var type = this.GetType();
            var properties = type.GetProperties();
            foreach (var property in properties)
            {
                if (property.PropertyType == typeof(bool))
                {
                    
                }
            }
        }
    }
    */

    public class Program
    {
        public IEnumerable<A> ReadFileA(string fileName)
        {
            var csv = new CsvReader(File.OpenText(fileName));
            csv.Configuration.RegisterClassMap<AMap>();
            //csv.Read();
            var records = csv.GetRecords<A>();
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
