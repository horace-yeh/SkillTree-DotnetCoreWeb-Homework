using Homework.Data.DataAnnotaions;
using Homework.Data.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Homework.Data.ViewModels
{
    public class ArticlesEdit : Articles
    {
        [BindProperty] //圖片綁定沒加會撈空
        [DisplayName("文章照片")]
        [AllowedExtensions(".jpg,.png", "傳檔案限制為jpg,png")]
        public IFormFile CoverPhotoImg { get; set; }

        [DisplayName("文章標籤(可以用，進行分隔)")]
        public List<string> TagsArray { get; set; }
    }

    public class ManagementEditViewModel
    {
    }
}