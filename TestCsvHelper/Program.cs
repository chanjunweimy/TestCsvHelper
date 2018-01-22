using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;

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
        public static readonly string DATE_FORMAT = "yyyyMMdd";
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
                var memberMap = MemberMap.CreateGeneric(typeof(T), property);
                if (property.PropertyType == typeof(DateTime) || property.PropertyType == typeof(DateTime?))
                {
                    memberMap.Name(property.Name).TypeConverterOption.Format(Constants.DATE_FORMAT).Default(null);
                }
                else if (property.PropertyType == typeof(bool) || property.PropertyType == typeof(bool?))
                {
                    memberMap.Name(property.Name);
                }
                else
                {
                    memberMap.Name(property.Name).Default(null);
                }
                MemberMaps.Add(memberMap);
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
            var genericRegisterClassMapMethod = typeof(IReaderConfiguration).GetMethods()
                .First(m => m.Name == "RegisterClassMap" && m.IsGenericMethod);
            var genericTypeForCsv = typeof(SuperMap<>);
            var mapperTypeForCsv = genericTypeForCsv.MakeGenericType(typeof(A));
            var closedRegisterClassMapMethod =
                genericRegisterClassMapMethod.MakeGenericMethod(mapperTypeForCsv);
            closedRegisterClassMapMethod.Invoke(csv.Configuration, null);
            var genericGetRecordMethod = typeof(CsvReader).GetMethods()
                .First(m => m.Name == "GetRecords" && m.IsGenericMethod);
            var closedGetRecordMethod = genericGetRecordMethod.MakeGenericMethod(typeof(A));

            var records = (IEnumerable<T>)closedGetRecordMethod.Invoke(csv, null);
            return records;
        }

        public IEnumerable<T> ReadFileParentWithMissingField<T>(string fileName)
        {
            var csv = new CsvReader(File.OpenText(fileName));
            csv.Configuration.MissingFieldFound = null;
            var genericRegisterClassMapMethod = typeof(IReaderConfiguration).GetMethods()
                .First(m => m.Name == "RegisterClassMap" && m.IsGenericMethod);
            var genericTypeForCsv = typeof(SuperMap<>);
            var mapperTypeForCsv = genericTypeForCsv.MakeGenericType(typeof(A));
            var closedRegisterClassMapMethod =
                genericRegisterClassMapMethod.MakeGenericMethod(mapperTypeForCsv);
            closedRegisterClassMapMethod.Invoke(csv.Configuration, null);
            var genericGetRecordMethod = typeof(CsvReader).GetMethods()
                .First(m => m.Name == "GetRecords" && m.IsGenericMethod);
            var closedGetRecordMethod = genericGetRecordMethod.MakeGenericMethod(typeof(A));

            var records = (IEnumerable<T>)closedGetRecordMethod.Invoke(csv, null);
            
            return records;
        }

        public T GetFileParentRow<T>(string fileName, int row)
        {
            var csv = new CsvReader(File.OpenText(fileName));
            var genericRegisterClassMapMethod = typeof(IReaderConfiguration).GetMethods()
                .First(m => m.Name == "RegisterClassMap" && m.IsGenericMethod);
            var genericTypeForCsv = typeof(SuperMap<>);
            var mapperTypeForCsv = genericTypeForCsv.MakeGenericType(typeof(A));
            var closedRegisterClassMapMethod =
                genericRegisterClassMapMethod.MakeGenericMethod(mapperTypeForCsv);
            closedRegisterClassMapMethod.Invoke(csv.Configuration, null);
            var genericGetRecordMethod = typeof(CsvReader).GetMethods()
                .First(m => m.Name == "GetRecord" && m.IsGenericMethod);
            var closedGetRecordMethod = genericGetRecordMethod.MakeGenericMethod(typeof(A));
            var records = new List<T>();
            while (csv.Read())
            {
                var data = closedGetRecordMethod.Invoke(csv, null);
                if (csv.Context.Row == row + 1)
                {
                    return (T) data;
                }
            }
            return default(T);
        }

        static void Main(string[] args)
        {
            var program = new Program();
            var contents = program.ReadFileParent<A>("TestData/A.csv").ToList();
            Console.WriteLine(contents[0].Id);
        }
    }
}
