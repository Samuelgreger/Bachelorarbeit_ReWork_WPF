using Newtonsoft.Json;
using Rework_WPF_Testing.Services;

namespace Rework_WPF_Testing.UnitTests
{
    public class AppSettingsServiceTests
    {
        TestAppSettingsService appSettingsService = new();

        [Fact]
        public async void GetAppSettingsAsync_Success()
        {
            // Act
            var result = await appSettingsService.GetAppSettingsAsync();

            // Assert
            Assert.IsType<List<LineConfig>>(result);

            foreach (var setting in result)
            {
                Assert.IsType<LineConfig>(setting);
            }

            // Assert properties of the first LineConfig object
            Assert.Equal("Linie 852", result[0].Name);
            Assert.Equal("127.0.0.1", result[0].ConfigEOL.Host);
            Assert.Equal(3307, result[0].ConfigEOL.Port);
            Assert.Equal("root", result[0].ConfigEOL.Username);
            Assert.Equal("Gruner1357#", result[0].ConfigEOL.Password);
            Assert.Equal("linie_852", result[0].ConfigEOL.Database);
            Assert.Equal("prad", result[0].FromTable.Table);
            //Assert.Equal(2, result[0].FromTable.Columns.Count);
            Assert.Equal("Seriennummer", result[0].FromTable.Columns[0]);
        }

        [Fact]
        public async void GetAppSettingsAsync_FileNotFound()
        {
            // Act & Assert
            await Assert.ThrowsAsync<FileNotFoundException>(() => appSettingsService.GetAppSettingsAsync("settings_failed.json"));
        }

        [Fact]
        public async Task GetAppSettingsAsync_JsonReadingFailed()
        {
            // Arrange
            string mockJson = "[Invalid Json]";

            // Act & Assert
            await Assert.ThrowsAsync<JsonReaderException>(() => appSettingsService.GetAppSettingsAsync("settings.json", mockJson));
        }

