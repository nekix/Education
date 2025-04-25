using FrameworksEducation.AspNetCore.ConsoleClient.Chapter_11.Exercise_1;

namespace FrameworksEducation.AspNetCore.ConsoleClient
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            // Exercise 11
            ApiService service = new ApiService();
            await service.SearchProducts();
        }
    }
}
