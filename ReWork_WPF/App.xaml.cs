using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NLog;
using ReWork_WPF.Models.Services;
using ReWork_WPF.ViewModels;
using ReWork_WPF.Views;
using System.Windows;

namespace ReWork_WPF
{
    public partial class App : Application
    {
        public static IHost? AppHost { get; private set; }
        
        public App()
        {
            AppHost = Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) =>
                {
                    services.AddSingleton<StoreDataView>();

                    services.AddSingleton<StoreDataViewModel>();

                    services.AddSingleton<IAppSettingsService, AppSettingsService>();
                    services.AddSingleton<IDatabaseService, DatabaseService>();
                    services.AddSingleton<IMessageDisplayService, MessageDisplayService>();

                    services.AddSingleton(LogManager.GetCurrentClassLogger());
                })
                .Build();
        }
        protected override async void OnStartup(StartupEventArgs e)
        {
            await AppHost!.StartAsync();

            var startup = AppHost.Services.GetRequiredService<StoreDataView>();

            startup.Show();

            base.OnStartup(e);
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            await AppHost!.StopAsync();

            base.OnExit(e);
        }
    }
}
