using MySql.Data.MySqlClient;
using ReWork_WPF.Exceptions;
using ReWork_WPF.Models.DataTypes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Threading.Tasks;

namespace ReWork_WPF.Models.Services
{
    public class DatabaseService : IDatabaseService
    {
        public async Task<string> StoreDataInDb(ConfigEOL connectionSettings, List<Tables> tablesToDelete, List<string> unwantedColumns, int selectedFailureReason, string seriesNum, Tables fromTable, Tables toTable, string? partNum = null)
        {
            string host = connectionSettings.Host;
            int port = connectionSettings.Port;
            string username = connectionSettings.Username;
            string password = connectionSettings.Password;
            string database = connectionSettings.Database;

            MySqlConnection conn = await GetDbConnection(host, port, username, database, password);

            try
            {
                using (conn)
                {
                    conn.Open();
                    using (MySqlTransaction transaction = conn.BeginTransaction())
                    {
                        // Retrieve the column names and data Types from the source table
                        var result = GetColumnTypesAndNames(conn, connectionSettings, fromTable, unwantedColumns);
                        List<string> columnTypes = result.columnTypes;
                        List<string> columnNames = result.columnNames;

                        // Get data from the 'fromTable'
                        DataTable dataRows = ReadFromTable(conn, fromTable, transaction, seriesNum, partNum);

                        // Safe the date in the 'toTable'
                        StoreToTable(conn, toTable, transaction, columnNames, columnTypes, dataRows, selectedFailureReason, unwantedColumns);

                        // Delete from all tables to delete ('tablesToDelete') and the 'fromTable'
                        DeleteTablesToDelete(conn, fromTable, tablesToDelete, transaction, seriesNum, partNum);

                        transaction.Commit();
                        return "OK";
                    }
                }
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        ///     Creates and returns a database connection based on the provided connection parameters.
        /// </summary>
        /// <param name="host">The database server host.</param>
        /// <param name="port">The database server port.</param>
        /// <param name="username">The username for the database connection.</param>
        /// <param name="database">The name of the database.</param>
        /// <param name="password">The password for the database connection.</param>
        /// <returns>
        ///     A task that represents the asynchronous operation. The task result is a <see cref="MySqlConnection"/> 
        ///     object representing the database connection.
        /// </returns>
        private Task<MySqlConnection> GetDbConnection(string host, int port, string username, string database, string password)
        {
            string connString = "Server=" + host + ";Database=" + database +
                ";port=" + port + ";User Id=" + username + ";password=" + password;

            MySqlConnection conn = new MySqlConnection(connString);
            return Task.FromResult(conn);
        }

        /// <summary>
        ///     Retrieves the column types and names from the specified table in the database connection.
        /// </summary>
        /// <param name="conn">The database connection.</param>
        /// <param name="connectionSettings">The configuration settings for the database connection.</param>
        /// <param name="fromTable">The source table.</param>
        /// <param name="unwantedColumns">The list of unwanted columns to exclude from the result.</param>
        /// <returns>
        ///     A tuple containing the list of column types and the list of column names.
        /// </returns>
        private (List<string> columnTypes, List<string> columnNames) GetColumnTypesAndNames(MySqlConnection conn, ConfigEOL connectionSettings, Tables fromTable, List<string> unwantedColumns)
        {
            DataTable schemaTable = conn.GetSchema("Columns", new[] { null, null, fromTable.Table });
            if (schemaTable.Rows.Count == 0)
            {
                throw new Exception($"Von dem eingestellten 'fromTable' ({fromTable.Table}) aus " +
                    $"der Datenbank ({connectionSettings.Database}) konnte kein Schema geladen werden. Prüfen Sie die Konfigurationsdatei.");
            }

            List<string> columnTypes = new List<string>();
            List<string> columnNames = new List<string>();
            foreach (DataRow row in schemaTable.Rows)
            {
                string columnName = row.Field<string>("COLUMN_NAME");
                string columnType = row.Field<string>("DATA_TYPE");

                if (unwantedColumns.Contains(columnName))
                    continue;

                columnNames.Add(columnName);
                columnTypes.Add(columnType);
            }
            return (columnTypes, columnNames);
        }

        /// <summary>
        ///     Reads data from the specified table (fromTable) in the database based on the input series number and part number.
        /// </summary>
        /// <param name="conn">The database connection.</param>
        /// <param name="fromTable">The source table.</param>
        /// <param name="transaction">The database transaction.</param>
        /// <param name="seriesNum">The series number.</param>
        /// <param name="partNum">The part number (optional).</param>
        /// <returns>
        ///     A <see cref="DataTable"/> containing the retrieved data rows.
        /// </returns>
        /// <exception cref="UserInputException">Thrown when the user input is not found in the database</exception>
        private DataTable ReadFromTable(MySqlConnection conn, Tables fromTable, MySqlTransaction transaction, string seriesNum, string? partNum = null)
        {
            DataTable dataRows = new DataTable();
            string selectQuery = $"SELECT * FROM {fromTable.Table} WHERE {fromTable.Columns[0]} = @seriesNum";
            if (fromTable.Columns.Count > 1 && !string.IsNullOrWhiteSpace(fromTable.Columns[1]) && partNum != null)
            {
                Debug.WriteLine($"PartNum is '{partNum}'");
                selectQuery += $" AND {fromTable.Columns[1]} = @partNum";
            }
            using (MySqlCommand selectCommand = new MySqlCommand(selectQuery, conn, transaction))
            {
                selectCommand.Parameters.AddWithValue("@seriesNum", seriesNum);
                if (partNum != null)
                {
                    selectCommand.Parameters.AddWithValue("@partNum", partNum);
                }

                using (MySqlDataAdapter adapter = new MySqlDataAdapter(selectCommand))
                {
                    adapter.Fill(dataRows);
                }

                if (dataRows.Rows.Count == 0)
                {
                    throw new UserInputException("Die angegebenen Eingaben wurden nicht in der Datenbank gefunden. Bitte prüfen Sie diese und versuchen Sie es erneut.");
                }
            }
            return dataRows;
        }

        /// <summary>
        ///     Stores the provided data rows into the specified table in the database.
        /// </summary>
        /// <param name="conn">The database connection.</param>
        /// <param name="toTable">The destination table.</param>
        /// <param name="transaction">The database transaction.</param>
        /// <param name="columnNames">The list of column names.</param>
        /// <param name="columnTypes">The list of column types.</param>
        /// <param name="dataRows">The data rows to be stored.</param>
        /// <param name="selectedFailureReason">The selected status of the EoL-Test.</param>
        /// <param name="unwantedColumns">The list of unwanted columns to exclude from the insertion.</param>
        /// <exception cref="Exception">Thrown when no rows are inserted into the database.</exception>
        private void StoreToTable(MySqlConnection conn, Tables toTable, MySqlTransaction transaction, List<string> columnNames, List<string> columnTypes, DataTable dataRows, int selectedFailureReason, List<string> unwantedColumns)
        {
            columnNames.Add("EoL_Test_Status");

            // Remove unwanted columns from the DataTable
            foreach (var i in unwantedColumns)
            {
                if(dataRows.Columns.Contains(i))
                {
                    dataRows.Columns.Remove(i);
                }
            }

            string insertQuery = $"INSERT INTO {toTable.Table} ({string.Join(", ", columnNames)}) VALUES ";

            foreach (DataRow row in dataRows.Rows)
            {
                List<string> valuePlaceholders = new List<string>();
                List<MySqlParameter> parameters = new List<MySqlParameter>();

                for (int i = 0; i < dataRows.Columns.Count; i++)
                {
                    DataColumn col = dataRows.Columns[i];
                    string columnName = columnNames[i];
                    string columnType = columnTypes[i];

                    // Prepare the placeholder for the column value
                    string valuePlaceholder = "@" + columnName;
                    valuePlaceholders.Add(valuePlaceholder);

                    object columnValue = row[col];
                    MySqlDbType dbType = GetMySqlDbType(columnType);

                    MySqlParameter parameter = new MySqlParameter(valuePlaceholder, dbType);
                    parameter.Value = columnValue;
                    parameters.Add(parameter);
                }
                valuePlaceholders.Add("@EoL_Test_Status");
                MySqlParameter statusParameter = new MySqlParameter("@EoL_Test_Status", MySqlDbType.Int32);
                statusParameter.Value = selectedFailureReason;
                parameters.Add(statusParameter);

                string insertValues = $"({string.Join(", ", valuePlaceholders)})";
                string fullInsertQuery = insertQuery + insertValues;

                // Execute the insert command
                using (MySqlCommand insertCommand = new MySqlCommand(fullInsertQuery, conn, transaction))
                {
                    insertCommand.Parameters.AddRange(parameters.ToArray());
                    Debug.WriteLine("Execute the insert command");
                    int rowsAffected = insertCommand.ExecuteNonQuery();
                    Debug.WriteLine($"rows Affected {rowsAffected}");

                    if (rowsAffected < 1)
                    {
                        throw new Exception("Es wurden keine Daten in die Datenbank eingefügt. Überprüfen Sie Ihre Eingaben und versuchen Sie es erneut.");
                    }
                }
            }
        }

        /// <summary>
        ///     Deletes data from the specified tables in the database based on the provided criteria.
        /// </summary>
        /// <param name="conn">The database connection.</param>
        /// <param name="fromTable">The source table to delete data from.</param>
        /// <param name="tablesToDelete">The list of tables to delete data from.</param>
        /// <param name="transaction">The database transaction.</param>
        /// <param name="seriesNum">The series number, which is given to filter the data to delete.</param>
        /// <param name="partNum">The optional part number to further filter the data to delete.</param>
        /// <exception cref="Exception">Thrown when no data is deleted from the tables.</exception>
        private void DeleteTablesToDelete(MySqlConnection conn, Tables fromTable, List<Tables> tablesToDelete, MySqlTransaction transaction, string seriesNum, string? partNum = null)
        {
            int rowsAffected = 0;
            string deleteFromTableData = $"DELETE FROM {fromTable.Table} WHERE {fromTable.Columns[0]} = @seriesNum";
            if (fromTable.Columns.Count > 1 && !string.IsNullOrWhiteSpace(fromTable.Columns[1]) && partNum != null)
            {
                deleteFromTableData += $" AND {fromTable.Columns[1]} = @partNum";
            }
            using (MySqlCommand deleteFromTableDataCommand = new MySqlCommand(deleteFromTableData, conn, transaction))
            {
                deleteFromTableDataCommand.Parameters.AddWithValue("@seriesNum", seriesNum);
                if (partNum != null)
                {
                    deleteFromTableDataCommand.Parameters.AddWithValue("@partNum", partNum);
                }
                rowsAffected += deleteFromTableDataCommand.ExecuteNonQuery();
            }
            if (rowsAffected < 1)
            {
                throw new Exception($"Es wurden keine Daten in der Datenbank aus der '{fromTable.Table}' gelöscht. Überprüfen Sie Ihre Eingaben und versuchen Sie es erneut.");
            }

            rowsAffected = 0;
            foreach (var table in tablesToDelete)
            {
                string deleteQuery = $"DELETE FROM {table.Table} WHERE {table.Columns[0]} = @seriesNum";
                if (table.Columns.Count > 1 && !string.IsNullOrWhiteSpace(table.Columns[1]) && partNum != null)
                {
                    deleteQuery += $" AND {table.Columns[1]} = @partNum";
                }
                using (MySqlCommand deleteCommand = new MySqlCommand(deleteQuery, conn, transaction))
                {
                    if (partNum != null)
                    {
                        deleteCommand.Parameters.AddWithValue("@partNum", partNum);
                    }
                    deleteCommand.Parameters.AddWithValue("@seriesNum", seriesNum);
                    rowsAffected += deleteCommand.ExecuteNonQuery();
                }
            }
            
            if (rowsAffected < 1)
            {
                throw new Exception("Es wurden keine Daten in der Datenbank gelöscht. Überprüfen Sie Ihre Eingaben und versuchen Sie es erneut.");
            }
        }

        /// <summary>
        ///     Retrieves the corresponding MySqlDbType based on the provided column type.
        /// </summary>
        /// <param name="columnType">The column type.</param>
        /// <returns>The corresponding MySqlDbType.</returns>
        /// <remarks>If no matching MySqlDbType is found, a default type (MySqlDbType.VarChar) is returned.</remarks>
        private MySqlDbType GetMySqlDbType(string columnType)
        {
            Dictionary<string, MySqlDbType> columnTypeMapping = new Dictionary<string, MySqlDbType>()
            {
                { "bigint", MySqlDbType.Int64 },
                { "timestamp", MySqlDbType.Timestamp },
                { "int", MySqlDbType.Int32 },
                { "double", MySqlDbType.Double },
                { "float", MySqlDbType.Float },
                { "tinyint", MySqlDbType.Byte },

                { "binary", MySqlDbType.Binary },
                { "bit", MySqlDbType.Bit },
                { "char", MySqlDbType.String },
                { "date", MySqlDbType.Date },
                { "datetime", MySqlDbType.DateTime },
                { "decimal", MySqlDbType.Decimal },
                { "enum", MySqlDbType.Enum },
                { "mediumint", MySqlDbType.Int24 },
                { "smallint", MySqlDbType.Int16 },
                { "text", MySqlDbType.Text },
                { "time", MySqlDbType.Time },
                { "varbinary", MySqlDbType.VarBinary },
                { "varchar", MySqlDbType.VarChar },
                { "year", MySqlDbType.Year }
            };

            if (columnTypeMapping.ContainsKey(columnType.ToLower()))
            {
                return columnTypeMapping[columnType.ToLower()];
            }
            else
            {
                throw new Exception($"Der Spaltentyp ({columnType}) wird nicht unterstützt. Kontaktieren Sie bitte den Entwickler.");
            }
        }
    }
}
