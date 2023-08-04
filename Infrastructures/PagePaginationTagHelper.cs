using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using BookStore.Models.ViewModels;

namespace BookStore.Infrastructures;
[HtmlTargetElement("nav", Attributes = "page-model")]
public class PaginationTagHelper : TagHelper
{
    private IUrlHelperFactory urlHelperFactory;

    public PaginationTagHelper(IUrlHelperFactory helperFactory)
    {
        urlHelperFactory = helperFactory;
    }

    [ViewContext]
    [HtmlAttributeNotBound]
    public ViewContext? ViewContext { get; set; }

    public PagingInfo? PageModel { get; set; }

    public string? PageAction { get; set; }

    [HtmlAttributeName(DictionaryAttributePrefix = "page-url-")]
    public Dictionary<string, object> PageUrlValues { get; set; } = new Dictionary<string, object>();

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        if (ViewContext != null && PageModel != null)
        {
            IUrlHelper urlHelper = urlHelperFactory.GetUrlHelper(ViewContext);
            TagBuilder ulTag = new("ul");
            ulTag.AddCssClass("pagination");

            // Add Previous Page link
            TagBuilder prevLiTag = new("li");
            TagBuilder prevATag = new("a");
            PageUrlValues["bookPage"] = PageModel.CurrentPage - 1;
            prevATag.Attributes["href"] = urlHelper.Action(PageAction, PageUrlValues);
            prevATag.Attributes["aria-label"] = "Previous";
            prevATag.InnerHtml.AppendHtml("<span aria-hidden=\"true\"><i class=\"icon-long-arrow-left\"></i></span> Prev ");
            prevLiTag.AddCssClass("page-item");
            prevATag.AddCssClass("page-link page-link-prev");
            if (PageModel.CurrentPage == 1) prevLiTag.AddCssClass("disabled");
            prevLiTag.InnerHtml.AppendHtml(prevATag);
            ulTag.InnerHtml.AppendHtml(prevLiTag);


            // Add Page number links
            for (int i = 1; i <= PageModel.TotalPages; i++)
            {
                TagBuilder liTag = new("li");
                TagBuilder aTag = new("a");
                PageUrlValues["bookPage"] = i;
                aTag.Attributes["href"] = urlHelper.Action(PageAction, PageUrlValues);
                aTag.AddCssClass("page-link");
                aTag.InnerHtml.Append(i.ToString());
                liTag.AddCssClass("page-item");
                if (i == PageModel.CurrentPage)
                {
                    liTag.AddCssClass("active");
                    liTag.Attributes["aria-current"] = "page";
                }
                liTag.InnerHtml.AppendHtml(aTag);
                ulTag.InnerHtml.AppendHtml(liTag);
            }

            // Add "Total Pages" indicator
            TagBuilder totalLiTag = new("li");
            totalLiTag.AddCssClass("page-item-total");
            totalLiTag.InnerHtml.Append("of " + PageModel.TotalPages);
            ulTag.InnerHtml.AppendHtml(totalLiTag);

            // Add Next Page link
            TagBuilder nextLiTag = new("li");
            TagBuilder nextATag = new("a");
            PageUrlValues["bookPage"] = PageModel.CurrentPage + 1;
            nextATag.Attributes["href"] = urlHelper.Action(PageAction, PageUrlValues);
            nextATag.Attributes["aria-label"] = "Next";
            nextATag.InnerHtml.AppendHtml(" Next <span aria-hidden=\"true\"><i class=\"icon-long-arrow-right\"></i></span>");
            nextLiTag.AddCssClass("page-item");
            nextATag.AddCssClass("page-link page-link-next");
            nextLiTag.InnerHtml.AppendHtml(nextATag);
            if (PageModel.CurrentPage >= PageModel.TotalPages) nextLiTag.AddCssClass("disabled");
            ulTag.InnerHtml.AppendHtml(nextLiTag);

            //Output final ul
            output.Content.AppendHtml(ulTag);
        }
    }
}