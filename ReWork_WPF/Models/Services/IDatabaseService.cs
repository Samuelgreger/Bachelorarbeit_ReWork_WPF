using ReWork_WPF.Models.DataTypes;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ReWork_WPF.Models.Services
{
    /// <summary>
    ///     Represents a service for storing the EoL-Test data to the database.
    /// </summary>
    public interface IDatabaseService
    {
        /// <summary>
        ///     Stores data in the database based on the provided settings and input parameters.
        /// </summary>
        /// <param name="connectionSettings">The configuration settings for the database connection.</param>
        /// <param name="tablesToDelete">The list of tables to delete data from.</param>
        /// <param name="unwantedColumns">The list of unwanted columns to exclude from the data storage process.</param>
        /// <param name="selectedFailureReason">The selected status of the EoL-Test.</param>
        /// <param name="seriesNum">The series number, which was given from the user.</param>
        /// <param name="fromTable">The source table.</param>
        /// <param name="toTable">The table the data is stored into.</param>
        /// <param name="partNum">The part number, which was given from the user (optional).</param>
        /// <returns>
        ///     A task that represents the asynchronous operation. 
        ///     The task result is a string indicating the result of the operation.
        ///     If something in fails an error is thrown.
        /// </returns>
        Task<string> StoreDataInDb(ConfigEOL connectionSettings, List<Tables> tablesToDelete, List<string> unwantedColumns, int selectedFailureReason, string seriesNum, Tables fromTable, Tables toTable, string? partNum = null);
    }
}
