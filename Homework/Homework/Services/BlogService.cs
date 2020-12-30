using Arch.EntityFrameworkCore.UnitOfWork;
using Homework.Data.Models;
using Homework.Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Arch.EntityFrameworkCore.UnitOfWork.Collections;

namespace Homework.Services
{
    public class BlogService : IBlogService
    {
        //EF Core unitofwork https://github.com/Arch/UnitOfWork

        private readonly IRepository<Articles> _articlesRepository;

        private readonly IRepository<TagCloud> _tagClodRepository;

        private readonly IUnitOfWork _unitOfWork;

        public BlogService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _articlesRepository = unitOfWork.GetRepository<Articles>();
            _tagClodRepository = unitOfWork.GetRepository<TagCloud>();
        }

        public async ValueTask<IList<Articles>> GetAllArticleAsync()
        {
            var temp = await _articlesRepository.GetAll().ToListAsync();
            return temp;
        }

        public async ValueTask<IList<TagCloud>> GetAllTagCloudAsync()
        {
            var temp = await _tagClodRepository.GetAll().ToListAsync();
            return temp;
        }

        public async ValueTask<Articles> GetArticleAsync(Guid Id)
        {
            var temp = await _articlesRepository.GetAll().Where(x => x.Id == Id).FirstOrDefaultAsync();
            return temp;
        }

        public async ValueTask SaveAsync()
        {
            await _unitOfWork.SaveChangesAsync();
        }

        public async ValueTask<IPagedList<Articles>> ToPagedListArticleAsync(int pageIndex, int pageSize)
        {
            return await _articlesRepository.GetPagedListAsync(pageIndex: pageIndex, pageSize: pageSize);
        }

        public async ValueTask<IPagedList<Articles>> ToPagedListArticleBySearchAsync(string keyword, int pageIndex, int pageSize)
        {
            return await _articlesRepository.GetPagedListAsync
                (
                    predicate: (x => x.Tags.Contains(keyword) || x.Title.Contains(keyword) || x.Body.Contains(keyword)),
                    pageIndex: pageIndex,
                    pageSize: pageSize
                );
        }

        public async ValueTask<IPagedList<Articles>> ToPagedListArticleByTagAsync(string tag, int pageIndex, int pageSize)
        {
            return await _articlesRepository.GetPagedListAsync(predicate: x => x.Tags.Contains(tag), pageIndex: pageIndex, pageSize: pageSize);
        }
    }
}