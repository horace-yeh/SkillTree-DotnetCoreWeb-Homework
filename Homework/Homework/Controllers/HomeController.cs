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

        //TODO:問題紀錄
        /* 1. EFcore 有辦法抽離成repository service架構嗎，不在主要專案安裝EFCore?
         * 2. Model 專案拆分可實現性? 目前因為需要IPagedList，所以也裝了Arch.EntityFrameworkCore.UnitOfWork
         * 3. 圖片上傳時，使用IFormFile 似乎無法使用FileExtensions檢核檔案類型，有什麼方式可以比較簡單處理這部分
         *    https://github.com/aspnet/Mvc/issues/5117
         * 4. LINQ ROWNumber產生方式，頁面呈現很多時候需要一個流水號
         * 5. Model檢核機制有沒有比較活性的做法，同樣的Model新增某個欄位需要檢核，編輯時則不需要
         */

        #endregion 問題紀錄

        private readonly IBlogService _blogService;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, IBlogService blogService)
        {
            _logger = logger;
            _blogService = blogService;
        }

        public async Task<IActionResult> Detial(Guid id)
        {
            var model = await _blogService.GetArticleAsync(id);
            return View(model);
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