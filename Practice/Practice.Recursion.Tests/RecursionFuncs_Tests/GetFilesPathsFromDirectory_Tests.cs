using Shouldly;

namespace Practice.Recursion.Tests.RecursionFuncs_Tests;

public class GetFilesPathsFromDirectory_Tests
{
    [Fact]
    public void Should_Return()
    {
        var dirPath = Directory.GetCurrentDirectory();

        var res = RecursionFuncs.GetFilesPathsFromDirectory(dirPath);

        var files = Directory.GetFiles(dirPath, "*", SearchOption.AllDirectories);

        res.ShouldBe(files);
    }
}