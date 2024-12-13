using Shouldly;
using Xunit.Extensions.Ordering;

namespace Practice.Recursion.Tests.RecursionFuncs_Tests;

[Order(1)]
public class PrintEvenNumberValues_Tests
{
    [Theory]
    [InlineData(new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 }, new int[] { 2, 4, 6, 8, 10})]
    public void Should_Return(int[] numbers, int[] resultNumbers)
    {
        TextWriter originalConsoleOut = Console.Out;
        using StringWriter consoleWriter = new StringWriter();
        Console.SetOut(consoleWriter);
        
        RecursionFuncs.PrintEvenNumberValues(numbers.ToList(), 0);

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