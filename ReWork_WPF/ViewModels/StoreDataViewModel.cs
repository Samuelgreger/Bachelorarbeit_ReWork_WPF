using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MySql.Data.MySqlClient;
using NLog;
using ReWork_WPF.Exceptions;
using ReWork_WPF.Models.DataTypes;
using ReWork_WPF.Models.Services;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;

namespace ReWork_WPF.ViewModels
{
    /// <summary>
    ///     Represents the view model for storing data in the application.
    /// </summary>
    public partial class StoreDataViewModel : BaseViewModel
    {
        IAppSettingsService appSettingsService;
        IDatabaseService databaseService;
        private readonly Logger logger;
        private readonly IMessageDisplayService messageDisplayService;

        public ObservableCollection<LineConfig> Lines { get; } = new();

        [ObservableProperty]
        public LineConfig selectedLine;

        [ObservableProperty]
        public ReworkStatus selectedFailureReason;

        [ObservableProperty]
        public string seriesNum;

        [ObservableProperty]
        public string partNum;

        /// <summary>
        ///     Initializes a new instance of the <see cref="StoreDataViewModel"/> class.
        /// </summary>
        /// <param name="appSettingsService">The app settings service.</param>
        /// <param name="databaseService">The database service.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="messageDisplayService">The message display service.</param>
        public StoreDataViewModel(IAppSettingsService appSettingsService, IDatabaseService databaseService, Logger logger, IMessageDisplayService messageDisplayService)
        {
            this.logger = logger;
            this.appSettingsService = appSettingsService;
            this.databaseService = databaseService;
            this.messageDisplayService = messageDisplayService;

            Title = "EoL - Nachbearbeitung";

            // call the method to get the settings on startup
            Task.Run(GetSettings);
        }

        /// <summary>
        ///     Retrieves the application settings and populates the <see cref="Lines"/> collection.
        /// </summary>
        [RelayCommand]
        async Task GetSettings()
        {
            if (IsBusy)
                return;
            try
            {
                IsBusy = true;

                var lines = await appSettingsService.GetAppSettingsAsync();

                if (Lines.Count != 0)
                    Lines.Clear();

                foreach (var line in lines)
                {
                    Lines.Add(line);
                }
                Debug.WriteLine("==> Settings for the Lines loaded");
            }
            catch (Exception ex)
            {
                logger.Error($"{ex.GetType().FullName} |\n {ex.Message} | \n StackTrace: {ex.StackTrace}");
                
                string message = "Fehler beim Laden der Konfigurationen. Bitte stellen Sie sicher das die Konfigurationsdatei zur verfügung steht.";
                Type exceptionType = ex.GetType();
                
                if (exceptionType == typeof(Exception))
                    await messageDisplayService.ShowMessage(ex.Message, "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                else await messageDisplayService.ShowMessage(message, "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsBusy = false;
            }
        }

        /// <summary>
        ///     Stores the data in the database based on the selected line configuration and input values.
        /// </summary>
        [RelayCommand]
        async Task StoreData()
        {
            string checkedResult;
            string statusMessage = "Daten wurden Erfolgreich gespeichert";
            string status = "Erfolgreich";

            if (IsBusy)
                return;
            try
            {
                IsBusy = true;
                
                checkedResult = CheckInput();
                if (checkedResult != "OK")
                {
                    statusMessage = checkedResult;
                    status = "Fehler";
                    throw new UserInputException(checkedResult);
                }

                string result;

                if (string.IsNullOrWhiteSpace(PartNum))
                {
                    result = await databaseService.StoreDataInDb(SelectedLine.ConfigEOL, SelectedLine.TablesToDelete, SelectedLine.UnwantedColumns, SelectedFailureReason.Key, SeriesNum, SelectedLine.FromTable, SelectedLine.ToTable);
                }
                else
                {
                    result = await databaseService.StoreDataInDb(SelectedLine.ConfigEOL, SelectedLine.TablesToDelete, SelectedLine.UnwantedColumns, SelectedFailureReason.Key, SeriesNum, SelectedLine.FromTable, SelectedLine.ToTable, PartNum);
                }

                if (result != "OK")
                {
                    statusMessage = result;
                    status = "Fehler";
                    throw new Exception(result);
                }
                SeriesNum = "";
                SelectedFailureReason = null;

                Debug.WriteLine($"--> Stored Data Successfully");
            }
            catch (MySqlException ex)
            {
                logger.Error($"{ex.GetType().FullName} |\n {ex.Message} | \n StackTrace: {ex.StackTrace}");
                status = "Fehler";
                statusMessage = "Ein interner Fehler in einem SQL Befehl ist aufgetreten. Bitte prüfen Sie die Konfigurationsdatei und starten Sie die Anwendung erneut.";
            }
            catch (UserInputException ex)
            {
                status = "Fehler in den Eingaben";
                statusMessage = ex.Message;
            }
            catch (Exception ex)
            {
                logger.Error($"{ex.GetType().FullName} |\n {ex.Message} | \n StackTrace: {ex.StackTrace}");
                status = "Fehler";
                statusMessage = "Ein interner Fehler ist aufgetreten. Bitte prüfen Sie die Konfigurationsdatei und starten Sie die Anwendung erneut";
            }
            finally
            {
                await messageDisplayService.ShowMessage(statusMessage, status, MessageBoxButton.OK, MessageBoxImage.Information);
                IsBusy = false;
            }
        }

        /// <summary>
        ///     Checks the input values for validity before storing data in the database.
        /// </summary>
        /// <returns>A <see cref="string"/> indicating the validation result. Returns "OK" if the input is valid; otherwise, an error message.</returns>
        private string CheckInput()
        {
            string returnValue = "OK";
            
            if (SelectedLine == null)
            {
                returnValue = "Bitte wählen Sie eine Linie aus bevor Sie Daten eingeben und speichern.";
                return returnValue;
            }
            else if (string.IsNullOrWhiteSpace(SeriesNum))
            {
                returnValue = $"Die eingegebene Seriennummer: '{SeriesNum}' ist nicht gültig. Bitte überprüfen Sie ihre Eingabe.";
                return returnValue;
            }
            else if (SelectedFailureReason == null)
            {
                returnValue = $"Bitte wählen Sie einen Status für den EoL-Test aus.";
            }
            return returnValue;
        }
    }
}