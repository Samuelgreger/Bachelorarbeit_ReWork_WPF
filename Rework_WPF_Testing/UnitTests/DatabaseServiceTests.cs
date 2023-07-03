using MySql.Data.MySqlClient;
using ReWork_WPF.Exceptions;

namespace Rework_WPF_Testing.UnitTests
{
    public class DatabaseServiceTests
    {
        IDatabaseService databaseService = new DatabaseService();
        Mock<IDatabaseService> mockDatabaseService = new Mock<IDatabaseService>();

        LineConfig mockSettings;
        public DatabaseServiceTests()
        {
            mockSettings = new LineConfig
            {
                Name = "Linie 852",
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
            };
        }

        [Fact]
        public async Task StoreDataInDb_WithoutPartNum_Success()
        {
            //Arrange
            mockDatabaseService.Setup(x => x.StoreDataInDb(mockSettings.ConfigEOL, mockSettings.TablesToDelete, mockSettings.UnwantedColumns, 1, "15", mockSettings.FromTable, mockSettings.ToTable, null)).ReturnsAsync("OK");
            
            // Act
            string res = await mockDatabaseService.Object.StoreDataInDb(mockSettings.ConfigEOL, mockSettings.TablesToDelete, mockSettings.UnwantedColumns, 1, "15", mockSettings.FromTable, mockSettings.ToTable);

            //Assert
            Assert.Equal("OK", res);
        }

        [Fact]
        public async Task StoreDataInDb_WithPartNum_Success()
        {
            //Arrange
            mockDatabaseService.Setup(x => x.StoreDataInDb(mockSettings.ConfigEOL, mockSettings.TablesToDelete, mockSettings.UnwantedColumns, 1, "15", mockSettings.FromTable, mockSettings.ToTable, "47095")).ReturnsAsync("OK");

            // Act
            string res = await mockDatabaseService.Object.StoreDataInDb(mockSettings.ConfigEOL, mockSettings.TablesToDelete, mockSettings.UnwantedColumns, 1, "15", mockSettings.FromTable, mockSettings.ToTable, "47095");

            //Assert
            Assert.Equal("OK", res);
        }

        [Fact]
        public async Task StoreDataInDb_InvalidDbConnectionSettings()
        {
            //Arrange
            ConfigEOL invalidData = new ConfigEOL
            {
                Host = "127.0.0.1",
                Port = 3307,
                Username = "root",
                Password = "Gruner1357#",
                Database = "linie_852_invalid"
            };
            mockSettings.ConfigEOL = invalidData;

            // Act and Assert
            Assert.Equal(mockSettings.ConfigEOL, invalidData);
            var exception = await Assert.ThrowsAsync<MySqlException>(() => databaseService.StoreDataInDb(mockSettings.ConfigEOL, mockSettings.TablesToDelete, mockSettings.UnwantedColumns, 1, "15", mockSettings.FromTable, mockSettings.ToTable)); 
        }

        // Die Eingaben wurden nicht in der Datenbank gefunden
        [Theory]
        [InlineData("12345677")]
        [InlineData("15", "12345678998745")]
        [InlineData("12345677", "47095")]
        [InlineData("12345677", "12345677")]
        [InlineData("", "12345677")]
        [InlineData("12345677", "")]
        [InlineData("-12345677", "")]
        public async Task StoreDataInDb_InvalidSeriesNumAndPartNum(string seriesNum, string? partNum = null)
        {
            Exception exception;
            // Act and Assert
            if (partNum == null)
            {
                exception = await Assert.ThrowsAsync<UserInputException>(() => databaseService.StoreDataInDb(mockSettings.ConfigEOL, mockSettings.TablesToDelete, mockSettings.UnwantedColumns, 1, seriesNum, mockSettings.FromTable, mockSettings.ToTable));
            }
            else
            {
                exception = await Assert.ThrowsAsync<UserInputException>(() => databaseService.StoreDataInDb(mockSettings.ConfigEOL, mockSettings.TablesToDelete, mockSettings.UnwantedColumns, 1, seriesNum, mockSettings.FromTable, mockSettings.ToTable, partNum));
            }
            Assert.Equal("Die angegebenen Eingaben wurden nicht in der Datenbank gefunden. Bitte prüfen Sie diese und versuchen Sie es erneut.", exception.Message);
        }

        [Fact]
        public async Task StoreDataInDb_InvalidFromTable()
        {
            //Arrange
            Tables invalidFromTable = new Tables
            {
                Table = "prad_invalid",
                Columns = new List<string> { "Seriennummer", "Teilenummer" }
            };

            mockSettings.FromTable = invalidFromTable;

            // Act and Assert
            Assert.Equal(mockSettings.FromTable, invalidFromTable);
            var exception = await Assert.ThrowsAsync<Exception>(() => databaseService.StoreDataInDb(mockSettings.ConfigEOL, mockSettings.TablesToDelete, mockSettings.UnwantedColumns, 1, "15", mockSettings.FromTable, mockSettings.ToTable));
            Assert.Equal($"Von dem eingestellten 'fromTable' ({invalidFromTable.Table}) aus der Datenbank ({mockSettings.ConfigEOL.Database}) konnte kein Schema geladen werden. Prüfen Sie die Konfigurationsdatei.", exception.Message);
        }

        [Fact]
        public async Task StoreDataInDb_InvalidColumnsFromTable()
        {
            //Arrange
            Tables invalidFromTable = new Tables
            {
                Table = "prad",
                Columns = new List<string> { "Seriennummer_invalid", "Teilenummer" }
            };

            mockSettings.FromTable = invalidFromTable;

            // Act and Assert
            Assert.Equal(mockSettings.FromTable, invalidFromTable);
            var exception = await Assert.ThrowsAsync<MySqlException>(() => databaseService.StoreDataInDb(mockSettings.ConfigEOL, mockSettings.TablesToDelete, mockSettings.UnwantedColumns, 1, "15", mockSettings.FromTable, mockSettings.ToTable));
        }

        [Fact]
        public async Task StoreDataInDb_InvalidToTable()
        {
            //Arrange
            Tables invalidToTable = new Tables
            {
                Table = "prad_rework_invalid",
                Columns = new List<string> { "Seriennummer", "Teilenummer" }
            };

            mockSettings.ToTable = invalidToTable;

            // Act and Assert
            Assert.Equal(mockSettings.ToTable, invalidToTable);
            var exception = await Assert.ThrowsAsync<MySqlException>(() => databaseService.StoreDataInDb(mockSettings.ConfigEOL, mockSettings.TablesToDelete, mockSettings.UnwantedColumns, 1, "15", mockSettings.FromTable, mockSettings.ToTable));
        }

        [Theory]
        [InlineData("abblasen_invalid", new string[] { "Seriennummer" })]
        [InlineData("abblasen", new string[] { "Seriennummer_invalid" })]

        public async Task StoreDataInDb_InvalidTablesToDelete(string table, string[] col)
        {
            //Arrange
            List<Tables> invalidTablesToDelete = new List<Tables>
            {
                new Tables { Table = table, Columns = new List<string>(col) },
            };

            mockSettings.TablesToDelete = invalidTablesToDelete;

            // Act and Assert
            Assert.Equal(mockSettings.TablesToDelete, invalidTablesToDelete);
            var exception = await Assert.ThrowsAsync<MySqlException>(() => databaseService.StoreDataInDb(mockSettings.ConfigEOL, mockSettings.TablesToDelete, mockSettings.UnwantedColumns, 1, "15", mockSettings.FromTable, mockSettings.ToTable));
        }
    }
}
