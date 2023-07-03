using ReWork_WPF.Models.DataTypes;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ReWork_WPF.Models.Services
{
    /// <summary>
    ///     Represents a service for retrieving application settings from the settings file.
    /// </summary>
    public interface IAppSettingsService
    {
        /// <summary>
        ///     Retrieves the application settings from the 'settings.json' asynchronously.
        /// </summary>
        /// 
        /// <returns>
        ///     A task that represents the asynchronous operation.
        ///     The task result contains a list of 'LineConfig' objects representing the settings for the production lines.
        /// </returns>
        Task<List<LineConfig>> GetAppSettingsAsync();
    }
}
