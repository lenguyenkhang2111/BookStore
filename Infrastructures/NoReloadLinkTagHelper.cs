using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace BookStore.Infrastructures;

[HtmlTargetElement("a", Attributes = "asp-controller, asp-action, no-link")]
public class NoReloadLinkTagHelper : TagHelper
{
    private readonly IUrlHelperFactory urlHelperFactory;

    [ViewContext]
    [HtmlAttributeNotBound]
    public ViewContext? ViewContext { get; set; }

    public NoReloadLinkTagHelper(IUrlHelperFactory urlHelperFactory)
    {
        this.urlHelperFactory = urlHelperFactory;
    }

    [HtmlAttributeName("asp-controller")]
    public string? Controller { get; set; }

    [HtmlAttributeName("asp-action")]
    public string? Action { get; set; }

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        if (ViewContext != null)
        {
            var urlHelper = urlHelperFactory.GetUrlHelper(ViewContext);

            var currentController = ViewContext.RouteData.Values["controller"]?.ToString();
            var currentAction = ViewContext.RouteData.Values["action"]?.ToString();

            if (Controller == currentController && Action == currentAction)
            {
                output.Attributes.Add("href", "#");
                output.Attributes.Add("onclick", "return false;");
            }
            else
            {
                var url = urlHelper.Action(Action, Controller);
                output.Attributes.Add("href", url);
            }
        }
    }
}
