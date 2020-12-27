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

        ValueTask<Articles> GetArticleAsync(Guid Id);

        ValueTask SaveAsync();

        ValueTask<IPagedList<Articles>> ToPagedListArticleAsync(int pageNumber, int pageSize);
    }
}