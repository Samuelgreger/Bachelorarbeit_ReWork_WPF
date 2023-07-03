using System.Threading.Tasks;
using System.Windows;

namespace ReWork_WPF.Models.Services
{
    public class MessageDisplayService : IMessageDisplayService
    {
        public Task<MessageBoxResult> ShowMessage(string messageBoxText, string caption, MessageBoxButton button, MessageBoxImage icon)
        {
            return Task.FromResult(MessageBox.Show(messageBoxText, caption, button, icon));
        }
    }
}
