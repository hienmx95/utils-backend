using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Utils.Common;
using System;
using System.Collections.Generic;

namespace Utils.Entities
{
    public class SupplierUser : DataEntity, IEquatable<SupplierUser>
    {
        public long Id { get; set; }
        public string Username { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public string Password { get; set; }
        public long SupplierId { get; set; }
        public long StatusId { get; set; }
        public Guid RowId { get; set; }
        public bool Used { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        public bool Equals(SupplierUser other)
        {
            return other != null && Id == other.Id;
        }
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }

    public class SupplierUserFilter : FilterEntity
    {
        public IdFilter Id { get; set; }
        public StringFilter Username { get; set; }
        public StringFilter DisplayName { get; set; }
        public StringFilter Description { get; set; }
        public StringFilter Password { get; set; }
        public IdFilter SupplierId { get; set; }
        public IdFilter StatusId { get; set; }
        public GuidFilter RowId { get; set; }
        public DateFilter CreatedAt { get; set; }
        public DateFilter UpdatedAt { get; set; }
        public List<SupplierUserFilter> OrFilter { get; set; }
        public SupplierUserOrder OrderBy { get; set; }
        public SupplierUserSelect Selects { get; set; }
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum SupplierUserOrder
    {
        Id = 0,
        Username = 1,
        DisplayName = 2,
        Description = 3,
        Password = 4,
        Supplier = 5,
        Status = 6,
        Row = 10,
        Used = 11,
        CreatedAt = 50,
        UpdatedAt = 51,
    }

    [Flags]
    public enum SupplierUserSelect : long
    {
        ALL = E.ALL,
        Id = E._0,
        Username = E._1,
        DisplayName = E._2,
        Description = E._3,
        Password = E._4,
        Supplier = E._5,
        Status = E._6,
        Row = E._10,
        Used = E._11,
    }
}
