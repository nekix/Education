using FrameworksEducation.AspNetCore.Chapter_2.Exercise_3;
using AppBuilder3_1 = FrameworksEducation.AspNetCore.Chapter_3.Exercise_1.AppBuilder;
using AppBuilder4_1 = FrameworksEducation.AspNetCore.Chapter_4.Exercise_1.AppBuilder;
using AppBuilder5_2 = FrameworksEducation.AspNetCore.Chapter_5.Exercise_2.AppBuilder;
using AppBuilder6_1 = FrameworksEducation.AspNetCore.Chapter_6.Exercise_1.AppBuilder;
using AppBuilder7_1 = FrameworksEducation.AspNetCore.Chapter_7.Exercise_1.AppBuilder;
using AppBuilder9_1 = FrameworksEducation.AspNetCore.Chapter_9.Exercise_1.AppBuilder;
using AppBuilder10_1 = FrameworksEducation.AspNetCore.Chapter_10.Exercise_1.AppBuilder;
using AppBuilder11_1 = FrameworksEducation.AspNetCore.Chapter_11.Exercise_1.AppBuilder;
using AppBuilder13_1 = FrameworksEducation.AspNetCore.Chapter_13.AppBuilder;

namespace FrameworksEducation.AspNetCore;

public class Program
{
    public static async Task Main(string[] args)
    {
        // Chapter 2. Exercise 3.
        //WebApplication appModern = AppBuilder.BuildAndConfigureModernApp(args);
        //WebApplication appLegacy = AppBuilder.BuildAndConfigureLegacyApp(args);

        //await Task.WhenAny(appModern.RunAsync(), appLegacy.RunAsync());

        // Chapter 3. Exercise 1.
        //WebApplication appDefault = AppBuilder3_1.ConfigureFromDefaultBuilder(args);
        //WebApplication appSlim = AppBuilder3_1.ConfigureFromSlimBuilder(args);
        //WebApplication appEmpty = AppBuilder3_1.ConfigureFromEmptyBuilder(args);

        //await Task.WhenAny(appDefault.RunAsync(), appSlim.RunAsync(), appEmpty.RunAsync());

        // Chapter 4. Exercise 1.
        //WebApplication app = AppBuilder4_1.Configure(args);
        //await app.RunAsync();

        // Chapter 5. Exercise 2.
        //WebApplication app = AppBuilder5_2.Configure(args);
        //await app.RunAsync();

        // Chapter 6. Exercise 1.
        //WebApplication app = AppBuilder6_1.Configure(args);
        //await app.RunAsync();

        // Chapter 7. Exercise 1.
        // Пример запроса:
        // http://localhost:5005/products/threads/search?query=x20&tag=%D0%BD%D0%B8%D1%85%D1%80%D0%BE%D0%BC%D0%BE%D0%B2%D0%B0%D1%8F&tag=%D0%93%D0%9E%D0%A1%D0%A2&min-price=10000&max-price=2000000&av-quantity=200
        //WebApplication app = AppBuilder7_1.Configure(args);
        //await app.RunAsync();

        // Chapter 9. Exercise 1.
        //WebApplication app = AppBuilder9_1.Configure(args);
        //await app.RunAsync();

        // Chapter 10. Exercise 1.
        //WebApplication app = AppBuilder10_1.Configure(args);
        //await app.RunAsync();

        // Chapter 11. Exercise 1.
        //WebApplication app = AppBuilder11_1.Configure(args);
        //await app.RunAsync();

        // Chapter 13.
        WebApplication app = AppBuilder13_1.Configure(args);
        await app.RunAsync();
    }


}
