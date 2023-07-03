using System.Threading.Tasks;
using System.Windows;

namespace ReWork_WPF.Models.Services
{
    /// <summary>
    ///     Represents a service for showing a message box to the user on the UI and not breaking MVVM architecture.
    /// </summary>
    public interface IMessageDisplayService
    {
        /// <summary>
        ///     Displays a message box with the specified text, caption, buttons, and icon, and returns the result.
        /// </summary>
        /// <param name="messageBoxText">The text to display in the message box.</param>
        /// <param name="caption">The text to display in the title bar of the message box.</param>
        /// <param name="button">One of the MessageBoxButton values that specifies which buttons to display in the message box.</param>
        /// <param name="icon">One of the MessageBoxImage values that specifies the icon to display in the message box.</param>
        /// <returns>
        ///     A task that represents the asynchronous operation. 
        ///     The task result contains the MessageBoxResult value that specifies which message box button is clicked by the user.
        /// </returns>
        Task<MessageBoxResult> ShowMessage(string messageBoxText, string caption, MessageBoxButton button, MessageBoxImage icon);
    }
}
