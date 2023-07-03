using Newtonsoft.Json;
using ReWork_WPF.Models.DataTypes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace ReWork_WPF.Models.Services
{
    public class AppSettingsService : IAppSettingsService
    {
        public async Task<List<LineConfig>> GetAppSettingsAsync()
        {
            try
            {
                using Stream fileStream = File.OpenRead("settings.json");
                using StreamReader reader = new StreamReader(fileStream);

                var json = await reader.ReadToEndAsync();

                var appSettings = JsonConvert.DeserializeObject<List<LineConfig>>(json);

                // Perform additional validation
                string validationMessage = await ValidateAppSettings(appSettings);
                if(validationMessage == "OK")
                {
                    return appSettings;
                }
                else
                {
                    throw new Exception($"Konfigurationsdatei ist nicht Korrekt:\n" +
                        $"{validationMessage}");
                }
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        ///     Validates the application settings.
        /// </summary>
        /// <param name="appSettings">
        ///     The list of 'LineConfig' objects representing the application settings.
        /// </param>
        /// <returns>
        ///     A string with a fitting message for the user.
        ///     The message contains a description of the error if the validation fails, or "OK" if the validation passes.
        /// </returns>
        public Task<string> ValidateAppSettings(List<LineConfig> appSettings)
        {
            if (appSettings == null || appSettings.Count < 1)
            {
                return Task.FromResult("Es wurden keine Daten in der Konfigurationsdatei gefunden. Bitte stellen Sie sicher, dass diese zur Verfügung steht");
            }

            int count = 0;
            foreach (LineConfig line in appSettings)
            {
                count++;
                if (string.IsNullOrWhiteSpace(line.Name))
                {
                    return Task.FromResult($"'Name' wird nicht in der Konfiguration definiert. (Listenelement: '{count}')");
                }

                if (line.ConfigEOL == null)
                {
                    return Task.FromResult($"'ConfigEOL' fehlt in der Konfiguration für die Linie '{line.Name}' oder hat das falsche Format.");
                }

                // Validate ConfigEOL
                if (string.IsNullOrWhiteSpace(line.ConfigEOL.Host) || string.IsNullOrWhiteSpace(line.ConfigEOL.Username) || string.IsNullOrWhiteSpace(line.ConfigEOL.Database) || string.IsNullOrWhiteSpace(line.ConfigEOL.Password) || line.ConfigEOL.Port == null)
                {
                    return Task.FromResult($"Die Werte für 'ConfigEOL' sind in den Einstellungen für die Linie '{line.Name}' inkorrekt oder haben das falsche Format.");
                }

                // Validate TablesToDelete
                if (line.TablesToDelete == null || line.TablesToDelete.Count < 1)
                {
                    return Task.FromResult($"'TablesToDelete' fehlt oder ist leer für die Linie '{line.Name}'.");
                }

                foreach (var item in line.TablesToDelete)
                {
                    if (string.IsNullOrWhiteSpace(item.Table) || item.Columns == null || item.Columns.Count == 0)
                    {
                        return Task.FromResult($"Die einzelnen Element in 'TablesToDelete' fehlen oder haben das falsche Format für die Linie '{line.Name}'.");
                    }
                }

                // Validate FromTable
                if (line.FromTable == null || string.IsNullOrWhiteSpace(line.FromTable.Table) || line.FromTable.Columns == null || line.FromTable.Columns.Count < 1)
                {
                    return Task.FromResult($"'FromTable' fehlt oder hat das falsche Format für die Linie '{line.Name}'.");
                }

                // Validate ToTable
                if (line.ToTable == null || string.IsNullOrWhiteSpace(line.ToTable.Table) || line.ToTable.Columns == null || line.ToTable.Columns.Count < 1)
                {
                    return Task.FromResult($"'ToTable' fehlt oder hat das falsche Format für die Linie '{line.Name}'.");
                }

                // Validate ReworkStatus
                if (line.ReworkStatus == null || line.ReworkStatus.Count == 0)
                {
                    return Task.FromResult($"'ReworkStatus' fehlt oder ist leer für die Linie '{line.Name}'.");
                }

                foreach (var status in line.ReworkStatus)
                {
                    if(status.Key == null || string.IsNullOrWhiteSpace(status.Value))
                    {
                        return Task.FromResult($"Einzelne Elemente in 'ReworkStatus' fehlen oder haben das falsche Format für die Linie '{line.Name}'.");
                    }
                }

                // Validate UnwantedColumns
                if (line.UnwantedColumns == null || line.UnwantedColumns.Count == 0)
                {
                    return Task.FromResult($"'UnwantedColumns' fehlt oder ist leer für die Linie '{line.Name}'.");
                }
            }
            return Task.FromResult("OK");
        }
    }
}