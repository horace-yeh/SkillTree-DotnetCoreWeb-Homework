using Homework.Data.Models;
using Homework.Data.ViewModels;
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
    public class PaginationHelper : TagHelper
    {
        public PaginationHelper(IUrlHelperFactory urlHelperFactory, IActionContextAccessor accessor)
        {
            UrlHelperFactory = urlHelperFactory;
            Accessor = accessor;
        }

        public HomeIndexViewModel Info { get; set; }
        private IActionContextAccessor Accessor { get; }
        private IUrlHelperFactory UrlHelperFactory { get; }

        public string GeneratePage(string url, int pageNum, int PageIndex)
        {
            var liClass = pageNum == PageIndex ? "page-item active" : "page-item";
            var html = $@"<li class=""{liClass}""><a class=""page-link"" href=""{url}"">{pageNum + 1}</a>";
            return html;
        }

        public string GeneratePages(HomeIndexViewModel Info)
        {
            var html = "";
            for (var i = 0; i < Info.ArticlesList.TotalPages; i++)
            {
                var url = GenerateUrl(Info.ActionName, Info.Q, Info.QQ, i);
                html += GeneratePage(url, i, Info.ArticlesList.PageIndex);
            }
            return html;
        }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var previous = GeneratePrevious(Info);
            var pages = GeneratePages(Info);
            var next = GenerateNext(Info);
            output.TagName = "div";
            output.Content.SetHtmlContent(
                $@"<nav class=""pagination justify-content-center"">
                          <ul class=""pagination"">
                          {previous}{pages}{next}
                          </ul>
                          </nav>"
                );
        }

        private string GenerateNext(HomeIndexViewModel Info)
        {
            var url = GenerateUrl(Info.ActionName, Info.Q, Info.QQ, Info.ArticlesList.PageIndex + 1);
            var liClass = Info.ArticlesList.HasNextPage ? "page-item" : "page-item disabled";
            var html = $@"<li class=""{liClass}""><a class=""page-link"" href=""{url}"">Next</a></li>";
            return html;
        }

        private string GeneratePrevious(HomeIndexViewModel Info)
        {
            var url = GenerateUrl(Info.ActionName, Info.Q, Info.QQ, Info.ArticlesList.PageIndex - 1);
            var liClass = Info.ArticlesList.HasPreviousPage ? "page-item" : "page-item disabled";
            var html = $@"<li class=""{liClass}""><a class=""page-link"" href=""{url}"">Previous</a></li>";
            return html;
        }

        private string GenerateUrl(string ActionName, string q, string qq, int pageNum)
        {
            var actionContext = Accessor.ActionContext;
            var urlHelper = UrlHelperFactory.GetUrlHelper(actionContext);
            var url = $@"{urlHelper.Action(ActionName)}?page={pageNum}";
            if (ActionName == "Search")
            {
                url += @$"&q={q}";
                return url;
            }
            if (ActionName == "Tags")
            {
                url += @$"&qq={qq}";
                return url;
            }
            return url;
        }
    }
}