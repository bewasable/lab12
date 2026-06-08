using lab12.Data;
using lab12.Services;
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;

namespace lab12.ViewModels
{
    public class ContactsListViewModel : INotifyPropertyChanged
    {
        private readonly IDialogService _dialogService;
        private readonly INavigationService _navigation;
        private readonly IDbContextFactory<PhoneBookContext> _contextFactory;

        public ObservableCollection<Contact> Contacts { get; } = new();

        private string _name = "";
        private string _phone = "";
        private Contact? _selectedContact;

        public string Name { get => _name; set { _name = value; OnPropertyChanged(); } }
        public string Phone { get => _phone; set { _phone = value; OnPropertyChanged(); } }
        public Contact? SelectedContact { get => _selectedContact; set { _selectedContact = value; OnPropertyChanged(); } }

        public ICommand AddCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand EditCommand { get; }

        public ContactsListViewModel(
            IDialogService dialogService,
            INavigationService navigation,
            IDbContextFactory<PhoneBookContext> contextFactory)
        {
            _dialogService = dialogService;
            _navigation = navigation;
            _contextFactory = contextFactory;

            LoadContacts();

            AddCommand = new RelayCommand(_ => AddContact());
            DeleteCommand = new RelayCommand(p => DeleteContact(p as Contact));
            EditCommand = new RelayCommand(_ => EditContact());
        }

        private void LoadContacts()
        {
            Contacts.Clear();
            using var context = _contextFactory.CreateDbContext();
            foreach (var c in context.Contacts.ToList())
                Contacts.Add(c);
        }

        private void AddContact()
        {
            if (string.IsNullOrWhiteSpace(Name) || string.IsNullOrWhiteSpace(Phone))
            {
                _dialogService.ShowError("Введите имя и телефон");
                return;
            }

            if (!Regex.IsMatch(Phone, @"^(\+7\d{10}|\d{10})$"))
            {
                _dialogService.ShowError("Неверный формат телефона");
                return;
            }

            using var context = _contextFactory.CreateDbContext();
            if (context.Contacts.Any(c => c.Phone == Phone))
            {
                _dialogService.ShowWarning("Контакт с таким номером уже существует");
                return;
            }

            var newContact = new Contact { Name = Name, Phone = Phone };
            context.Contacts.Add(newContact);
            context.SaveChanges();

            Contacts.Add(newContact);
            _dialogService.ShowInfo("Контакт добавлен");

            Name = "";
            Phone = "";
        }

        private void DeleteContact(Contact? contact)
        {
            if (contact == null) return;

            if (_dialogService.ShowConfirmation($"Удалить {contact.Name}?"))
            {
                using var context = _contextFactory.CreateDbContext();
                var toDelete = context.Contacts.Find(contact.Id);
                if (toDelete != null)
                {
                    context.Contacts.Remove(toDelete);
                    context.SaveChanges();
                }
                Contacts.Remove(contact);
                _dialogService.ShowInfo("Контакт удалён");
            }
        }

        private void EditContact()
        {
            if (SelectedContact != null)
                _navigation.NavigateTo<ContactEditViewModel>(SelectedContact);
            else
                _dialogService.ShowWarning("Выберите контакт");
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
