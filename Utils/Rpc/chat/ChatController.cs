using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Utils.Common;
using Utils.Entities;
using Utils.Enums;
using Utils.Helpers;
using Utils.Service;

namespace Utils.Rpc.chat
{
    public class ChatRoute : Root
    {
        public const string Base = Rpc + Module + "/chat";
        public const string Count = Base + "/count";
        public const string List = Base + "/list";
        public const string Create = Base + "/create";
        public const string Delete = Base + "/delete";
        public const string SaveFile = Base + "/save-file";
    }

    public class ChatController : RpcController
    {
        private IChatService ChatService;
        private IFileService FileService;
        private ICurrentContext CurrentContext;

        public ChatController(ChatService ChatService, IFileService FileService, ICurrentContext CurrentContext)
        {
            this.ChatService = ChatService;
            this.FileService = FileService;
            this.CurrentContext = CurrentContext;
        }

        [Route(ChatRoute.Count), HttpPost]
        public async Task<int> Count([FromBody] Chat_ChatMessageFilterDTO Chat_ChatMessageFilterDTO)
        {
            ChatMessageFilter ChatMessageFilter = new ChatMessageFilter {
                Id = Chat_ChatMessageFilterDTO.Id,
                SenderId = Chat_ChatMessageFilterDTO.SenderId,
                RecipientId = Chat_ChatMessageFilterDTO.RecipientId,
                CreatedAt = Chat_ChatMessageFilterDTO.CreatedAt,
                UpdatedAt = Chat_ChatMessageFilterDTO.UpdatedAt,
            };
            return await ChatService.Count(ChatMessageFilter);
        }

        [Route(ChatRoute.List), HttpPost]
        public async Task<List<Chat_ChatMessageDTO>> List([FromBody] Chat_ChatMessageFilterDTO Chat_ChatMessageFilterDTO)
        {
            ChatMessageFilter ChatMessageFilter = new ChatMessageFilter
            {
                Skip = Chat_ChatMessageFilterDTO.Skip,
                Take = Chat_ChatMessageFilterDTO.Take,
                Id = Chat_ChatMessageFilterDTO.Id,
                SenderId = Chat_ChatMessageFilterDTO.SenderId,
                RecipientId = Chat_ChatMessageFilterDTO.RecipientId,
                CreatedAt = Chat_ChatMessageFilterDTO.CreatedAt,
                UpdatedAt = Chat_ChatMessageFilterDTO.UpdatedAt,
            };
            List<ChatMessage> ChatMessages =  await ChatService.List(ChatMessageFilter);
            List<Chat_ChatMessageDTO> Chat_ChatMessageDTOs = ChatMessages
                .Select(x => new Chat_ChatMessageDTO(x))
                .ToList();
            return Chat_ChatMessageDTOs;
        }

        [Route(ChatRoute.Create), HttpPost]
        public async Task<Chat_ChatMessageDTO> Create([FromBody] Chat_ChatMessageDTO Chat_ChatMessageDTO)
        {
            ChatMessage ChatMessage = new ChatMessage();
            ChatMessage.RecipientId = Chat_ChatMessageDTO.RecipientId;
            ChatMessage.SenderId = Chat_ChatMessageDTO.RecipientId; // doi cai nay thanh currentContext.RowId
            ChatMessage.FileId = Chat_ChatMessageDTO.FileId;
            ChatMessage.Content = Chat_ChatMessageDTO.Content;
            if (ChatMessage.FileId != null)
                ChatMessage.ChatMessageTypeId = ChatMessageTypeEnum.file.Id;
            ChatMessage.CreatedAt = StaticParams.DateTimeNow;
            ChatMessage.UpdatedAt = StaticParams.DateTimeNow;
            ChatMessage = await ChatService.Create(ChatMessage);
            return new Chat_ChatMessageDTO(ChatMessage);
        }

        [Route(ChatRoute.Delete), HttpPost]
        public async Task<bool> Delete([FromBody] Chat_ChatMessageDTO Chat_ChatMessageDTO)
        {
            return await ChatService.Delete(Chat_ChatMessageDTO.Id);
        }

        [Route(ChatRoute.SaveFile), HttpPost]
        public async Task<ActionResult<Chat_FileDTO>> SaveFile(IFormFile file)
        {
            FileInfo fileInfo = new FileInfo(file.FileName);
            Entities.File File = new Entities.File();
            File.Path = $"/Chat/{StaticParams.DateTimeNow.ToString("yyyyMMdd")}/{Guid.NewGuid()}/{fileInfo.Name}"; ;
            MemoryStream memoryStream = new MemoryStream();
            file.CopyTo(memoryStream);
            File.Content = memoryStream.ToArray();
            File = await FileService.Create(File);
            File.Path = "/rpc/utils/file/download" + File.Path;
            return new Chat_FileDTO(File);
        }
    }
}
