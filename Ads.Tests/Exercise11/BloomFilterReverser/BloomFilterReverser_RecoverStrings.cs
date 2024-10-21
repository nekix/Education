extern alias Exercise11;

using Exercise11.AlgorithmsDataStructures;
using Exercise11.Ads.Exercise11;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Shouldly;

namespace Ads.Tests.Exercise11.BloomFilterReverser
{
    public class BloomFilterReverser_RecoverStrings : BloomFilter_BaseTests
    {
        [Fact]
        public void Should_Recover()
        {
            int count = 10;

            var filter = GetFulledBloomFilter(32, GenerateTestStrings().Take(count));

            var result = filter.RecoverStrings(GenerateTestStrings());

            result.CleanResult.Count().ShouldBeLessThanOrEqualTo(count);
            var r = result.CleanResult.Union(result.CollisionResults).Union(result.FalsePositiveResults).ToList();

            GenerateTestStrings().Take(count).ToList().ForEach(t => r.Contains(t).ShouldBeTrue());
        }

        private static IEnumerable<string> GenerateTestStrings()
        {
            return new List<string>
            {
                "hello",
                "world",
                "this is a test",
                "another string",
                "CSharp",
                "random text",
                "example data",
                "test case",
                "LINQ",
                "data generation",
                "string length",
                "unit testing",
                "sample string",
                "array of strings",
                "enumerable",
                "foreach loop",
                "testing strings",
                "generate",
                "yield return",
                "method example",
                "code sample",
                "programming",
                "simple test",
                "string array",
                "testing purpose",
                "data structure",
                "performance test",
                "debugging",
                "exception handling",
                "string manipulation",
                "test string 123",
                "CSharp programming",
                "data science",
                "machine learning",
                "artificial intelligence",
                "API testing",
                "web development",
                "mobile applications",
                "software engineering",
                "version control",
                "repository",
                "agile methodology",
                "scrum",
                "kanban",
                "project management",
                "test-driven development",
                "clean code",
                "refactoring",
                "code review",
                "design patterns",
                "object-oriented programming"
            };
        }
    }
}
