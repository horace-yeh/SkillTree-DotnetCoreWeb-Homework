using Homework.Data.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Homework.Data.ViewModels
{
    public class ArticlesCreate : Articles
    {
        [DisplayName("文章照片")]
        [Required(ErrorMessage = "文章照片為必填欄位")]
        public IFormFile CoverPhotoImg { get; set; }
    }

    public class ManagementCreate
    {
    }
}