using NextCommerce.Data.Enums;

namespace NextCommerce.Services.Interfaces
{
    public interface ISettingsService
    {
        Task<string?> GetAsync(SettingType type);
        Task<bool> IsSetAsync(SettingType type);
    }
}
