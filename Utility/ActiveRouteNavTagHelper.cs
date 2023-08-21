using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace BookStore.Utility;
[HtmlTargetElement("li", Attributes = "asp-controller,asp-action, active-route")]
public class ActiveRouteTagHelper : TagHelper
{

    [ViewContext]
    [HtmlAttributeNotBound]
    public ViewContext? ViewContext { get; set; }

    [HtmlAttributeName("asp-controller")]
    public string? Controller { get; set; }

    [HtmlAttributeName("asp-action")]
    public string? Action { get; set; }

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        if (ViewContext != null)
        {
            var currentController = ViewContext.RouteData.Values["controller"]?.ToString();
            var currentAction = ViewContext.RouteData.Values["action"]?.ToString();

            if (Controller == currentController && Action == currentAction)
            {
                output.Attributes.Add("class", "active");
            }

        }
    }
}