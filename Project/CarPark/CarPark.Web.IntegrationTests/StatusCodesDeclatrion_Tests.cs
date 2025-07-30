using CarPark.Controllers.Api.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using System.Reflection;

namespace CarPark.Web.IntegrationTests;

public class StatusCodesDeclatrion_Tests
{
    public static class RfcStatusCodes
    {
        public static readonly Dictionary<string, HashSet<int>> ValidCodesByMethod = new()
        {
            ["GET"] = new HashSet<int>  { 200, 400, 401, 403, 404, 500 },
            ["POST"] = new HashSet<int> { 201, 204, 400, 401, 403, 404, 409, 500 },
            ["PUT"] = new HashSet<int>  { 200, 204, 400, 401, 403, 404, 409, 500 },
            ["DELETE"] = new HashSet<int> { 204, 400, 401, 403, 404, 409, 500 },
        };
    }

    [Fact]
    public void All_Controller_Actions_Should_Declare_Valid_RFC_StatusCodes()
    {
        IEnumerable<Type> controllerTypes = Assembly.GetAssembly(typeof(CarPark.Program))!
            .GetTypes()
            .Where(t => t.IsSubclassOf(typeof(ApiBaseController)));

        foreach (Type controller in controllerTypes)
        {
            IEnumerable<MethodInfo> methods = controller
                .GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Where(m => m.IsDefined(typeof(HttpMethodAttribute)));

            foreach (MethodInfo method in methods)
            {
                HttpMethodAttribute? httpMethodAttr = method.GetCustomAttributes<HttpMethodAttribute>().FirstOrDefault();
                string? httpMethod = httpMethodAttr?.HttpMethods.FirstOrDefault();

                if (string.IsNullOrEmpty(httpMethod))
                    continue;

                if (!RfcStatusCodes.ValidCodesByMethod.TryGetValue(httpMethod.ToUpper(), out HashSet<int>? validCodes))
                    continue;

                List<ProducesResponseTypeAttribute> producesAttrs = method.GetCustomAttributes<ProducesResponseTypeAttribute>().ToList();
                Assert.True(producesAttrs.Count != 0, $"Method {controller.Name}.{method.Name} lacks [ProducesResponseType] attributes.");

                foreach (ProducesResponseTypeAttribute attr in producesAttrs)
                {
                    Assert.True(validCodes.Contains(attr.StatusCode), $"Method {controller.Name}.{method.Name} has not valid attribute [ProducesResponseType] with status code: {attr.StatusCode}. " +
                                                                      Environment.NewLine +
                                                                      $"Allowed status codes for {httpMethod.ToUpper()}: {string.Join(", ", validCodes)}.");
                }
            }
        }
    }
}