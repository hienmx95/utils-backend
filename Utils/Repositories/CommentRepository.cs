using Utils.Common;
using Utils.Helpers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utils.Entities;
using Utils.Models;

namespace Utils.Repositories
{
    public interface ICommentRepository
    {
        Task<int> Count(Guid DiscussionId);
        Task<List<Comment>> List(Guid DiscussionId, OrderType OrderType);
        Task<Comment> Get(long Id);
        Task<bool> Create(Comment Comment);
        Task<bool> Update(Comment Comment);
        Task<bool> Delete(Comment Comment);
    }
    public class CommentRepository : ICommentRepository
    {
        private DataContext DataContext;
        public CommentRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }
        public async Task<int> Count(Guid DiscussionId)
        {
            int count = await DataContext.Comment.Where(p => p.DiscussionId == DiscussionId).CountAsync();
            return count;
        }

        public async Task<List<Comment>> List(Guid DiscussionId, OrderType OrderType)
        {
            IQueryable<CommentDAO> query = DataContext.Comment
                .Where(p => p.DiscussionId == DiscussionId && p.DeletedAt.HasValue == false);
            if (OrderType == OrderType.ASC)
                query = query.OrderBy(x => x.CreatedAt);
            if (OrderType == OrderType.DESC)
                query = query.OrderByDescending(x => x.CreatedAt);

            List<Comment> Comments = await query.Select(p => new Comment
            {
                Id = p.Id,
                Content = p.Content,
                Url = p.Url,
                DiscussionId = p.DiscussionId,
                CreatorId = p.CreatorId,
                Creator = p.Creator == null ? null : new AppUser
                {
                    Id = p.Creator.Id,
                    Username = p.Creator.Username,
                    DisplayName = p.Creator.DisplayName,
                    Avatar = p.Creator.Avatar,
                },
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt,
                DeletedAt = p.DeletedAt,
            }).ToListAsync();
            return Comments;
        }

        public async Task<bool> Create(Comment Comment)
        {
            CommentDAO CommentDAO = new CommentDAO
            {
                DiscussionId = Comment.DiscussionId,
                Content = Comment.Content,
                CreatorId = Comment.CreatorId,
                Url = Comment.Url,
                CreatedAt = StaticParams.DateTimeNow,
                UpdatedAt = StaticParams.DateTimeNow,
                DeletedAt = null,
            };
            DataContext.Comment.Add(CommentDAO);
            await DataContext.SaveChangesAsync();
            Comment.Id = CommentDAO.Id;
            await SaveReference(Comment);
            return true;
        }
        public async Task<bool> Update(Comment Comment)
        {
            CommentDAO CommentDAO = await DataContext.Comment.Where(p => p.Id == Comment.Id).FirstOrDefaultAsync();
            CommentDAO.Content = Comment.Content;
            CommentDAO.Url = Comment.Url;
            CommentDAO.CreatorId = Comment.CreatorId;
            CommentDAO.DiscussionId = Comment.DiscussionId;
            CommentDAO.CreatedAt = StaticParams.DateTimeNow;
            CommentDAO.UpdatedAt = StaticParams.DateTimeNow;
            CommentDAO.DeletedAt = null;
            await DataContext.SaveChangesAsync();
            await SaveReference(Comment);
            return true;
        }
        public async Task<bool> Delete(Comment Comment)
        {
            await DataContext.AppUserCommentMapping.Where(x => x.CommentId == Comment.Id).DeleteFromQueryAsync();
            await DataContext.Comment.Where(p => p.Id == Comment.Id).UpdateFromQueryAsync(x => new CommentDAO { DeletedAt = StaticParams.DateTimeNow });
            return true;
        }

        public async Task<Comment> Get(long Id)
        {
            Comment Comment = await DataContext.Comment.Where(p => p.Id == Id)
               .Select(p => new Comment
               {
                   Id = p.Id,
                   Content = p.Content,
                   DiscussionId = p.DiscussionId,
                   CreatorId = p.CreatorId,
                   Url = p.Url,
                   Creator = p.Creator == null ? null : new AppUser
                   {
                       Id = p.Creator.Id,
                       Username = p.Creator.Username,
                       DisplayName = p.Creator.DisplayName,
                       Avatar = p.Creator.Avatar,
                   },
                   CreatedAt = p.CreatedAt,
                   UpdatedAt = p.UpdatedAt,
                   DeletedAt = p.DeletedAt,
               }).FirstOrDefaultAsync();
            return Comment;
        }

        private async Task SaveReference(Comment Comment)
        {
            await DataContext.AppUserCommentMapping.Where(x => x.CommentId == Comment.Id).DeleteFromQueryAsync();

            if (Comment.AppUserCommentMappings != null)
            {
                List<AppUserCommentMappingDAO> AppUserCommentMappingDAOs = new List<AppUserCommentMappingDAO>();
                foreach (var AppUserCommentMapping in Comment.AppUserCommentMappings)
                {
                    AppUserCommentMappingDAO AppUserCommentMappingDAO = new AppUserCommentMappingDAO()
                    {
                        AppUserId = AppUserCommentMapping.AppUserId,
                        CommentId = Comment.Id
                    };
                    AppUserCommentMappingDAOs.Add(AppUserCommentMappingDAO);
                }

                await DataContext.AppUserCommentMapping.BulkMergeAsync(AppUserCommentMappingDAOs);
            }
        }
    }
}
