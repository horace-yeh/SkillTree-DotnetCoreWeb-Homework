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
        [Required]
        public string Body { get; set; }

        [Required]
        [StringLength(250)]
        public string CoverPhoto { get; set; }

        [DisplayName("建立時間")]
        [Column(TypeName = "datetime")]
        public DateTime CreateDate { get; set; }

        public DayOfWeek DayOfWeek { get; set; }
        public Guid Id { get; set; }
        public string Tags { get; set; }

        [DisplayName("文章標題")]
        [Required]
        [StringLength(100)]
        public string Title { get; set; }
    }
}