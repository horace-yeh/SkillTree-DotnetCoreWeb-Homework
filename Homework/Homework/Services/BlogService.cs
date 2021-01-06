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

        public async ValueTask<ArticlesEdit> GetArticleEditAsync(Guid Id)
        {
            var temp = await _articlesRepository.GetFirstOrDefaultAsync(
                a => new ArticlesEdit
                {
                    Body = a.Body,
                    CoverPhoto = a.CoverPhoto,
                    CreateDate = a.CreateDate,
                    DayOfWeek = a.DayOfWeek,
                    Id = a.Id,
                    Tags = a.Tags,
                    Title = a.Title
                },
                predicate: x => x.Id == Id);
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
            await InserTagCloudSync(articlesCreate.TagsArray);
            //await _unitOfWork.SaveChangesAsync();
            await SaveAsync();
        }

        public async ValueTask EditArticle(ArticlesEdit articlesEdit)
        {
            var originalItem = await GetArticleAsync(articlesEdit.Id);

            //標籤更新機制，先刪除再跑新增機制
            await DeleteTagCloudSync(originalItem.Tags.Split(","));
            await InserTagCloudSync(articlesEdit.TagsArray);

            //上傳圖片處理
            var UploadUrl = _configuration.GetValue<string>("AppSettings:UploadUrl");
            var DeleteFileFlag = (originalItem.CoverPhoto.IndexOf(UploadUrl) > -1);
            if(articlesEdit.CoverPhotoImg != null)
            {
                var fileName = await SaveFile(articlesEdit.CoverPhotoImg);
                var fileUrl = GetUploadUrl() + fileName;             
                if (DeleteFileFlag)
                {
                    var DeletePath = GetSaveFilePath()+ originalItem.CoverPhoto.Replace(UploadUrl,"");
                    DeleteFile(DeletePath);
                }
                originalItem.CoverPhoto = fileUrl;
            }

            originalItem.Tags = string.Join(",", articlesEdit.TagsArray);
            originalItem.DayOfWeek = articlesEdit.CreateDate.DayOfWeek;
            originalItem.Body = articlesEdit.Body;
            originalItem.Title = articlesEdit.Title;

            _articlesRepository.Update(originalItem);
            await SaveAsync();

        }

        public async ValueTask DeleteArticle(Guid Id)
        {
            var originalItem = await GetArticleAsync(Id);
            //上傳檔案處理
            var UploadUrl = _configuration.GetValue<string>("AppSettings:UploadUrl");
            var DeleteFileFlag = (originalItem.CoverPhoto.IndexOf(UploadUrl) > -1);
            if (DeleteFileFlag)
            {
                var DeletePath = GetSaveFilePath() + originalItem.CoverPhoto.Replace(UploadUrl, "");
                DeleteFile(DeletePath);
            }
            //標籤雲更新
            await DeleteTagCloudSync(originalItem.Tags.Split(","));

            _articlesRepository.Delete(Id);
            await SaveAsync();
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

        /// <summary>
        /// newList有original沒有
        /// </summary>
        /// <param name="original"></param>
        /// <param name="newList"></param>
        /// <returns></returns>
        private IList<string> GetInsertTagCloud(IList<string> original, IList<string> newList)
        {
            var diff = newList.Except(original).ToList();
            return diff;
        }

        private string GetSaveFilePath()
        {
            var webRootPath = _webHostEnvironment.WebRootPath;
            var uploadPath = _configuration.GetValue<string>("AppSettings:UploadPath");
            var filePath = webRootPath + uploadPath;
            return filePath;
        }

        /// <summary>
        /// original跟newListe共同都有
        /// </summary>
        /// <param name="original"></param>
        /// <param name="newList"></param>
        /// <returns></returns>
        private IList<string> GetUpdateTagCloud(IList<string> original, IList<string> newList)
        {
            var intersect = original.Intersect(newList).ToList();
            return intersect;
        }

        private string GetUploadUrl()
        {
            var actionContext = _actionContextAccessor.ActionContext;
            var urlHelper = _urlHelperFactory.GetUrlHelper(actionContext);
            var uploadUrl = _configuration.GetValue<string>("AppSettings:UploadUrl");
            var re = urlHelper.Content($@"~{uploadUrl}");
            return re;
        }

        private async Task InserTagCloudSync(IList<string> tags)
        {
            var nowTagCloud = await GetAllTagCloudTextAsync();
            var insertData = GetInsertTagCloud(nowTagCloud, tags);
            var updateData = GetUpdateTagCloud(nowTagCloud, tags);
            foreach (var item in insertData)
            {
                _tagClodRepository.Insert(new TagCloud { Id = Guid.NewGuid(), Amount = 1, Name = item });
            }
            var tempUpdate = await _tagClodRepository.GetAll().Where(x => updateData.Contains(x.Name)).ToListAsync();
            tempUpdate.ForEach(x => x.Amount += 1);
        }

        private async Task DeleteTagCloudSync(IList<string> tags)
        {
            var tempDelete = await _tagClodRepository.GetAll().Where(x => tags.Contains(x.Name)).ToListAsync();
            tempDelete.ForEach(x => x.Amount -= 1);
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