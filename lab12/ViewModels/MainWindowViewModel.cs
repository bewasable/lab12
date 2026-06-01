using lab12.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace lab12.ViewModels
{
    public class MainWindowViewModel
    {
        private readonly INavigationService _navigation;

        public INavigationService NavigationService { get; }

        public ICommand ShowContactsCommand { get; }
        public ICommand ShowAboutCommand { get; }

        public MainWindowViewModel(INavigationService navigation)
        {
            _navigation = navigation;
            NavigationService = navigation;

            ShowContactsCommand = new RelayCommand(_ => _navigation.NavigateTo<ContactsListViewModel>());
            ShowAboutCommand = new RelayCommand(_ => _navigation.NavigateTo<AboutViewModel>());

            // При старте сразу показываем список контактов
            _navigation.NavigateTo<ContactsListViewModel>();
        }
    }
}
