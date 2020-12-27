using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Homework.Models;
using Homework.Services.Interface;

namespace Homework.Controllers
{
    public class HomeController : Controller
    {
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
            //var model = await _blogService.GetAllArticleAsync();
            var model = await _blogService.ToPagedListArticleAsync(page, pageSize);
            return View(model);
        }

        //// GET api/values/Page/5/10
        //[HttpGet("Page/{pageIndex}/{pageSize}")]
        //public async Task<IPagedList<Blog>> Get(int pageIndex, int pageSize)
        //{
        //    // projection
        //    var items = _unitOfWork.GetRepository<Blog>().GetPagedList(b => new { Name = b.Title, Link = b.Url });

        //    return await _unitOfWork.GetRepository<Blog>().GetPagedListAsync(pageIndex: pageIndex, pageSize: pageSize);
        //}

        public IActionResult Privacy()
        {
            return View();
        }
    }
}