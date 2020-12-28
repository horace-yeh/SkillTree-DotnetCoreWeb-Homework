using Homework.DAL;
using Homework.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Homework
{
    public static class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new BlogDbContext(
                                                   serviceProvider.GetRequiredService<
                                                       DbContextOptions<BlogDbContext>>()))
            {
                if (context.Articles.Any())
                {
                    return;
                }

                var tmp = new List<Articles>();
                var tags = new StringBuilder();
                for (int i = 1; i <= 20; i++)
                {
                    var tag = RandomTag();
                    tmp.Add(new Articles
                    {
                        Id = Guid.NewGuid()
                              ,
                        Title = $"第{i}筆部落格"
                              ,
                        Body = LoremIpsum()
                              ,
                        DayOfWeek = DayOfWeek.Wednesday
                              ,
                        CoverPhoto = $"http://placehold.it/750x300?text=This is {i}"
                              ,
                        CreateDate = DateTime.UtcNow.AddDays(i)
                              ,
                        Tags = tag
                               ,
                    });
                    tags.Append(tag + ",");
                }

                context.Articles.AddRange(tmp);
                context.SaveChanges();
            }
        }

        private static string LoremIpsum()
        {
            using var webClient = new WebClient();
            var baseUri = "http://more.handlino.com/sentences.json?n=8";
            webClient.Encoding = Encoding.UTF8;
            var jsonString = webClient.DownloadString(new Uri(baseUri));
            using JsonDocument doc = JsonDocument.Parse(jsonString);
            var root = doc.RootElement;
            var students = root.GetProperty("sentences");
            var loremIpsum = students.EnumerateArray().Select(d => d.ToString()).ToList();
            return $"<p>{string.Join("</p><p>", loremIpsum)}</p>";
        }

        private static string RandomTag()
        {
            var tags = new List<string>()
                       {
                           "SkillTree"
                         , "twMVC"
                         , "demoshop"
                         , "Dotblogs"
                         , "MVC"
                         , "RazorPage"
                       };
            //先決定要取幾個
            var take = Enumerable.Range(1, 5).OrderBy(d => Guid.NewGuid()).First();
            //再亂數取幾個
            return string.Join(",", tags.OrderBy(d => Guid.NewGuid()).Take(take));
        }

        #region TagCloud 資料相關

        /* 空空自己產
         * Declare @temp nvarchar(50)= 'RazorPage'
         * insert into[BlogSkillTree].[dbo].[TagCloud]
         * select NEWID(),count(1),@temp
         * from[dbo].[Articles] where Tags like '%'+@temp+'%'
         */

        #endregion TagCloud 資料相關
    }
}