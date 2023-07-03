using System.Windows;

namespace Rework_WPF_Testing.Services
{
    public class TestMessageDisplayService : IMessageDisplayService
    {
        public Task<MessageBoxResult> ShowMessage(string messageBoxText, string caption, MessageBoxButton button, MessageBoxImage icon)
        {
            return Task.FromResult(MessageBoxResult.OK);
        }
    }
}
