namespace Rework_WPF_Testing.UnitTests
{
    public class StoreDataViewModelTests
    {
        List<LineConfig> lines = new List<LineConfig>
        {
            new LineConfig
            {
                Name = "Linie 111",
                ConfigEOL = new ConfigEOL
                {
                    Host = "127.0.0.1",
                    Port = 3307,
                    Username = "root",
                    Password = "Gruner1357#",
                    Database = "linie_852"
                },
                FromTable = new Tables
                {
                    Table = "prad",
                    Columns = new List<string> { "Seriennummer", "Teilenummer" }
                },
                ToTable = new Tables
                {
                    Table = "prad_rework",
                    Columns = new List<string> { "Seriennummer", "Teilenummer" }
                },
                UnwantedColumns = new List<string> { "ID", "Datum" },
                ReworkStatus = new List<ReworkStatus>
                {
                    new ReworkStatus { Key = 1, Value = "Erfolgreich" },
                    new ReworkStatus { Key = 2, Value = "Hamburger und noch was zum sehen wie lange das ist" },
                    new ReworkStatus { Key = 3, Value = "Pommes" },
                },
                TablesToDelete = new List<Tables>
                {
                    new Tables { Table = "abblasen", Columns = new List<string> { "Seriennummer" } },
                }
            }
        };

        [Fact]
        public async Task GetSettings_Success()
        {
            // Arrange
            var loggerMock = new Mock<Logger>();
            Mock<IAppSettingsService> appSettingsServiceMock = new Mock<IAppSettingsService>();
            Mock<IDatabaseService> databaseServiceMock = new Mock<IDatabaseService>();
            Mock<IMessageDisplayService> messageDisplayService = new Mock<IMessageDisplayService>();

            appSettingsServiceMock.Setup(s => s.GetAppSettingsAsync()).ReturnsAsync(lines);
            //appSettingsServiceMock.Setup(s => s.GetAppSettingsAsync()).ThrowsAsync(new Exception("Test"));

            var viewModel = new StoreDataViewModel(appSettingsServiceMock.Object, databaseServiceMock.Object, loggerMock.Object, messageDisplayService.Object);

            // Act
            await viewModel.GetSettingsCommand.ExecuteAsync(null);

            // Assert
            appSettingsServiceMock.Verify(s => s.GetAppSettingsAsync(), Times.AtLeastOnce);

            Assert.Equal("EoL - Nachbearbeitung", viewModel.Title);
            Assert.True(viewModel.IsNotBusy);
            Assert.True(viewModel.Lines.Count > 0);
            Assert.Equal(lines[0], viewModel.Lines[0]);
            Assert.Equal(lines[0].Name, viewModel.Lines[0].Name);
            Assert.Equal(lines[0].ConfigEOL.Database, viewModel.Lines[0].ConfigEOL.Database);
        }

        [Fact]
        public async Task StoreData_Success()
        {
            // Arrange
            var seriesNum = "1";
            var partNum = "1";
            var failureReason = new ReworkStatus { Key = 1, Value = "Erfolgreich" };

            var appSettingsServiceMock = new Mock<IAppSettingsService>();
            var databaseServiceMock = new Mock<IDatabaseService>();
            var loggerMock = new Mock<Logger>();
            Mock<IMessageDisplayService> messageDisplayService = new Mock<IMessageDisplayService>();

            appSettingsServiceMock.Setup(s => s.GetAppSettingsAsync()).ReturnsAsync(lines);
            databaseServiceMock.Setup(d => d
                .StoreDataInDb(lines[0].ConfigEOL, lines[0].TablesToDelete, lines[0].UnwantedColumns, failureReason.Key, "1", lines[0].FromTable, lines[0].ToTable, null))
                .ReturnsAsync("OK");
            messageDisplayService.Setup(m => m.ShowMessage("Hallo message", "caption", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information)).ReturnsAsync(System.Windows.MessageBoxResult.OK);

            var viewModel = new StoreDataViewModel(appSettingsServiceMock.Object, databaseServiceMock.Object, loggerMock.Object, messageDisplayService.Object);

            viewModel.SelectedLine = lines[0];
            viewModel.SeriesNum = seriesNum;
            viewModel.SelectedFailureReason = failureReason;

            // Act
            await viewModel.GetSettingsCommand.ExecuteAsync(null);
            await viewModel.StoreDataCommand.ExecuteAsync(null);

            // Assert
            appSettingsServiceMock.Verify(s => s.GetAppSettingsAsync(), Times.AtLeastOnce);
            databaseServiceMock.Verify(d => d.StoreDataInDb(lines[0].ConfigEOL, lines[0].TablesToDelete, lines[0].UnwantedColumns, failureReason.Key, "1", lines[0].FromTable, lines[0].ToTable, null), Times.Once);

            Assert.True(viewModel.IsNotBusy);
            Assert.Equal("", viewModel.SeriesNum);
            Assert.Null(viewModel.SelectedFailureReason);
        }
    }
}