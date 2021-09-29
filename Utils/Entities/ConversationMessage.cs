using System;
using System.Collections.Generic;
using Utils.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Utils.Entities
{
    public class ConversationMessage : DataEntity,  IEquatable<ConversationMessage>
    {
        public long Id { get; set; }
        public long ConversationId { get; set; }
        public long ConversationTypeId { get; set; }
        public long GlobalUserId { get; set; }
        public string Content { get; set; }
        public Conversation Conversation { get; set; }
        public ConversationType ConversationType { get; set; }
        public GlobalUser GlobalUser { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        
        public bool Equals(ConversationMessage other)
        {
            if (other == null) return false;
            if (this.Id != other.Id) return false;
            if (this.ConversationId != other.ConversationId) return false;
            if (this.ConversationTypeId != other.ConversationTypeId) return false;
            if (this.GlobalUserId != other.GlobalUserId) return false;
            if (this.Content != other.Content) return false;
            return true;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public class ConversationMessageFilter : FilterEntity
    {
        public IdFilter Id { get; set; }
        public IdFilter ConversationId { get; set; }
        public IdFilter ConversationTypeId { get; set; }
        public IdFilter GlobalUserId { get; set; }
        public StringFilter Content { get; set; }
        public DateFilter CreatedAt { get; set; }
        public DateFilter UpdatedAt { get; set; }
        public List<ConversationMessageFilter> OrFilter { get; set; }
        public ConversationMessageOrder OrderBy {get; set;}
        public ConversationMessageSelect Selects {get; set;}
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum ConversationMessageOrder
    {
        Id = 0,
        Conversation = 1,
        ConversationType = 2,
        GlobalUser = 3,
        Content = 4,
        CreatedAt = 50,
        UpdatedAt = 51,
    }

    [Flags]
    public enum ConversationMessageSelect:long
    {
        ALL = E.ALL,
        Id = E._0,
        Conversation = E._1,
        ConversationType = E._2,
        GlobalUser = E._3,
        Content = E._4,
    }
}
