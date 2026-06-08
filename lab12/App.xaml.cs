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

            string connectionString = "Server=localhost;Database=PhoneBookDB_Maltsev_2307e;Trusted_Connection=True;TrustServerCertificate=True;";

            // === Lab 14: Используем фабрику вместо AddDbContext ===
            services.AddDbContextFactory<PhoneBookContext>(options =>
                options.UseSqlServer(connectionString));

            services.AddSingleton<IDialogService, DialogService>();
            services.AddSingleton<INavigationService, NavigationService>();

            services.AddTransient<ContactsListViewModel>();
            services.AddTransient<ContactEditViewModel>();
            services.AddTransient<AboutViewModel>();
            services.AddSingleton<MainWindowViewModel>();

            services.AddSingleton<MainWindow>(sp =>
            {
                var window = new MainWindow();
                window.DataContext = sp.GetRequiredService<MainWindowViewModel>();
                return window;
            });

            var provider = services.BuildServiceProvider();
            provider.GetRequiredService<MainWindow>().Show();
        }
    }
}
