using System.Collections.Generic;

namespace ReWork_WPF.Models.DataTypes
{
    public class LineConfig
    {
        public string Name { get; set; }
        public ConfigEOL ConfigEOL { get; set; }
        public List<Tables> TablesToDelete { get; set; }
        public Tables FromTable { get; set; }
        public Tables ToTable { get; set; }
        public List<ReworkStatus> ReworkStatus { get; set; }
        public List<string> UnwantedColumns { get; set; }
    }

    public class ConfigEOL
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Database { get; set; }
    }

    public class ReworkStatus
    {
        public int Key { get; set; }
        public string Value { get; set; }
    }

    public class Tables
    {
        public string Table { get; set; }
        public List<string> Columns { get; set; }
    }
}
