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
        public IActionResult Create([FromBody] ArticlesCreate model)
        {
            var temp = model;
            return View(model);
        }

        public async Task<IActionResult> Index()
        {
            var model = await _blogService.GetAllArticleAsync();
            return View(model);
        }
    }
}