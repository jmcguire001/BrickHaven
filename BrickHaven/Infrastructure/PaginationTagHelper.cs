using BrickHaven.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

// This will help us build custom <a> tags
namespace BrickHaven.Infrastructure
{
    [HtmlTargetElement("div", Attributes = "page-model")] // This is looking for a div with the attribute 'page-model'
    public class PaginationTagHelper : TagHelper
    {
        private IUrlHelperFactory urlHelperFactory; // Helps us build the URL we want to use

        public PaginationTagHelper(IUrlHelperFactory temp)
        {
            urlHelperFactory = temp;
        }

        [ViewContext]
        [HtmlAttributeNotBound] // Users won't be able to view the context; it won't be bound to the HTML
        public ViewContext? ViewContext { get; set; }
        public string? PageAction { get; set; } // We can get this from cshtml page
        [HtmlAttributeName(DictionaryAttributePrefix = "page-url-")] // Looks for this in the route, and stores info in the dictionary
        public Dictionary<string, object> PageUrlValues { get; set; } = new Dictionary<string, object>(); // This will hold the page number
        public PaginationInfo PageModel { get; set; } // PageModel comes from page-model attribute in cshtml
        public bool PageClassEnabled { get; set; } = false;
        public string? PageClass { get; set; } = String.Empty;
        public string PageClassNormal { get; set; } = String.Empty;
        public string PageClassSelected { get; set; } = String.Empty;

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (ViewContext != null && PageModel != null)
            {
                IUrlHelper urlHelper = urlHelperFactory.GetUrlHelper(ViewContext);

                TagBuilder result = new TagBuilder("div");

                for (int i = 1; i <= PageModel.TotalNumPages; i++)
                {
                    TagBuilder tag = new TagBuilder("a"); // Build an <a> tag
                    PageUrlValues["pageNum"] = i; // Set the page number
                    tag.Attributes["href"] = urlHelper.Action(PageAction, PageUrlValues);

                    if (PageClassEnabled)
                    {
                        tag.AddCssClass(PageClass);
                        tag.AddCssClass(i == PageModel.CurrentPage ? PageClassSelected : PageClassNormal);
                    }

                    tag.InnerHtml.Append(i.ToString());
                    result.InnerHtml.AppendHtml(tag);
                }

                output.Content.AppendHtml(result.InnerHtml);
            }
        }
    }
}
