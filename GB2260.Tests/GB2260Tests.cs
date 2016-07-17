using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace GB2260.Tests
{
    [TestClass]
    public class GB2260Tests
    {
        private static GB2260 gb;
        private static GB2260 gb2;

        [ClassInitialize]
        public static void BeforeTests(TestContext testContext)
        {
            gb = new GB2260();
            gb2 = new GB2260("201308");
        }
        [TestMethod]
        [ExpectedException(typeof(System.IO.IOException))]
        public void InstantiationTest()
        {
            gb = new GB2260();
            gb2 = new GB2260("201308");
            Assert.AreEqual(gb.AvailableRevisions.Select(a => int.Parse(a)).Max().ToString(), gb.Revision);
            Assert.AreEqual("201308", gb2.Revision);
            Assert.AreEqual(34, gb2.Provinces.Count);
            var gb3 = new GB2260("999999");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidCodeException))]
        public void DivisionTest()
        {
            var division = gb.Division("110000");
            Assert.AreEqual("北京市", division.Name);
            Assert.AreEqual(null, division.Province);
            Assert.AreEqual(null, division.Prefecture);
            Assert.AreEqual("北京市", division.Description);
            Assert.AreEqual("110000", division.Code);
            Assert.AreEqual(gb.Revision, division.Revision);
            Assert.AreEqual("北京市", division.ToString());
            var division2 = gb.Division("999");
        }

        [TestMethod]
        public void PrefectureTest()
        {
            var division = gb.Division("110100");
            Assert.AreEqual("市辖区", division.Name);
            Assert.AreEqual("北京市", division.Province);
            Assert.AreEqual(null, division.Prefecture);
            Assert.AreEqual("北京市 市辖区", division.Description);
            Assert.AreEqual("110100", division.Code);
            Assert.AreEqual(gb.Revision, division.Revision);
        }

        [TestMethod]
        public void CountryTest()
        {
            var division = gb.Division("110101");
            Assert.AreEqual("东城区", division.Name);
            Assert.AreEqual("北京市", division.Province);
            Assert.AreEqual("市辖区", division.Prefecture);
            Assert.AreEqual("北京市 市辖区 东城区", division.Description);
            Assert.AreEqual("110101", division.Code);
            Assert.AreEqual(gb.Revision, division.Revision);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidCodeException))]
        public void PrefecturesTest()
        {
            var prefectures = gb.Prefectures("11");
            Assert.IsTrue(prefectures.Count > 0);
            var prefectures2 = gb.Prefectures("00");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidCodeException))]
        public void CountriesTest()
        {
            var countries = gb.Countries("1101");
            Assert.IsTrue(countries.Count > 0);
            var countries2 = gb.Prefectures("0000");
        }
    }
}