using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Homework.Helper.Tag
{
    [HtmlTargetElement("tags")]
    public class TagsHelper : TagHelper
    {
        public TagsHelper(IUrlHelperFactory urlHelperFactory, IActionContextAccessor accessor)
        {
            UrlHelperFactory = urlHelperFactory;
            Accessor = accessor;
        }

        private IActionContextAccessor Accessor { get; }

        private IUrlHelperFactory UrlHelperFactory { get; }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            var actionContext = Accessor.ActionContext;
            var urlHelper = UrlHelperFactory.GetUrlHelper(actionContext);
            var content = await output.GetChildContentAsync();
            var target = content.GetContent();
            var url = $"{urlHelper.Action("Tags", "Home")}?qq={target}";
            output.Attributes.SetAttribute("href", url);
            output.TagName = "a";
        }
    }
}