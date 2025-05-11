using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace FrameworksEducation.AspNetCore.Chapter_13.Services;

public class PrefixingRazorPagesRouteModelConvention : IPageRouteModelConvention
{
    private readonly string _prefix;

    public PrefixingRazorPagesRouteModelConvention(string prefix)
    {
        _prefix = prefix;
    }

    public void Apply(PageRouteModel model)
    { 
        List<SelectorModel> selectors = model.Selectors
            .Select(s => new SelectorModel
            {
                AttributeRouteModel = new AttributeRouteModel
                {
                    Template = AttributeRouteModel.CombineTemplates(
                        _prefix,
                        s.AttributeRouteModel?.Template)
                }
            })
            .ToList();

        model.Selectors.Clear();

        foreach (SelectorModel selector in selectors)
        {
            model.Selectors.Add(selector);
        }
    }
}