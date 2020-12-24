using Arch.EntityFrameworkCore.UnitOfWork;
using Homework.Data.Models;
using Homework.Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Homework.DAL;

namespace Homework.Services
{
    public class BlogService : IBlogService
    {
        private readonly IRepository<Articles> _articlesRepository;

        //private readonly BlogDbContext _blogDbContext;
        private readonly IRepository<TagCloud> _tagClodRepository;

        private readonly IUnitOfWork _unitOfWork;

        public BlogService(IUnitOfWork unitOfWork)
        {
            //_blogDbContext = blogDbContext;
            _unitOfWork = unitOfWork;
            _articlesRepository = unitOfWork.GetRepository<Articles>();
            _tagClodRepository = unitOfWork.GetRepository<TagCloud>();
        }

        public async ValueTask<IList<Articles>> GetAllArticleAsync()
        {
            var temp = await _articlesRepository.GetAll().ToListAsync();
            return temp;
        }

        public async ValueTask<Articles> GetArticleAsync(Guid Id)
        {
            var temp = await _articlesRepository.GetAll().Where(x => x.Id == Id).FirstOrDefaultAsync();
            return temp;
        }

        public ValueTask SaveAsync()
        {
            throw new NotImplementedException();
        }
    }
}