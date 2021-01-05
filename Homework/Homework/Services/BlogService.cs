using Arch.EntityFrameworkCore.UnitOfWork;
using Homework.Data.Models;
using Homework.Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Arch.EntityFrameworkCore.UnitOfWork.Collections;
using Homework.Data.ViewModels;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using System.Security.Policy;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;

namespace Homework.Services
{
    public class BlogService : IBlogService
    {
        //EF Core unitofwork https://github.com/Arch/UnitOfWork

        private readonly IActionContextAccessor _actionContextAccessor;
        private readonly IRepository<Articles> _articlesRepository;
        private readonly IConfiguration _configuration;
        private readonly IRepository<TagCloud> _tagClodRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUrlHelperFactory _urlHelperFactory;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public BlogService(
            IUnitOfWork unitOfWork,
            IConfiguration configuration,
            IWebHostEnvironment webHostEnvironment,
            IActionContextAccessor actionContextAccessor,
            IUrlHelperFactory urlHelperFactory
        )
        {
            _unitOfWork = unitOfWork;
            _articlesRepository = unitOfWork.GetRepository<Articles>();
            _tagClodRepository = unitOfWork.GetRepository<TagCloud>();
            _configuration = configuration;
            _webHostEnvironment = webHostEnvironment;
            _actionContextAccessor = actionContextAccessor;
            _urlHelperFactory = urlHelperFactory;
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

        public async ValueTask<IList<string>> GetAllTagCloudTextAsync()
        {
            var temp = await _tagClodRepository.GetAll().Select(x => x.Name).ToListAsync();
            return temp;
        }

        public async ValueTask<Articles> GetArticleAsync(Guid Id)
        {
            var temp = await _articlesRepository.GetFirstOrDefaultAsync(predicate: x => x.Id == Id);
            return temp;
        }

        public async ValueTask SaveArticle(ArticlesCreate articlesCreate)
        {
            var fileName = await SaveFile(articlesCreate.CoverPhotoImg);
            var fileUrl = GetUploadUrl() + fileName;
            articlesCreate.Tags = string.Join(",", articlesCreate.TagsArray);
            articlesCreate.CoverPhoto = fileUrl;
            articlesCreate.Id = Guid.NewGuid();
            articlesCreate.DayOfWeek = articlesCreate.CreateDate.DayOfWeek;
            _articlesRepository.Insert(articlesCreate);
            //tag未更新
            await _unitOfWork.SaveChangesAsync();
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

        private void DeleteFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }

        private string GetGuidFileName(IFormFile file)
        {
            var ext = Path.GetExtension(file.FileName);
            var guid = Guid.NewGuid().ToString("N");
            var newFileName = guid + ext;
            return newFileName;
        }

        private string GetSaveFilePath()
        {
            var webRootPath = _webHostEnvironment.WebRootPath;
            var uploadPath = _configuration.GetValue<string>("AppSettings:UploadPath");
            var filePath = webRootPath + uploadPath;
            return filePath;
        }

        private string GetUploadUrl()
        {
            var actionContext = _actionContextAccessor.ActionContext;
            var urlHelper = _urlHelperFactory.GetUrlHelper(actionContext);
            var uploadUrl = _configuration.GetValue<string>("AppSettings:UploadUrl");
            var re = urlHelper.Content($@"~{uploadUrl}");
            return re;
        }

        private async Task<string> SaveFile(IFormFile file)
        {
            var filePath = GetSaveFilePath();
            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }
            var newFileName = GetGuidFileName(file);
            var savePath = filePath + newFileName;

            using (var stream = new FileStream(savePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            return newFileName;
        }
    }
}