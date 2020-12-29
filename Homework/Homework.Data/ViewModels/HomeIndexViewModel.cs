using Arch.EntityFrameworkCore.UnitOfWork.Collections;
using Homework.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Homework.Data.ViewModels
{
    public class HomeIndexViewModel
    {
        public string ActionName { get; set; }
        public IPagedList<Articles> ArticlesList { get; set; }
        public string Q { get; set; }
        public string QQ { get; set; }
    }
}