using Homework.Data.DataAnnotaions;
using Homework.Data.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Homework.Data.ViewModels
{
    public class ArticlesCreate : Articles
    {
        [BindProperty] //圖片綁定沒加會撈空
        //[FileExtensions(Extensions = "jpg,png,jpeg", ErrorMessage = "上傳檔案限制為jpg,png,jpeg")] //這個有問題
        [DisplayName("文章照片")]
        [Required(ErrorMessage = "文章照片為必填欄位")]
        [AllowedExtensions(".jpg,.png", "傳檔案限制為jpg,png")]
        public IFormFile CoverPhotoImg { get; set; }

        [DisplayName("文章標籤(可以用，進行分隔)")]
        public List<string> TagsArray { get; set; }
    }

    public class ManagementCreateViewModel
    {
    }
}