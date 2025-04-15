using FrameworksEducation.AspNetCore.Chapter_2.Exercise_3;
using AppBuilder3_1 = FrameworksEducation.AspNetCore.Chapter_3.Exercise_1.AppBuilder;
using Microsoft.Extensions.Logging;

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
        WebApplication appDefault = AppBuilder3_1.ConfigureFromDefaultBuilder(args);
        WebApplication appSlim = AppBuilder3_1.ConfigureFromSlimBuilder(args);
        WebApplication appEmpty = AppBuilder3_1.ConfigureFromEmptyBuilder(args);

        await Task.WhenAny(appDefault.RunAsync(), appSlim.RunAsync(), appEmpty.RunAsync());
    }


}
