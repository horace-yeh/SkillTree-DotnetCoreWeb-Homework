using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Homework.Data.Models
{
    // TODO 為何使用partial class
    public partial class Articles
    {
        [DisplayName("文章內文")]
        [Required(ErrorMessage = "文章內文為必填欄位")]
        public string Body { get; set; }

        [DisplayName("文章照片")]
        [Required(ErrorMessage = "文章照片為必填欄位")]
        [StringLength(250)]
        public string CoverPhoto { get; set; }

        [DisplayName("建立時間")]
        [Column(TypeName = "datetime")]
        public DateTime CreateDate { get; set; }

        public DayOfWeek DayOfWeek { get; set; }
        public Guid Id { get; set; }

        [DisplayName("文章標籤")]
        public string Tags { get; set; }

        [DisplayName("文章標題")]
        [Required(ErrorMessage = "文章標題為必填欄位")]
        [StringLength(maximumLength: 100, ErrorMessage = "文章標題長度限制為100")]
        public string Title { get; set; }
    }
}