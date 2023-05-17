using Microsoft.EntityFrameworkCore;
using NextCommerce.Data;
using NextCommerce.Data.Enums;
using NextCommerce.Services.Interfaces;

namespace NextCommerce.Services
{
    public class SettingsService : ISettingsService
    {
        public readonly ApplicationDbContext _context;
        private readonly ILogger<SettingsService> _logger;

        public SettingsService(ApplicationDbContext context, ILogger<SettingsService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<bool> IsSetAsync(SettingType type)
        {
            var settingRecord = await _context.Settings.FirstOrDefaultAsync(s => s.Type == type);

            return settingRecord != null && !string.IsNullOrEmpty(settingRecord.Value);
        }

        public async Task<string?> GetAsync(SettingType type)
        {
            var settingRecord = await _context.Settings.FirstOrDefaultAsync(s => s.Type == type);

            return settingRecord?.Value;
        }
    }
}
