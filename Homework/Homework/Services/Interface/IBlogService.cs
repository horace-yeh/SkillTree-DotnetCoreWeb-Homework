using Arch.EntityFrameworkCore.UnitOfWork.Collections;
using Homework.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Homework.Services.Interface
{
    public interface IBlogService
    {
        ValueTask<IList<Articles>> GetAllArticleAsync();

        ValueTask<IList<TagCloud>> GetAllTagCloudAsync();

        ValueTask<IList<string>> GetAllTagCloudTextAsync();

        ValueTask<Articles> GetArticleAsync(Guid Id);

        ValueTask SaveAsync();

        ValueTask<IPagedList<Articles>> ToPagedListArticleAsync(int pageIndex, int pageSize);

        ValueTask<IPagedList<Articles>> ToPagedListArticleBySearchAsync(string keyword, int pageIndex, int pageSize);

        ValueTask<IPagedList<Articles>> ToPagedListArticleByTagAsync(string tag, int pageIndex, int pageSize);
    }
}