        [Theory]
        // Name empty
        [InlineData(@"[
              {
                ""Name"": """",
                ""ConfigEOL"": {
                  ""Host"": ""127.0.0.1"",
                  ""Port"": 3307,
                  ""Username"": ""root"",
                  ""Password"": ""Gruner1357#"",
                  ""Database"": ""linie_852""
                },
                ""FromTable"": {
                  ""Table"": ""prad"",
                  ""Columns"": [ ""Seriennummer"", ""Teilenummer"" ]
                },
                ""ToTable"": {
                  ""Table"": ""prad_rework"",
                  ""Columns"": [ ""Seriennummer"", ""Teilenummer"" ]
                },
                ""UnwantedColumns"": [ ""ID"", ""Datum"" ],
                ""TablesToDelete"": [
                  {
                    ""Table"": ""abblasen"",
                    ""Columns"": [ ""Seriennummer"" ]
                  },
                  {
                    ""Table"": ""stif_kbka_eindv"",
                    ""Columns"": [ ""Seriennummer_Grundplatte"" ]
                  },
                  {
                    ""Table"": ""kontakte_reinigen"",
                    ""Columns"": [ ""Seriennummer"" ]
                  },
                  {
                    ""Table"": ""mgnt_eindv"",
                    ""Columns"": [ ""Seriennummer_Grundplatte"" ]
                  }
                ],
                ""ReworkStatus"": [
                  {
                    ""Key"": 1,
                    ""Value"": ""Erfolgreich""
                  },
                  {
                    ""Key"": 2,
                    ""Value"": ""Hamburger und noch was zum sehen wie lange das ist""
                  },
                ]
              }
            ]")]
        // No Name
        [InlineData(@"[
              {
                ""ConfigEOL"": {
                  ""Host"": ""127.0.0.1"",
                  ""Port"": 3307,
                  ""Username"": ""root"",
                  ""Password"": ""Gruner1357#"",
                  ""Database"": ""linie_852""
                },
                ""FromTable"": {
                  ""Table"": ""prad"",
                  ""Columns"": [ ""Seriennummer"", ""Teilenummer"" ]
                },
                ""ToTable"": {
                  ""Table"": ""prad_rework"",
                  ""Columns"": [ ""Seriennummer"", ""Teilenummer"" ]
                },
                ""UnwantedColumns"": [ ""ID"", ""Datum"" ],
                ""TablesToDelete"": [
                  {
                    ""Table"": ""abblasen"",
                    ""Columns"": [ ""Seriennummer"" ]
                  },
                  {
                    ""Table"": ""stif_kbka_eindv"",
                    ""Columns"": [ ""Seriennummer_Grundplatte"" ]
                  },
                  {
                    ""Table"": ""kontakte_reinigen"",
                    ""Columns"": [ ""Seriennummer"" ]
                  },
                  {
                    ""Table"": ""mgnt_eindv"",
                    ""Columns"": [ ""Seriennummer_Grundplatte"" ]
                  }
                ],
                ""ReworkStatus"": [
                  {
                    ""Key"": 1,
                    ""Value"": ""Erfolgreich""
                  },
                  {
                    ""Key"": 2,
                    ""Value"": ""Hamburger und noch was zum sehen wie lange das ist""
                  },
                ]
              }
            ]")]
        // Empty Host
        [InlineData(@"[
              {
                ""Name"": ""Linie 852"",
                ""ConfigEOL"": {
                  ""Host"": """",
                  ""Port"": 3307,
                  ""Username"": ""root"",
                  ""Password"": ""Gruner1357#"",
                  ""Database"": ""linie_852""
                },
                ""FromTable"": {
                  ""Table"": ""prad"",
                  ""Columns"": [ ""Seriennummer"", ""Teilenummer"" ]
                },
                ""ToTable"": {
                  ""Table"": ""prad_rework"",
                  ""Columns"": [ ""Seriennummer"", ""Teilenummer"" ]
                },
                ""UnwantedColumns"": [ ""ID"", ""Datum"" ],
                ""TablesToDelete"": [
                  {
                    ""Table"": ""abblasen"",
                    ""Columns"": [ ""Seriennummer"" ]
                  },
                  {
                    ""Table"": ""stif_kbka_eindv"",
                    ""Columns"": [ ""Seriennummer_Grundplatte"" ]
                  },
                  {
                    ""Table"": ""kontakte_reinigen"",
                    ""Columns"": [ ""Seriennummer"" ]
                  },
                  {
                    ""Table"": ""mgnt_eindv"",
                    ""Columns"": [ ""Seriennummer_Grundplatte"" ]
                  }
                ],
                ""ReworkStatus"": [
                  {
                    ""Key"": 1,
                    ""Value"": ""Erfolgreich""
                  },
                  {
                    ""Key"": 2,
                    ""Value"": ""Hamburger und noch was zum sehen wie lange das ist""
                  },
                ]
              }
            ]")]
        // No ConfigEOL
        [InlineData(@"[
              {
                ""Name"": ""Linie 852"",
                ""FromTable"": {
                  ""Table"": ""prad"",
                  ""Columns"": [ ""Seriennummer"", ""Teilenummer"" ]
                },
                ""ToTable"": {
                  ""Table"": ""prad_rework"",
                  ""Columns"": [ ""Seriennummer"", ""Teilenummer"" ]
                },
                ""UnwantedColumns"": [ ""ID"", ""Datum"" ],
                ""TablesToDelete"": [
                  {
                    ""Table"": ""abblasen"",
                    ""Columns"": [ ""Seriennummer"" ]
                  },
                  {
                    ""Table"": ""stif_kbka_eindv"",
                    ""Columns"": [ ""Seriennummer_Grundplatte"" ]
                  },
                  {
                    ""Table"": ""kontakte_reinigen"",
                    ""Columns"": [ ""Seriennummer"" ]
                  },
                  {
                    ""Table"": ""mgnt_eindv"",
                    ""Columns"": [ ""Seriennummer_Grundplatte"" ]
                  }
                ],
                ""ReworkStatus"": [
                  {
                    ""Key"": 1,
                    ""Value"": ""Erfolgreich""
                  },
                  {
                    ""Key"": 2,
                    ""Value"": ""Hamburger und noch was zum sehen wie lange das ist""
                  },
                ]
              }
            ]")]
        // FromTable Empty Table 
        [InlineData(@"[
              {
                ""Name"": ""Linie 852"",
                ""ConfigEOL"": {
                  ""Host"": ""127.0.0.1"",
                  ""Port"": 3307,
                  ""Username"": ""root"",
                  ""Password"": ""Gruner1357#"",
                  ""Database"": ""linie_852""
                },
                ""FromTable"": {
                  ""Table"": """",
                  ""Columns"": [ ""Seriennummer"", ""Teilenummer"" ]
                },
                ""ToTable"": {
                  ""Table"": ""prad_rework"",
                  ""Columns"": [ ""Seriennummer"", ""Teilenummer"" ]
                },
                ""UnwantedColumns"": [ ""ID"", ""Datum"" ],
                ""TablesToDelete"": [
                  {
                    ""Table"": ""abblasen"",
                    ""Columns"": [ ""Seriennummer"" ]
                  },
                  {
                    ""Table"": ""stif_kbka_eindv"",
                    ""Columns"": [ ""Seriennummer_Grundplatte"" ]
                  },
                  {
                    ""Table"": ""kontakte_reinigen"",
                    ""Columns"": [ ""Seriennummer"" ]
                  },
                  {
                    ""Table"": ""mgnt_eindv"",
                    ""Columns"": [ ""Seriennummer_Grundplatte"" ]
                  }
                ],
                ""ReworkStatus"": [
                  {
                    ""Key"": 1,
                    ""Value"": ""Erfolgreich""
                  },
                  {
                    ""Key"": 2,
                    ""Value"": ""Hamburger und noch was zum sehen wie lange das ist""
                  },
                ]
              }
            ]")]
        // FromTable Empty
        [InlineData(@"[
              {
                ""Name"": ""Linie 852"",
                ""ConfigEOL"": {
                  ""Host"": ""127.0.0.1"",
                  ""Port"": 3307,
                  ""Username"": ""root"",
                  ""Password"": ""Gruner1357#"",
                  ""Database"": ""linie_852""
                },
                ""FromTable"": {
                },
                ""ToTable"": {
                  ""Table"": ""prad_rework"",
                  ""Columns"": [ ""Seriennummer"", ""Teilenummer"" ]
                },
                ""UnwantedColumns"": [ ""ID"", ""Datum"" ],
                ""TablesToDelete"": [
                  {
                    ""Table"": ""abblasen"",
                    ""Columns"": [ ""Seriennummer"" ]
                  },
                  {
                    ""Table"": ""stif_kbka_eindv"",
                    ""Columns"": [ ""Seriennummer_Grundplatte"" ]
                  },
                  {
                    ""Table"": ""kontakte_reinigen"",
                    ""Columns"": [ ""Seriennummer"" ]
                  },
                  {
                    ""Table"": ""mgnt_eindv"",
                    ""Columns"": [ ""Seriennummer_Grundplatte"" ]
                  }
                ],
                ""ReworkStatus"": [
                  {
                    ""Key"": 1,
                    ""Value"": ""Erfolgreich""
                  },
                  {
                    ""Key"": 2,
                    ""Value"": ""Hamburger und noch was zum sehen wie lange das ist""
                  },
                ]
              }
            ]")]
        // FromTable Null
        [InlineData(@"[
              {
                ""Name"": ""Linie 852"",
                ""ConfigEOL"": {
                  ""Host"": """",
                  ""Port"": 3307,
                  ""Username"": ""root"",
                  ""Password"": ""Gruner1357#"",
                  ""Database"": ""linie_852""
                },
                ""ToTable"": {
                  ""Table"": ""prad_rework"",
                  ""Columns"": [ ""Seriennummer"", ""Teilenummer"" ]
                },
                ""UnwantedColumns"": [ ""ID"", ""Datum"" ],
                ""TablesToDelete"": [
                  {
                    ""Table"": ""abblasen"",
                    ""Columns"": [ ""Seriennummer"" ]
                  },
                  {
                    ""Table"": ""stif_kbka_eindv"",
                    ""Columns"": [ ""Seriennummer_Grundplatte"" ]
                  },
                  {
                    ""Table"": ""kontakte_reinigen"",
                    ""Columns"": [ ""Seriennummer"" ]
                  },
                  {
                    ""Table"": ""mgnt_eindv"",
                    ""Columns"": [ ""Seriennummer_Grundplatte"" ]
                  }
                ],
                ""ReworkStatus"": [
                  {
                    ""Key"": 1,
                    ""Value"": ""Erfolgreich""
                  },
                  {
                    ""Key"": 2,
                    ""Value"": ""Hamburger und noch was zum sehen wie lange das ist""
                  },
                ]
              }
            ]")]
        // FromTable no columns
        [InlineData(@"[
              {
                ""Name"": ""Linie 852"",
                ""ConfigEOL"": {
                  ""Host"": ""127.0.0.1"",
                  ""Port"": 3307,
                  ""Username"": ""root"",
                  ""Password"": ""Gruner1357#"",
                  ""Database"": ""linie_852""
                },
                ""FromTable"": {
                  ""Table"": ""prad"",
                  ""Columns"": [ ]
                },
                ""ToTable"": {
                  ""Table"": ""prad_rework"",
                  ""Columns"": [ ""Seriennummer"", ""Teilenummer"" ]
                },
                ""UnwantedColumns"": [ ""ID"", ""Datum"" ],
                ""TablesToDelete"": [
                  {
                    ""Table"": ""abblasen"",
                    ""Columns"": [ ""Seriennummer"" ]
                  },
                  {
                    ""Table"": ""stif_kbka_eindv"",
                    ""Columns"": [ ""Seriennummer_Grundplatte"" ]
                  },
                  {
                    ""Table"": ""kontakte_reinigen"",
                    ""Columns"": [ ""Seriennummer"" ]
                  },
                  {
                    ""Table"": ""mgnt_eindv"",
                    ""Columns"": [ ""Seriennummer_Grundplatte"" ]
                  }
                ],
                ""ReworkStatus"": [
                  {
                    ""Key"": 1,
                    ""Value"": ""Erfolgreich""
                  },
                  {
                    ""Key"": 2,
                    ""Value"": ""Hamburger und noch was zum sehen wie lange das ist""
                  },
                ]
              }
            ]")]
        // ToTable Empty Table
        [InlineData(@"[
              {
                ""Name"": ""Linie 852"",
                ""ConfigEOL"": {
                  ""Host"": ""127.0.0.1"",
                  ""Port"": 3307,
                  ""Username"": ""root"",
                  ""Password"": ""Gruner1357#"",
                  ""Database"": ""linie_852""
                },
                ""FromTable"": {
                  ""Table"": ""prad"",
                  ""Columns"": [ ""Seriennummer"", ""Teilenummer"" ]
                },
                ""ToTable"": {
                  ""Table"": ""prad_rework"",
                  ""Columns"": [ ""Seriennummer"", ""Teilenummer"" ]
                },
                ""UnwantedColumns"": [ ""ID"", ""Datum"" ],
                ""TablesToDelete"": [
                  {
                    ""Table"": """",
                    ""Columns"": [ ""Seriennummer"" ]
                  },
                  {
                    ""Table"": ""stif_kbka_eindv"",
                    ""Columns"": [ ""Seriennummer_Grundplatte"" ]
                  },
                  {
                    ""Table"": ""kontakte_reinigen"",
                    ""Columns"": [ ""Seriennummer"" ]
                  },
                  {
                    ""Table"": ""mgnt_eindv"",
                    ""Columns"": [ ""Seriennummer_Grundplatte"" ]
                  }
                ],
                ""ReworkStatus"": [
                  {
                    ""Key"": 1,
                    ""Value"": ""Erfolgreich""
                  },
                  {
                    ""Key"": 2,
                    ""Value"": ""Hamburger und noch was zum sehen wie lange das ist""
                  },
                ]
              }
            ]")]
        // TablesToDelete Null
        [InlineData(@"[
              {
                ""Name"": ""Linie 852"",
                ""ConfigEOL"": {
                  ""Host"": ""127.0.0.1"",
                  ""Port"": 3307,
                  ""Username"": ""root"",
                  ""Password"": ""Gruner1357#"",
                  ""Database"": ""linie_852""
                },
                ""FromTable"": {
                  ""Table"": ""prad"",
                  ""Columns"": [ ""Seriennummer"", ""Teilenummer"" ]
                },
                ""ToTable"": {
                  ""Table"": ""prad_rework"",
                  ""Columns"": [ ""Seriennummer"", ""Teilenummer"" ]
                },
                ""UnwantedColumns"": [ ""ID"", ""Datum"" ],
                ""ReworkStatus"": [
                  {
                    ""Key"": 1,
                    ""Value"": ""Erfolgreich""
                  },
                  {
                    ""Key"": 2,
                    ""Value"": ""Hamburger und noch was zum sehen wie lange das ist""
                  },
                ]
              }
            ]")]
        // UnwantedColumns Null
        [InlineData(@"[
              {
                ""Name"": ""Linie 852"",
                ""ConfigEOL"": {
                  ""Host"": ""127.0.0.1"",
                  ""Port"": 3307,
                  ""Username"": ""root"",
                  ""Password"": ""Gruner1357#"",
                  ""Database"": ""linie_852""
                },
                ""FromTable"": {
                  ""Table"": ""prad"",
                  ""Columns"": [ ""Seriennummer"", ""Teilenummer"" ]
                },
                ""ToTable"": {
                  ""Table"": ""prad_rework"",
                  ""Columns"": [ ""Seriennummer"", ""Teilenummer"" ]
                },
                ""TablesToDelete"": [
                  {
                    ""Table"": ""abblasen"",
                    ""Columns"": [ ""Seriennummer"" ]
                  },
                  {
                    ""Table"": ""stif_kbka_eindv"",
                    ""Columns"": [ ""Seriennummer_Grundplatte"" ]
                  },
                  {
                    ""Table"": ""kontakte_reinigen"",
                    ""Columns"": [ ""Seriennummer"" ]
                  },
                  {
                    ""Table"": ""mgnt_eindv"",
                    ""Columns"": [ ""Seriennummer_Grundplatte"" ]
                  }
                ],
                ""ReworkStatus"": [
                  {
                    ""Key"": 1,
                    ""Value"": ""Erfolgreich""
                  },
                  {
                    ""Key"": 2,
                    ""Value"": ""Hamburger und noch was zum sehen wie lange das ist""
                  },
                ]
              }
            ]")]      
        // ReworkStatus Empty Value
        [InlineData(@"[
              {
                ""Name"": ""Linie 852"",
                ""ConfigEOL"": {
                  ""Host"": ""127.0.0.1"",
                  ""Port"": 3307,
                  ""Username"": ""root"",
                  ""Password"": ""Gruner1357#"",
                  ""Database"": ""linie_852""
                },
                ""FromTable"": {
                  ""Table"": ""prad"",
                  ""Columns"": [ ""Seriennummer"", ""Teilenummer"" ]
                },
                ""ToTable"": {
                  ""Table"": ""prad_rework"",
                  ""Columns"": [ ""Seriennummer"", ""Teilenummer"" ]
                },
                ""UnwantedColumns"": [ ""ID"", ""Datum"" ],
                ""TablesToDelete"": [
                  {
                    ""Table"": ""abblasen"",
                    ""Columns"": [ ""Seriennummer"" ]
                  },
                  {
                    ""Table"": ""stif_kbka_eindv"",
                    ""Columns"": [ ""Seriennummer_Grundplatte"" ]
                  },
                  {
                    ""Table"": ""kontakte_reinigen"",
                    ""Columns"": [ ""Seriennummer"" ]
                  },
                  {
                    ""Table"": ""mgnt_eindv"",
                    ""Columns"": [ ""Seriennummer_Grundplatte"" ]
                  }
                ],
                ""ReworkStatus"": [
                  {
                    ""Key"": 1,
                    ""Value"": """"
                  },
                  {
                    ""Key"": 2,
                    ""Value"": ""Hamburger und noch was zum sehen wie lange das ist""
                  },
                ]
              }
            ]")]
        // ReworkStatus Null
        [InlineData(@"[
              {
                ""Name"": ""Linie 852"",
                ""ConfigEOL"": {
                  ""Host"": ""127.0.0.1"",
                  ""Port"": 3307,
                  ""Username"": ""root"",
                  ""Password"": ""Gruner1357#"",
                  ""Database"": ""linie_852""
                },
                ""FromTable"": {
                  ""Table"": ""prad"",
                  ""Columns"": [ ""Seriennummer"", ""Teilenummer"" ]
                },
                ""ToTable"": {
                  ""Table"": ""prad_rework"",
                  ""Columns"": [ ""Seriennummer"", ""Teilenummer"" ]
                },
                ""UnwantedColumns"": [ ""ID"", ""Datum"" ],
                ""TablesToDelete"": [
                  {
                    ""Table"": ""abblasen"",
                    ""Columns"": [ ""Seriennummer"" ]
                  },
                  {
                    ""Table"": ""stif_kbka_eindv"",
                    ""Columns"": [ ""Seriennummer_Grundplatte"" ]
                  },
                  {
                    ""Table"": ""kontakte_reinigen"",
                    ""Columns"": [ ""Seriennummer"" ]
                  },
                  {
                    ""Table"": ""mgnt_eindv"",
                    ""Columns"": [ ""Seriennummer_Grundplatte"" ]
                  }
                ],
              }
            ]")]
        // ReworkStatus Empty Value 
        [InlineData(@"[
              {
                ""Name"": ""Linie 852"",
                ""ConfigEOL"": {
                  ""Host"": ""127.0.0.1"",
                  ""Port"": 3307,
                  ""Username"": ""root"",
                  ""Password"": ""Gruner1357#"",
                  ""Database"": ""linie_852""
                },
                ""FromTable"": {
                  ""Table"": ""prad"",
                  ""Columns"": [ ""Seriennummer"", ""Teilenummer"" ]
                },
                ""ToTable"": {
                  ""Table"": ""prad_rework"",
                  ""Columns"": [ ""Seriennummer"", ""Teilenummer"" ]
                },
                ""UnwantedColumns"": [ ""ID"", ""Datum"" ],
                ""TablesToDelete"": [
                  {
                    ""Table"": ""abblasen"",
                    ""Columns"": [ ""Seriennummer"" ]
                  },
                  {
                    ""Table"": ""stif_kbka_eindv"",
                    ""Columns"": [ ""Seriennummer_Grundplatte"" ]
                  },
                  {
                    ""Table"": ""kontakte_reinigen"",
                    ""Columns"": [ ""Seriennummer"" ]
                  },
                  {
                    ""Table"": ""mgnt_eindv"",
                    ""Columns"": [ ""Seriennummer_Grundplatte"" ]
                  }
                ],
                ""ReworkStatus"": [
                  {
                    ""Key"": 1,
                  },
                  {
                    ""Key"": 2,
                    ""Value"": ""Hamburger und noch was zum sehen wie lange das ist""
                  }
                ]
              }
            ]")]
        public async Task GetAppSettingsAsync_InvalidJson(string mockJson)
        {
            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => appSettingsService.GetAppSettingsAsync("settings.json", mockJson));
        }
    }
}