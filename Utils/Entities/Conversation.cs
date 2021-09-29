using System;
using System.Collections.Generic;
using Utils.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Utils.Entities
{
    public class Conversation : DataEntity,  IEquatable<Conversation>
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public List<ConversationParticipant> ConversationParticipants { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        
        public bool Equals(Conversation other)
        {
            if (other == null) return false;
            if (this.Id != other.Id) return false;
            if (this.Name != other.Name) return false;
            if (this.ConversationParticipants?.Count != other.ConversationParticipants?.Count) return false;
            else if (this.ConversationParticipants != null && other.ConversationParticipants != null)
            {
                for (int i = 0; i < ConversationParticipants.Count; i++)
                {
                    ConversationParticipant ConversationParticipant = ConversationParticipants[i];
                    ConversationParticipant otherConversationParticipant = other.ConversationParticipants[i];
                    if (ConversationParticipant == null && otherConversationParticipant != null)
                        return false;
                    if (ConversationParticipant != null && otherConversationParticipant == null)
                        return false;
                    if (ConversationParticipant.Equals(otherConversationParticipant) == false)
                        return false;
                }
            }
            return true;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public class ConversationFilter : FilterEntity
    {
        public IdFilter Id { get; set; }
        public StringFilter Name { get; set; }
        public DateFilter CreatedAt { get; set; }
        public DateFilter UpdatedAt { get; set; }
        public List<ConversationFilter> OrFilter { get; set; }
        public ConversationOrder OrderBy {get; set;}
        public ConversationSelect Selects {get; set;}
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum ConversationOrder
    {
        Id = 0,
        Name = 1,
        CreatedAt = 50,
        UpdatedAt = 51,
    }

    [Flags]
    public enum ConversationSelect:long
    {
        ALL = E.ALL,
        Id = E._0,
        Name = E._1,
    }
}
