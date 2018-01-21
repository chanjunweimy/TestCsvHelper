using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System;
using System.Globalization;
using System.Linq;

namespace TestCsvHelper.Tests
{
    [TestClass]
    public class CsvHelperMapperTest
    {
        [TestMethod]
        public void TestAMapper()
        {
            var program = new Program();
            var contents = program.ReadFileA("TestData/A.csv");
            CultureInfo provider = CultureInfo.InvariantCulture;
            var format = Constants.DATE_FORMAT;
            var expectedAs = new List<A> {
                new A
                {
                    Id = 1,
                    IsChild = true,
                    DateTime = DateTime.ParseExact("2018/11/11", format, provider),
                    Name = "abc"
                },
                new A
                {
                    Id = 2,
                    IsChild = true,
                    DateTime = DateTime.ParseExact("2018/01/11", format, provider),
                    Name = "abcd"
                },
                new A
                {
                    Id = 3,
                    IsChild = false,
                    DateTime = DateTime.ParseExact("2018/11/12", format, provider),
                    Name = "abcde"
                }
            };
            foreach (var content in contents.ToList())
            {
                Assert.AreEqual(expectedAs[content.Id - 1].Id, content.Id);
                Assert.AreEqual(expectedAs[content.Id - 1].IsChild, content.IsChild);
                Assert.AreEqual(expectedAs[content.Id - 1].DateTime, content.DateTime);
                Assert.AreEqual(expectedAs[content.Id - 1].Name, content.Name);
            }

        }
    }
}
