using Microsoft.EntityFrameworkCore;
using NextCommerce.Data.Enums;
using System.Xml.Linq;

namespace NextCommerce.Data.Entities
{
    [Index(nameof(Type), IsUnique = true)]
    public class Setting
    {
        public int Id { get; set; }
        public SettingType Type { get; set; }
        public string? Value { get; set; }
    }
}
