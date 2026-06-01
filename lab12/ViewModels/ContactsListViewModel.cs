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
        private readonly PhoneBookContext _context;

        public ObservableCollection<Contact> Contacts { get; } = new();

        private string _name = "";
        private string _phone = "";
        private Contact? _selectedContact;

        public string Name
        {
            get => _name;
            set { _name = value; OnPropertyChanged(); }
        }

        public string Phone
        {
            get => _phone;
            set { _phone = value; OnPropertyChanged(); }
        }

        public Contact? SelectedContact
        {
            get => _selectedContact;
            set { _selectedContact = value; OnPropertyChanged(); }
        }

        public ICommand AddCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand EditCommand { get; }

        public ContactsListViewModel(
            IDialogService dialogService,
            INavigationService navigation,
            PhoneBookContext context)
        {
            _dialogService = dialogService;
            _navigation = navigation;
            _context = context;

            LoadContactsFromDatabase();

            AddCommand = new RelayCommand(_ => AddContact());
            DeleteCommand = new RelayCommand(p => DeleteContact(p as Contact));
            EditCommand = new RelayCommand(_ => EditContact());
        }

        private void LoadContactsFromDatabase()
        {
            Contacts.Clear();
            var contactsFromDb = _context.Contacts.ToList();
            foreach (var contact in contactsFromDb)
            {
                Contacts.Add(contact);
            }
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

            if (_context.Contacts.Any(c => c.Phone == Phone))
            {
                _dialogService.ShowWarning("Контакт с таким номером уже существует");
                return;
            }

            var newContact = new Contact { Name = Name, Phone = Phone };
            _context.Contacts.Add(newContact);
            _context.SaveChanges();

            Contacts.Add(newContact);
            _dialogService.ShowInfo("Контакт успешно добавлен");

            Name = "";
            Phone = "";
        }

        private void DeleteContact(Contact? contact)
        {
            if (contact == null) return;

            if (_dialogService.ShowConfirmation($"Удалить контакт {contact.Name}?"))
            {
                _context.Contacts.Remove(contact);
                _context.SaveChanges();
                Contacts.Remove(contact);
                _dialogService.ShowInfo("Контакт удалён");
            }
        }

        private void EditContact()
        {
            if (SelectedContact != null)
            {
                _navigation.NavigateTo<ContactEditViewModel>(SelectedContact);
            }
            else
            {
                _dialogService.ShowWarning("Выберите контакт для редактирования");
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
