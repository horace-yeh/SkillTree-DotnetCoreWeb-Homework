using Homework.Data.Models;
using Homework.Services;
using Homework.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Homework.ViewComponents
{
    [ViewComponent]
    public class SummaryViewComponent : ViewComponent
    {
        private readonly IBlogService _blogService;

        public SummaryViewComponent(IBlogService blogService)
        {
            _blogService = blogService;
        }

        //public IViewComponentResult Invoke(Guid guid)
        //{
        //    var data = _blogService.GetArticleAsync(guid);
        //    return View(data);
        //}

        public async Task<IViewComponentResult> InvokeAsync(Guid guid)
        {
            var data = await _blogService.GetArticleAsync(guid);
            return View(data);
        }
    }
}