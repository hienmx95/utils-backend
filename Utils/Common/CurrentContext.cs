using System.Collections.Generic;

namespace Utils.Common
{
    public interface ICurrentContext : IServiceScoped
    {
        long UserId { get; set; }
        string UserName { get; set; }
        int TimeZone { get; set; }
        string Language { get; set; }
        Dictionary<long, FilterPermissionDefinition> Filters { get; set; }
    }
    public class CurrentContext : ICurrentContext
    {
        public long UserId { get; set; }
        public string UserName { get; set; }
        public int TimeZone { get; set; }
        public string Language { get; set; }
        public Dictionary<long, FilterPermissionDefinition> Filters { get; set; }
    }
}
