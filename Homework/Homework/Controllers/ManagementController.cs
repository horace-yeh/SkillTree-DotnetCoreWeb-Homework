using Homework.Data.Models;
using Homework.Data.ViewModels;
using Homework.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Homework.Controllers
{
    public class ManagementController : Controller
    {
        private readonly IBlogService _blogService;
        private readonly ILogger<ManagementController> _logger;

        public ManagementController(ILogger<ManagementController> logger, IBlogService blogService)
        {
            _logger = logger;
            _blogService = blogService;
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(ArticlesCreate model)
        {
            //[Bind("CoverPhotoImg")] 圖片綁定沒加會撈空

            ModelState.Remove("CoverPhoto"); // 移除原先圖片必填欄位驗證
            if (ModelState.IsValid)
            {
                var temp = model;
            }

            return View(model);
        }

        public async Task<IActionResult> Index()
        {
            var model = await _blogService.GetAllArticleAsync();
            return View(model);
        }

        public async Task<JsonResult> JsonGetTagCloudText()
        {
            var data = await _blogService.GetAllTagCloudTextAsync();
            return Json(data);
        }
    }
}