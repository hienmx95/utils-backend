using Utils.Common;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utils.Entities;
using Utils.Models;

namespace Utils.Repositories
{
    public interface IFileRepository
    {
        Task<int> Count(FileFilter FileFilter);
        Task<List<File>> List(FileFilter FileFilter);
        Task<File> Get(long Id);
        Task<File> Download(long Id);
        Task<bool> Create(File File);
        Task<bool> Delete(long Id);
    }
    public class FileRepository : IFileRepository
    {
        private DataContext context;
        private readonly IMongoClient MongoClient = null;
        public FileRepository(DataContext context, IMongoClient MongoClient)
        {
            this.context = context;
            this.MongoClient = MongoClient;
        }
        public async Task<int> Count(FileFilter FileFilter)
        {
            IQueryable<FileDAO> query = context.File.AsNoTracking();
            query = DynamicFilter(query, FileFilter);
            return await query.CountAsync();
        }

        public async Task<List<File>> List(FileFilter FileFilter)
        {
            IQueryable<FileDAO> query = context.File.AsNoTracking();
            query = DynamicFilter(query, FileFilter);
            query = DynamicOrder(query, FileFilter);
            List<File> Files = await DynamicSelect(query, FileFilter);
            return Files;
        }

        private IQueryable<FileDAO> DynamicFilter(IQueryable<FileDAO> query, FileFilter filter)
        {
            if (filter.Id != null)
                query = query.Where(q => q.Id, filter.Id);
            if (filter.Path != null)
                query = query.Where(q => q.Path, filter.Path?.ToLower());
            if (filter.Level != null)
                query = query.Where(q => q.Level, filter.Level);
            if (filter.IsFile != null)
                query = query.Where(q => q.IsFile == filter.IsFile.Value);
            return query;
        }
        private IQueryable<FileDAO> DynamicOrder(IQueryable<FileDAO> query, FileFilter filter)
        {
            switch (filter.OrderType)
            {
                case OrderType.ASC:
                    switch (filter.OrderBy)
                    {
                        case FileOrder.Id:
                            query = query.OrderBy(q => q.Id);
                            break;
                        case FileOrder.Path:
                            query = query.OrderBy(q => q.Path);
                            break;
                        case FileOrder.Level:
                            query = query.OrderBy(q => q.Level);
                            break;
                        case FileOrder.IsFile:
                            query = query.OrderBy(q => q.IsFile);
                            break;
                    }
                    break;
                case OrderType.DESC:
                    switch (filter.OrderBy)
                    {
                        case FileOrder.Id:
                            query = query.OrderByDescending(q => q.Id);
                            break;
                        case FileOrder.Path:
                            query = query.OrderByDescending(q => q.Path);
                            break;
                        case FileOrder.Level:
                            query = query.OrderByDescending(q => q.Level);
                            break;
                        case FileOrder.IsFile:
                            query = query.OrderByDescending(q => q.IsFile);
                            break;
                    }
                    break;
            }
            return query;
        }
        private async Task<List<File>> DynamicSelect(IQueryable<FileDAO> query, FileFilter filter)
        {
            query = query.Skip(filter.Skip).Take(filter.Take);
            List<File> Files = await query.Select(q => new File
            {
                Id = q.Id,
                IsFile = q.IsFile,
                Level = q.Level,
                Path = q.Path,
                Name = q.Name,
                RowId = q.RowId,
            }).ToListAsync();
            return Files;
        }

        public async Task<File> Get(long Id)
        {
            FileDAO fileDAO = await context.File.Where(f => f.Id == Id).FirstOrDefaultAsync();
            File file = new File
            {
                Id = fileDAO.Id,
                Name = fileDAO.Name,
                IsFile = fileDAO.IsFile,
                Path = fileDAO.Path,
                Level = fileDAO.Level,
                RowId = fileDAO.RowId,
            };
            return file;
        }

        public async Task<File> Download(long Id)
        {
            FileDAO fileDAO = await context.File.Where(f => f.Id == Id).FirstOrDefaultAsync();
            File file = new File
            {
                Id = fileDAO.Id,
                Name = fileDAO.Name,
                IsFile = fileDAO.IsFile,
                Path = fileDAO.Path,
                Level = fileDAO.Level,
                RowId = fileDAO.RowId,
                CreatedAt = fileDAO.CreatedAt,
            };

            IMongoDatabase MongoDatabase = MongoClient.GetDatabase($"File_{file.CreatedAt.ToString("yyyyMMdd")}");
            GridFSBucket gridFSBucket = new GridFSBucket(MongoDatabase);
            file.Content = await gridFSBucket.DownloadAsBytesAsync(new ObjectId(fileDAO.GridId));
            return file;
        }

        public async Task<bool> Create(File File)
        {
            List<string> paths = File.Path.Split("/").Select(p => p.ToLower()).ToList();
            List<string> dbPaths = new List<string>();
            for (int i = 0; i < paths.Count; i++)
            {
                string path = string.Join("/", paths.Skip(0).Take(i + 1).ToList());
                dbPaths.Add(path);
            }
            List<FileDAO> fileDAOs = context.File.Where(f => dbPaths.Contains(f.Path)).ToList();
            IMongoDatabase MongoDatabase = MongoClient.GetDatabase($"File_{DateTime.Now.ToString("yyyyMMdd")}");
            GridFSBucket gridFSBucket = new GridFSBucket(MongoDatabase);
            ObjectId objectId = await gridFSBucket.UploadFromBytesAsync(File.Key, File.Content);
            for (int i = 0; i < paths.Count; i++)
            {
                string path = string.Join("/", paths.Skip(0).Take(i + 1).ToList());
                FileDAO fileDAO = fileDAOs.Where(f => f.Path == path).FirstOrDefault();
                if (fileDAO == null)
                {
                    fileDAO = new FileDAO
                    {
                        IsFile = false,
                        Level = i,
                        Path = path,
                        Name = paths[i],
                        RowId = Guid.NewGuid(),
                        CreatedAt = DateTime.Now,
                    };
                    context.File.Add(fileDAO);
                }
                if (i == paths.Count - 1)
                {
                    fileDAO.GridId = objectId.ToString();
                    fileDAO.IsFile = true;
                    fileDAO.MimeType = File.MimeType;
                }
                File.RowId = fileDAO.RowId;
            }
            await context.SaveChangesAsync();
            File.Id = await context.File.Where(f => f.Path == File.Path).Select(f => f.Id).FirstOrDefaultAsync();
            return true;
        }

        public async Task<bool> Delete(long Id)
        {
            FileDAO fileDAO = await context.File.Where(f => f.Id == Id).FirstOrDefaultAsync();
            context.File.Remove(fileDAO);
            await context.SaveChangesAsync();
            IMongoDatabase MongoDatabase = MongoClient.GetDatabase($"File_{fileDAO.CreatedAt.ToString("yyyyMMdd")}");
            GridFSBucket gridFSBucket = new GridFSBucket(MongoDatabase);
            await gridFSBucket.DeleteAsync(fileDAO.GridId);
            return true;
        }
    }
}
