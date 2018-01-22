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
        private readonly string ERROR_ID = "Id is not equal";
        private readonly string ERROR_IS_CHILD = "IsChild is not equal";
        private readonly string ERROR_DATE_TIME = "DateTime is not equal";
        private readonly string ERROR_NAME = "Name is not equal";

        [TestMethod]
        public void TestAMapper()
        {
            var program = new Program();
            var contents = program.ReadFileA("TestData/A.csv");
            var provider = CultureInfo.InvariantCulture;
            var format = Constants.DATE_FORMAT;
            var expectedAs = new List<A> {
                new A
                {
                    Id = 1,
                    IsChild = true,
                    DateTime = DateTime.ParseExact("20181111", format, provider),
                    Name = "abc"
                },
                new A
                {
                    Id = 2,
                    IsChild = true,
                    DateTime = DateTime.ParseExact("20180111", format, provider),
                    Name = "abcd"
                },
                new A
                {
                    Id = 3,
                    IsChild = false,
                    DateTime = DateTime.ParseExact("20181112", format, provider),
                    Name = "abcde"
                }
            };
            foreach (var content in contents.ToList())
            {
                Assert.AreEqual(expectedAs[content.Id - 1].Id, content.Id, ERROR_ID);
                Assert.AreEqual(expectedAs[content.Id - 1].IsChild, content.IsChild, ERROR_IS_CHILD);
                Assert.AreEqual(expectedAs[content.Id - 1].DateTime, content.DateTime, ERROR_DATE_TIME);
                Assert.AreEqual(expectedAs[content.Id - 1].Name, content.Name, ERROR_NAME);
            }
        }

        [TestMethod]
        public void TestSuperMapper()
        {
            var program = new Program();
            var contents = program.ReadFileParent<A>("TestData/A.csv");
            var provider = CultureInfo.InvariantCulture;
            var format = Constants.DATE_FORMAT;
            var expectedAs = new List<A> {
                new A
                {
                    Id = 1,
                    IsChild = true,
                    DateTime = DateTime.ParseExact("20181111", format, provider),
                    Name = "abc"
                },
                new A
                {
                    Id = 2,
                    IsChild = true,
                    DateTime = DateTime.ParseExact("20180111", format, provider),
                    Name = "abcd"
                },
                new A
                {
                    Id = 3,
                    IsChild = false,
                    DateTime = DateTime.ParseExact("20181112", format, provider),
                    Name = "abcde"
                }
            };
            foreach (var content in contents.ToList())
            {
                Assert.AreEqual(expectedAs[content.Id - 1].Id, content.Id, ERROR_ID);
                Assert.AreEqual(expectedAs[content.Id - 1].IsChild, content.IsChild, ERROR_IS_CHILD);
                Assert.AreEqual(expectedAs[content.Id - 1].DateTime, content.DateTime, ERROR_DATE_TIME);
                Assert.AreEqual(expectedAs[content.Id - 1].Name, content.Name, ERROR_NAME);
            }
        }

        [TestMethod]
        public void TestSuperMapperWithMissing()
        {
            var program = new Program();
            var contents = program.ReadFileParentWithMissingField<A>("TestData/AWithMissing.csv");
            var provider = CultureInfo.InvariantCulture;
            var format = Constants.DATE_FORMAT;
            var expectedAs = new List<A> {
                new A
                {
                    Id = 1,
                    IsChild = true,
                    DateTime = DateTime.ParseExact("20181111", format, provider),
                    Name = null
                },
                new A
                {
                    Id = 2,
                    IsChild = true,
                    DateTime = DateTime.ParseExact("20180111", format, provider),
                    Name = null
                },
                new A
                {
                    Id = 3,
                    IsChild = false,
                    DateTime = DateTime.ParseExact("20181112", format, provider),
                    Name = null
                }
            };
            foreach (var content in contents.ToList())
            {
                Assert.AreEqual(expectedAs[content.Id - 1].Id, content.Id, ERROR_ID);
                Assert.AreEqual(expectedAs[content.Id - 1].IsChild, content.IsChild, ERROR_IS_CHILD);
                Assert.AreEqual(expectedAs[content.Id - 1].DateTime, content.DateTime, ERROR_DATE_TIME);
                Assert.AreEqual(expectedAs[content.Id - 1].Name, content.Name, ERROR_NAME);
            }
        }

        [TestMethod]
        public void TestSuperMapperByRow()
        {
            const int row = 2;
            var program = new Program();
            var content = program.GetFileParentRow<A>("TestData/A.csv", row);
            var provider = CultureInfo.InvariantCulture;
            var format = Constants.DATE_FORMAT;
            var expectedAs = new List<A> {
                new A
                {
                    Id = 1,
                    IsChild = true,
                    DateTime = DateTime.ParseExact("20181111", format, provider),
                    Name = "abc"
                },
                new A
                {
                    Id = 2,
                    IsChild = true,
                    DateTime = DateTime.ParseExact("20180111", format, provider),
                    Name = "abcd"
                },
                new A
                {
                    Id = 3,
                    IsChild = false,
                    DateTime = DateTime.ParseExact("20181112", format, provider),
                    Name = "abcde"
                }
            };

            Assert.AreEqual(expectedAs[row - 1].Id, content.Id, ERROR_ID);
            Assert.AreEqual(expectedAs[row - 1].IsChild, content.IsChild, ERROR_IS_CHILD);
            Assert.AreEqual(expectedAs[row - 1].DateTime, content.DateTime, ERROR_DATE_TIME);
            Assert.AreEqual(expectedAs[row - 1].Name, content.Name, ERROR_NAME);
        }

        [TestMethod]
        [ExpectedException(typeof(CsvHelper.MissingFieldException))]
        public void TestSuperMapperWithMissing_Failed()
        {
            var program = new Program();

            var contents = program.ReadFileParent<A>("TestData/AWithMissing.csv");
            var provider = CultureInfo.InvariantCulture;
            var format = Constants.DATE_FORMAT;
            var expectedAs = new List<A>
            {
                new A
                {
                    Id = 1,
                    IsChild = true,
                    DateTime = DateTime.ParseExact("20181111", format, provider),
                    Name = "abc"
                },
                new A
                {
                    Id = 2,
                    IsChild = true,
                    DateTime = DateTime.ParseExact("20180111", format, provider),
                    Name = "abcd"
                },
                new A
                {
                    Id = 3,
                    IsChild = false,
                    DateTime = DateTime.ParseExact("20181112", format, provider),
                    Name = "abcde"
                }
            };
            foreach (var content in contents.ToList())
            {
                Assert.AreEqual(expectedAs[content.Id - 1].Id, content.Id, ERROR_ID);
                Assert.AreEqual(expectedAs[content.Id - 1].IsChild, content.IsChild, ERROR_IS_CHILD);
                Assert.AreEqual(expectedAs[content.Id - 1].DateTime, content.DateTime, ERROR_DATE_TIME);
                Assert.AreEqual(expectedAs[content.Id - 1].Name, content.Name, ERROR_NAME);
            }
        }
    }
}
