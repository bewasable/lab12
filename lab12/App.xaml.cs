using lab12;
using lab12.Data;
using lab12.Services;
using lab12.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Configuration;
using System.Data;
using System.Windows;

namespace lab12
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var services = new ServiceCollection();

            // ====================== SQL Server ======================
            string connectionString = "Server=localhost;Database=PhoneBookDB_Maltsev_2307e;Trusted_Connection=True;TrustServerCertificate=True;";

            services.AddDbContext<PhoneBookContext>(options =>
                options.UseSqlServer(connectionString));
            // =========================================================

            // Сервисы
            services.AddSingleton<IDialogService, DialogService>();
            services.AddSingleton<INavigationService, NavigationService>();

            // ViewModel'и
            services.AddTransient<ContactsListViewModel>();
            services.AddTransient<ContactEditViewModel>();
            services.AddTransient<AboutViewModel>();
            services.AddSingleton<MainWindowViewModel>();

            // Главное окно
            services.AddSingleton<MainWindow>(sp =>
            {
                var window = new MainWindow();
                window.DataContext = sp.GetRequiredService<MainWindowViewModel>();
                return window;
            });

            var provider = services.BuildServiceProvider();
            var mainWindow = provider.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }
    }

}
