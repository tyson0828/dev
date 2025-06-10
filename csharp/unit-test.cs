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

    [Fact]
    public void Should_Parse_And_Flatten_PLists_Correctly()
    {
        // Arrange
        var testDir = "TestData";
        Directory.CreateDirectory(testDir);

        File.WriteAllText(Path.Combine(testDir, "A.plist"), @"
PList A.1 {
  PreExecRefPList Pre1
  Pat A.1.p1
  RefPList B.1
  Pat A.1.p2
  RefPList B.1
  Pat A.1.p3
  PostExecRefPList Post1
}

PList A.2 {
  PreExecRefPList Pre1
  Pat A.2.p1
  RefPList B.1
  Pat A.2.p2
  RefPList B.1
  Pat A.2.p3
  PostExecRefPList Post1
}

PList A.3 {
  PreExecRefPList Pre1
  Pat A.2.p1
  RefPList B.1
  Pat A.3.p2
  RefPList B.1
  Pat A.3.p3
  PostExecRefPList Post1
}
");

        File.WriteAllText(Path.Combine(testDir, "B.plist"), @"
PList B.1 {
  RefPList C.1
  Pat B.1.p1
}
");

        File.WriteAllText(Path.Combine(testDir, "C.plist"), @"
PList C.1 {
  Pat C.1.p1
}
");

        File.WriteAllText(Path.Combine(testDir, "Pre1.plist"), @"
PList Pre1 {
  Pat Pre1.p1
}
");

        File.WriteAllText(Path.Combine(testDir, "Post1.plist"), @"
PList Post1 {
  Pat Post1.p1
}
");

        var parser = new PListParser();

        // Act
        parser.ParseAllPlists(testDir);

        // Assert
        parser.PrintFlattenedResults();

        var flattened = GetFlattened(parser, "A.1");
        Assert.Contains("A.1.p1", flattened);
        Assert.Contains("A.1.p2", flattened);
        Assert.Contains("A.1.p3", flattened);
        Assert.Contains("B.1.p1", flattened);
        Assert.Contains("C.1.p1", flattened);
        Assert.Contains("Pre1.p1", flattened);
        Assert.Contains("Post1.p1", flattened);
    }

    private HashSet<string> GetFlattened(PListParser parser, string plistName)
    {
        var field = typeof(PListParser).GetField("flattenedMap", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var map = (Dictionary<string, HashSet<string>>)field.GetValue(parser)!;
        return map[plistName];
    }
}

    }
}
