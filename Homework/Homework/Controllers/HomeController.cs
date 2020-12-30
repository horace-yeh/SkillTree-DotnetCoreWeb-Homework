using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Homework.Models;
using Homework.Services.Interface;
using Homework.Data.ViewModels;

namespace Homework.Controllers
{
    public class HomeController : Controller
    {
        #region 問題紀錄

        /* 1. EFcore 有辦法抽離成repository service架構嗎，不在主要專案安裝EFCore?
         * 2. Model 專案拆分可實現性? 目前因為需要IPagedList，所以也裝了Arch.EntityFrameworkCore.UnitOfWork
         *
         */

        #endregion 問題紀錄

        //TODO: Pagination 需要改寫配合，不同的搜尋機制產生對應的分頁

        private readonly IBlogService _blogService;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, IBlogService blogService)
        {
            _logger = logger;
            _blogService = blogService;
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public async Task<IActionResult> Index(int page = 0, int pageSize = 5)
        {
            ViewBag.ActionName = "Index";
            var model = new HomeIndexViewModel
            {
                ArticlesList = await _blogService.ToPagedListArticleAsync(page, pageSize),
                ActionName = "Index"
            };
            return View(model);
        }

        public async Task<JsonResult> JsonGetTagCloud()
        {
            //"tag": "wpf",
            //"count": "147538"

            var data = await _blogService.GetAllTagCloudAsync();
            var re = data.Select(x => new { tag = x.Name, count = x.Amount.ToString() }).ToList();
            return Json(re);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public async Task<IActionResult> Search(string q, int page = 0, int pageSize = 5)
        {
            ViewBag.q = q;
            var model = new HomeIndexViewModel
            {
                ArticlesList = await _blogService.ToPagedListArticleBySearchAsync(q, page, pageSize),
                ActionName = "Search",
                Q = q
            };
            return View("Index", model);
        }

        public async Task<IActionResult> Tags(string qq, int page = 0, int pageSize = 5)
        {
            var model = new HomeIndexViewModel
            {
                ArticlesList = await _blogService.ToPagedListArticleByTagAsync(qq, page, pageSize),
                ActionName = "Tags",
                QQ = qq
            };
            return View("Index", model);
        }
    }
}