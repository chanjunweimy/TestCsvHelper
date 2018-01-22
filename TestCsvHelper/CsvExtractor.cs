using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TestCsvHelper
{
    public interface ICsvEntity
    {
    }

    public class A : ICsvEntity
    {
        public int Id { get; set; }
        public bool IsChild { get; set; }
        public DateTime DateTime { get; set; }
        public string Name { get; set; }
    }

    public class B : ICsvEntity
    {
        public int Id { get; set; }
        public int? Count { get; set; }
    }

    public class CsvConfigurationConstant
    {
        public static readonly string DATE_FORMAT = "yyyyMMdd";
    }

    public sealed class AMap : ClassMap<A>
    {
        public AMap()
        {
            Map(a => a.Id).Index(0).Name("Id");
            Map(a => a.IsChild).Index(1).Name("IsChild");
            Map(a => a.DateTime).Index(2).Name("DateTime").TypeConverterOption.Format(CsvConfigurationConstant.DATE_FORMAT).Default(null);          
            Map(a => a.Name).Index(3).Name("Name");
        }
    }
    
    public sealed class GenericClassMap<T> : ClassMap<T> where T: ICsvEntity
    {
        public GenericClassMap()
        {
            var properties = typeof(T)
                .GetProperties();
            foreach (var property in properties)
            {
                var memberMap = MemberMap.CreateGeneric(typeof(T), property);
                if (property.PropertyType.IsGenericType && property.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    var typeConverterFactory = new TypeConverterCache();
                    var nullableConverter = new NullableConverter(property.PropertyType, typeConverterFactory);
                    memberMap.TypeConverter(nullableConverter);
                }

                if (property.PropertyType == typeof(DateTime) || property.PropertyType == typeof(DateTime?))
                {
                    memberMap.Name(property.Name).TypeConverterOption.Format(CsvConfigurationConstant.DATE_FORMAT).Default(null);
                }
                else
                {
                    memberMap.Name(property.Name).Default(null);
                }
                MemberMaps.Add(memberMap);
            }
        }
    }

    public class CsvExtractor
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
            var genericTypeForCsv = typeof(GenericClassMap<>);
            var mapperTypeForCsv = genericTypeForCsv.MakeGenericType(typeof(T));
            var closedRegisterClassMapMethod =
                genericRegisterClassMapMethod.MakeGenericMethod(mapperTypeForCsv);
            closedRegisterClassMapMethod.Invoke(csv.Configuration, null);
            var genericGetRecordMethod = typeof(CsvReader).GetMethods()
                .First(m => m.Name == "GetRecords" && m.IsGenericMethod);
            var closedGetRecordMethod = genericGetRecordMethod.MakeGenericMethod(typeof(T));

            var records = (IEnumerable<T>)closedGetRecordMethod.Invoke(csv, null);
            return records;
        }

        public IEnumerable<T> ReadFileParentWithMissingField<T>(string fileName)
        {
            var csv = new CsvReader(File.OpenText(fileName));
            csv.Configuration.MissingFieldFound = null;
            var genericRegisterClassMapMethod = typeof(IReaderConfiguration).GetMethods()
                .First(m => m.Name == "RegisterClassMap" && m.IsGenericMethod);
            var genericTypeForCsv = typeof(GenericClassMap<>);
            var mapperTypeForCsv = genericTypeForCsv.MakeGenericType(typeof(T));
            var closedRegisterClassMapMethod =
                genericRegisterClassMapMethod.MakeGenericMethod(mapperTypeForCsv);
            closedRegisterClassMapMethod.Invoke(csv.Configuration, null);
            var genericGetRecordMethod = typeof(CsvReader).GetMethods()
                .First(m => m.Name == "GetRecords" && m.IsGenericMethod);
            var closedGetRecordMethod = genericGetRecordMethod.MakeGenericMethod(typeof(T));

            var records = (IEnumerable<T>)closedGetRecordMethod.Invoke(csv, null);
            
            return records;
        }

        public T GetFileParentRow<T>(string fileName, int row)
        {
            var csv = new CsvReader(File.OpenText(fileName));
            var genericRegisterClassMapMethod = typeof(IReaderConfiguration).GetMethods()
                .First(m => m.Name == "RegisterClassMap" && m.IsGenericMethod);
            var genericTypeForCsv = typeof(GenericClassMap<>);
            var mapperTypeForCsv = genericTypeForCsv.MakeGenericType(typeof(T));
            var closedRegisterClassMapMethod =
                genericRegisterClassMapMethod.MakeGenericMethod(mapperTypeForCsv);
            closedRegisterClassMapMethod.Invoke(csv.Configuration, null);
            var genericGetRecordMethod = typeof(CsvReader).GetMethods()
                .First(m => m.Name == "GetRecord" && m.IsGenericMethod);
            var closedGetRecordMethod = genericGetRecordMethod.MakeGenericMethod(typeof(T));
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
            var program = new CsvExtractor();
            var contents = program.ReadFileParent<A>("TestData/A.csv").ToList();
            Console.WriteLine(contents[0].Id);
        }
    }
}
