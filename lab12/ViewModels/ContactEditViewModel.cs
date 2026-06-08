using lab12.Data;
using lab12.Services;
using Microsoft.EntityFrameworkCore;
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
        private readonly IDbContextFactory<PhoneBookContext> _contextFactory;

        private Contact? _contact;
        private int _contactId;

        public string EditName { get; set; } = "";
        public string EditPhone { get; set; } = "";

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public ContactEditViewModel(INavigationService navigation, IDbContextFactory<PhoneBookContext> contextFactory)
        {
            _navigation = navigation;
            _contextFactory = contextFactory;

            SaveCommand = new RelayCommand(_ => SaveChanges());
            CancelCommand = new RelayCommand(_ => _navigation.NavigateTo<ContactsListViewModel>());
        }

        public void OnNavigatedTo(object? parameter)
        {
            if (parameter is Contact contact)
            {
                _contact = contact;
                _contactId = contact.Id;
                EditName = contact.Name;
                EditPhone = contact.Phone;
                OnPropertyChanged(nameof(EditName));
                OnPropertyChanged(nameof(EditPhone));
            }
        }

        private void SaveChanges()
        {
            using var context = _contextFactory.CreateDbContext();

            if (_contactId == 0) // Новый контакт
            {
                var newContact = new Contact { Name = EditName, Phone = EditPhone };
                context.Contacts.Add(newContact);
            }
            else // Редактирование (Fetch-Modify-Save)
            {
                var contactToUpdate = context.Contacts.Find(_contactId);
                if (contactToUpdate != null)
                {
                    contactToUpdate.Name = EditName;
                    contactToUpdate.Phone = EditPhone;
                }
            }

            context.SaveChanges();
            _navigation.NavigateTo<ContactsListViewModel>();
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
