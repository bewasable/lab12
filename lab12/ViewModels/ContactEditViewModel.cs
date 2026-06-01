using lab12.Data;
using lab12.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace lab12.ViewModels
{
    public class ContactEditViewModel : INotifyPropertyChanged, INavigationAware
    {
        private readonly INavigationService _navigation;
        private Contact? _contact;

        public string EditName
        {
            get => _contact?.Name ?? "";
            set
            {
                if (_contact != null)
                {
                    _contact.Name = value;
                    OnPropertyChanged();
                }
            }
        }

        public string EditPhone
        {
            get => _contact?.Phone ?? "";
            set
            {
                if (_contact != null)
                {
                    _contact.Phone = value;
                    OnPropertyChanged();
                }
            }
        }

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public ContactEditViewModel(INavigationService navigation)
        {
            _navigation = navigation;
            SaveCommand = new RelayCommand(_ => _navigation.NavigateTo<ContactsListViewModel>());
            CancelCommand = new RelayCommand(_ => _navigation.NavigateTo<ContactsListViewModel>());
        }

        public void OnNavigatedTo(object? parameter)
        {
            if (parameter is Contact contact)
            {
                _contact = contact;
                OnPropertyChanged(nameof(EditName));
                OnPropertyChanged(nameof(EditPhone));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
