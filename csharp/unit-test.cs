using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.IO;

namespace PlistParser.Tests
{
    [TestClass]
    public class PListParserTests
    {
        private string tempDir;

        [TestInitialize]
        public void Setup()
        {
            tempDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(tempDir);

            File.WriteAllText(Path.Combine(tempDir, "B.plist"), @"
PList B {
  Pat B.p1;
  Pat B.p2;
}");

            File.WriteAllText(Path.Combine(tempDir, "C.plist"), @"
PList C
{
  RefPList B;
  Pat C.p1;
}");

            File.WriteAllText(Path.Combine(tempDir, "A.plist"), @"
PList A {
  RefPList C;
  Pat A.p1;
  Pat A.p2;
  PList A.1 {
    PList A.1.1 {
      RefPList C;
      Pat A.1.1.p1;
    }
    PList A.1.2 {
      Pat A.1.2.p1;
    }
  }
}");
        }

        [TestCleanup]
        public void Cleanup() => Directory.Delete(tempDir, true);

        [TestMethod]
        public void TestFlattenedPatterns()
        {
            var parser = new PListParser();
            parser.ParseAllPlists(tempDir);

            Assert.IsTrue(parser.GetFlattenedMap()["A"].SetEquals(new[]
            {
                "B.p1", "B.p2", "C.p1", "A.p1", "A.p2", "A.1.1.p1", "A.1.2.p1"
            }));

            Assert.IsTrue(parser.GetFlattenedMap()["B"].SetEquals(new[]
            {
                "B.p1", "B.p2"
            }));

            Assert.IsTrue(parser.GetFlattenedMap()["C"].SetEquals(new[]
            {
                "B.p1", "B.p2", "C.p1"
            }));
        }
    }
}
