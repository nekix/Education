using Shouldly;
using Xunit.Extensions.Ordering;

namespace Practice.Recursion.Tests.RecursionFuncs_Tests;

[Order(2)]
public class PrintValuesWithEvenNumberIndex_Tests
{
    [Theory]
    [InlineData(new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 }, new int[] { 1, 3, 5, 7, 9 })]
    public void Should_Return(int[] numbers, int[] resultNumbers)
    {
        TextWriter originalConsoleOut = Console.Out;
        using StringWriter consoleWriter = new StringWriter();
        Console.SetOut(consoleWriter);

        RecursionFuncs.PrintValuesWithEvenNumberIndex(numbers.ToList(), 0);

        consoleWriter.Flush();
        string output = consoleWriter.GetStringBuilder().ToString();
        Console.SetOut(originalConsoleOut);

        string[] evenNumbers = output.Split(Environment.NewLine)
            .Where(t => !string.IsNullOrEmpty(t))
            .ToArray();

        evenNumbers.Length.ShouldBe(resultNumbers.Length);
        for (int i = 0; i < evenNumbers.Length; i++)
            evenNumbers[i].ShouldBe(resultNumbers[i].ToString());
    }
}