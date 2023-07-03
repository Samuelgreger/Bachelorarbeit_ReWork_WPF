using Newtonsoft.Json;

namespace Rework_WPF_Testing.Services
{
    public class TestAppSettingsService : AppSettingsService
    {
        public async Task<List<LineConfig>> GetAppSettingsAsync(string? settingsFile = "settings.json", string json = null)
        {
            try
            {
                if (!File.Exists(settingsFile))
                {
                    throw new FileNotFoundException($"The {settingsFile} file was not found.");
                }

                using Stream fileStream = File.OpenRead(settingsFile);
                using StreamReader reader = new StreamReader(fileStream);

                if (json == null)
                {
                    json = await reader.ReadToEndAsync();
                }

                var appSettings = JsonConvert.DeserializeObject<List<LineConfig>>(json);

                // Perform additional validation
                string validationMessage = await ValidateAppSettings(appSettings);
                if (validationMessage == "OK")
                {
                    return appSettings;
                }
                else
                {
                    throw new Exception($"Konfigurationsdatei ist nicht Korret:\n" +
                        $"{validationMessage}");
                }
            }
            catch
            {
                throw;
            }
        }
    }
}