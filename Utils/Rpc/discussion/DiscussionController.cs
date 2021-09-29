using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Utils.Common;
using Utils.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Utils.Entities;
using Utils.Models;
using Utils.Service;

namespace Utils.Rpc.discussion
{
    public class DiscussionRoute : Root
    {
        public const string Base = Rpc + Module + "/discussion";
        public const string Count = Base + "/count";
        public const string List = Base + "/list";
        public const string Create = Base + "/create";
        public const string Update = Base + "/update";
        public const string Delete = Base + "/delete";
        public const string SaveFile = Base + "/save-file";
        public const string SingleListAppUser = Base + "/single-list-app-user";
        public const string ListMentioned = Base + "/list-mentioned";
    }

    public class DiscussionController : RpcController
    {
        private ICommentService CommentService;
        private IAppUserService AppUserService;
        private IFileService FileService;
        private DataContext DataContext;
        public DiscussionController(
            ICommentService CommentService, 
            IAppUserService AppUserService,
            IFileService FileService,
            DataContext DataContext)
        {
            this.CommentService = CommentService;
            this.AppUserService = AppUserService;
            this.FileService = FileService;
            this.DataContext = DataContext;
        }

        [Route(DiscussionRoute.Count), HttpPost]
        public async Task<int> Count([FromBody] Discussion_CommentFilterDTO Discussion_CommentFilterDTO)
        {
            Guid DiscussionId = Discussion_CommentFilterDTO.DiscussionId?.Equal ?? Guid.Empty;

            return await CommentService.Count(DiscussionId);
        }


        [Route(DiscussionRoute.List), HttpPost]
        public async Task<List<Discussion_CommentDTO>> ListPost([FromBody] Discussion_CommentFilterDTO Discussion_CommentFilterDTO)
        {
            Guid DiscussionId = Discussion_CommentFilterDTO.DiscussionId?.Equal ?? Guid.Empty;
            List<Comment> Comments = await CommentService.List(DiscussionId, Discussion_CommentFilterDTO.OrderType);
            List<Discussion_CommentDTO> Discussion_CommentDTOs = Comments.Select(p => new Discussion_CommentDTO(p)).ToList();
            return Discussion_CommentDTOs;
        }


        [Route(DiscussionRoute.Create), HttpPost]
        public async Task<Discussion_CommentDTO> Create([FromBody] Discussion_CommentDTO Discussion_CommentDTO)
        {
            Comment Comment = new Comment
            {
                Id = Discussion_CommentDTO.Id,
                DiscussionId = Discussion_CommentDTO.DiscussionId,
                Content = Discussion_CommentDTO.Content,
                Url = Discussion_CommentDTO.Url,
            };

            Comment = await CommentService.Create(Comment);
            Discussion_CommentDTO = new Discussion_CommentDTO(Comment);
            return Discussion_CommentDTO;
        }

        [Route(DiscussionRoute.Update), HttpPost]
        public async Task<Discussion_CommentDTO> Update([FromBody] Discussion_CommentDTO Discussion_CommentDTO)
        {
            Comment Comment = new Comment
            {
                Id = Discussion_CommentDTO.Id,
                DiscussionId = Discussion_CommentDTO.DiscussionId,
                Content = Discussion_CommentDTO.Content,
                Url = Discussion_CommentDTO.Url,
            };

            Comment = await CommentService.Update(Comment);
            Discussion_CommentDTO = new Discussion_CommentDTO(Comment);
            return Discussion_CommentDTO;
        }

        [Route(DiscussionRoute.Delete), HttpPost]
        public async Task<bool> Delete([FromBody] Discussion_CommentDTO Discussion_CommentDTO)
        {
            Comment Comment = new Comment
            {
                Id = Discussion_CommentDTO.Id,
                DiscussionId = Discussion_CommentDTO.DiscussionId,
                Content = Discussion_CommentDTO.Content,
            };

            return await CommentService.Delete(Comment);
        }

        [Route(DiscussionRoute.SaveFile), HttpPost]
        public async Task<ActionResult<Discussion_FileDTO>> SaveFile(IFormFile file)
        {
            FileInfo fileInfo = new FileInfo(file.FileName);
            Entities.File File = new Entities.File();
            File.Path = $"/discussion/{StaticParams.DateTimeNow.ToString("yyyyMMdd")}/{Guid.NewGuid()}/{fileInfo.Name}"; ;
            MemoryStream memoryStream = new MemoryStream();
            file.CopyTo(memoryStream);
            File.Content = memoryStream.ToArray();
            File = await FileService.Create(File);
            File.Path = "/rpc/utils/file/download" + File.Path;
            return new Discussion_FileDTO(File);
        }

        [Route(DiscussionRoute.SingleListAppUser), HttpPost]
        public async Task<List<Discussion_AppUserDTO>> SingleListAppUser([FromBody] Discussion_AppUserFilterDTO Discussion_AppUserFilterDTO)
        {
            AppUserFilter AppUserFilter = new AppUserFilter
            {
                Id = Discussion_AppUserFilterDTO.Id,
                DisplayName = Discussion_AppUserFilterDTO.DisplayName,
                Username = Discussion_AppUserFilterDTO.Username,
                Skip = 0,
                Take= 10,
                OrderBy = AppUserOrder.Username,
                OrderType = OrderType.ASC,
                Selects = AppUserSelect.ALL,
            };
            List<AppUser> AppUsers= await AppUserService.List(AppUserFilter);
            List<Discussion_AppUserDTO> Discussion_AppUserDTOs = AppUsers.Select(a => new Discussion_AppUserDTO(a)).ToList();

            return Discussion_AppUserDTOs;
        }

        [Route(DiscussionRoute.ListMentioned), HttpPost]
        public async Task<List<Discussion_MentionedDTO>> ListMentioned([FromBody] Discussion_MentionedFilterDTO Discussion_MentionedFilterDTO)
        {
            var query1 = from ac in DataContext.AppUserCommentMapping
                         join c in DataContext.Comment on ac.CommentId equals c.Id
                         join au in DataContext.AppUser on c.CreatorId equals au.Id
                         where (Discussion_MentionedFilterDTO.AppUserId.HasValue == false || ac.AppUserId == Discussion_MentionedFilterDTO.AppUserId.Value)
                         select new Discussion_MentionedDTO
                         {
                             AppUserName = c.Creator.DisplayName,
                             Avatar = c.Creator.Avatar,
                             CreatedAt = c.CreatedAt,
                             Url = c.Url,
                             Content = $"{c.Creator.DisplayName} đã nhắc đến bạn trong một bình luận"
                         };


            var result = query1.Skip(0).Take(5).AsEnumerable().ToList();
            return result;
        }
    }
}