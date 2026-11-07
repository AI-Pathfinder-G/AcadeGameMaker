using System;
using System.Globalization;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using AcadeGameMaker.Profile;
using NUnit.Framework;

namespace AcadeGameMaker.Tests.EditMode.Profile
{
    // REQ-M5D5-001..006; AC-M5D5-001..007. Strict JCS golden and revalidation matrix.
    public sealed class ProfileBindingOverridesJsonTests
    {
        [Test]
        public void AC001_EmptySentinelAndDefaultBoundariesAreExact()
        {
            var empty = ProfileBindingOverridesJson.Parse(string.Empty);
            Assert.That(empty.IsEmpty, Is.True);
            Assert.That(empty.CanonicalText, Is.EqualTo(string.Empty));
            Assert.Throws<ArgumentNullException>(() => ProfileBindingOverridesJson.Parse(null));
            Assert.Throws<ArgumentException>(() => ProfileBindingOverridesJson.Parse(" \t\r\n"));
            var @default = default(ProfileBindingOverridesJson);
            Assert.Throws<InvalidOperationException>(() => @default.Validate());
            Assert.Throws<InvalidOperationException>(() => { var ignored = @default.IsEmpty; });
            Assert.Throws<InvalidOperationException>(() => { var ignored = @default.CanonicalText; });
        }

        [Test]
        public void AC002_RecursiveOrdinalSortingAndDecodedDuplicatesAreStrict()
        {
            AssertCanonical(" { \"z\": [ { \"b\": 2, \"a\": 1 }, 3 ], \"a\": true } ", "{\"a\":true,\"z\":[{\"a\":1,\"b\":2},3]}");
            Assert.Throws<ArgumentException>(() => ProfileBindingOverridesJson.Parse("{\"a\":1,\"\\u0061\":2}"));
            Assert.Throws<ArgumentException>(() => ProfileBindingOverridesJson.Parse("[{\"x\":1,\"\\u0078\":2}]"));
            Assert.Throws<ArgumentException>(() => ProfileBindingOverridesJson.Parse("{\"a\":1,}"));
            Assert.Throws<ArgumentException>(() => ProfileBindingOverridesJson.Parse("{/*x*/\"a\":1}"));
            Assert.Throws<ArgumentException>(() => ProfileBindingOverridesJson.Parse("true false"));
        }

        [Test]
        public void AC002_MaxDepth64IsAcceptedAndDepth65IsRejected()
        {
            var depth64 = NestedArray(64);
            Assert.That(ProfileBindingOverridesJson.Parse(depth64).CanonicalText, Is.EqualTo(depth64));
            Assert.Throws<ArgumentException>(() => ProfileBindingOverridesJson.Parse(NestedArray(65)));
        }

        [Test]
        public void AC003_CustomStringEscapingAndUnicodeValidationAreExact()
        {
            AssertCanonical("\"\\\"\\\\\\b\\t\\n\\f\\r\\u0001/\u00e9\\ud83d\\ude00\"", "\"\\\"\\\\\\b\\t\\n\\f\\r\\u0001/\u00e9\ud83d\ude00\"");
            Assert.Throws<ArgumentException>(() => ProfileBindingOverridesJson.Parse("\"\\ud800\""));
            Assert.Throws<ArgumentException>(() => ProfileBindingOverridesJson.Parse("\"\\udc00\""));
            Assert.Throws<ArgumentException>(() => ProfileBindingOverridesJson.Parse("\"e\\u0301\""));
            Assert.Throws<ArgumentException>(() => ProfileBindingOverridesJson.Parse("{\"e\\u0301\":1}"));
        }

        [Test]
        public void AC004_Binary64AppendixThresholdAndCultureGoldensAreExact()
        {
            AssertCanonical("-0", "0");
            AssertCanonical("5e-324", "5e-324");
            AssertCanonical("1.7976931348623157e308", "1.7976931348623157e+308");
            AssertCanonical("9007199254740992", "9007199254740992");
            AssertCanonical("1e-6", "0.000001");
            AssertCanonical("1e-7", "1e-7");
            AssertCanonical("1e20", "100000000000000000000");
            AssertCanonical("1e21", "1e+21");
            AssertCanonical("333333333.33333329", "333333333.3333333");
            var prior = CultureInfo.CurrentCulture;
            try
            {
                CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("fr-FR");
                Assert.That(ProfileBindingOverridesJson.Parse("1.5").CanonicalText, Is.EqualTo("1.5"));
            }
            finally { CultureInfo.CurrentCulture = prior; }
        }

        [Test]
        public void AC005_IdempotenceHashAndPublicRevalidationAreStrict()
        {
            var first = ProfileBindingOverridesJson.Parse("{\"b\":\"\\u0061\",\"a\":[true,false]}");
            var second = ProfileBindingOverridesJson.Parse(" { \"a\" : [ true , false ], \"b\" : \"a\" }");
            Assert.That(first.CanonicalText, Is.EqualTo("{\"a\":[true,false],\"b\":\"a\"}"));
            Assert.That(second.CanonicalText, Is.EqualTo(first.CanonicalText));
            Assert.That(Sha256(first.CanonicalText), Is.EqualTo(Sha256(second.CanonicalText)));
            Assert.That(ProfileBindingOverridesJson.Parse(first.CanonicalText).CanonicalText, Is.EqualTo(first.CanonicalText));

            var malformed = Corrupt(first, "_canonicalText", "{ \"a\" : 1 }");
            Assert.Throws<InvalidOperationException>(() => malformed.Validate());
            Assert.Throws<InvalidOperationException>(() => { var ignored = malformed.IsEmpty; });
            Assert.Throws<InvalidOperationException>(() => { var ignored = malformed.CanonicalText; });
            Assert.That(ProfileBindingOverridesJson.Parse("[]").CanonicalText, Is.EqualTo("[]"));
        }

        [Test]
        public void AC006_StaticProfileBoundaryUsesStrictParserAndCustomCanonicalization()
        {
            var source = System.IO.File.ReadAllText(System.IO.Path.Combine(FindRoot(), "Assets", "AcadeGameMaker", "Runtime", "Profile", "ProfileBindingOverridesJson.cs"));
            StringAssert.Contains("JsonDocumentOptions", source);
            StringAssert.Contains("StringComparer.Ordinal", source);
            StringAssert.Contains("WriteString", source);
            StringAssert.Contains("ShortestRoundTrip", source);
            foreach (var forbidden in new[] { "UnityEngine", "System.IO", "Cryptography", "DateTime", "Random", "System.Net", "callback", "Utf8JsonWriter" })
                StringAssert.DoesNotContain(forbidden, source);
        }

        private static void AssertCanonical(string source, string expected) => Assert.That(ProfileBindingOverridesJson.Parse(source).CanonicalText, Is.EqualTo(expected));
        private static ProfileBindingOverridesJson Corrupt(ProfileBindingOverridesJson source, string field, object value)
        {
            object boxed = source;
            typeof(ProfileBindingOverridesJson).GetField(field, BindingFlags.Instance | BindingFlags.NonPublic).SetValue(boxed, value);
            return (ProfileBindingOverridesJson)boxed;
        }
        private static string Sha256(string value) { using (var hash = SHA256.Create()) return BitConverter.ToString(hash.ComputeHash(Encoding.UTF8.GetBytes(value))).Replace("-", string.Empty); }
        private static string NestedArray(int depth) => new string('[', depth) + "0" + new string(']', depth);
        private static string FindRoot()
        {
            var current = new System.IO.DirectoryInfo(TestContext.CurrentContext.TestDirectory);
            while (current != null) { if (System.IO.File.Exists(System.IO.Path.Combine(current.FullName, "Packages", "manifest.json"))) return current.FullName; current = current.Parent; }
            throw new System.IO.DirectoryNotFoundException();
        }
    }
}
