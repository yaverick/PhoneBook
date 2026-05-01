using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations; // ДОДАНО ДЛЯ ВАЛІДАЦІЇ
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows.Forms;

namespace PhoneBook
{
    public enum ContactGroup { Родина, Друзі, Робота, Інше }

    // ── 3. Використання атрибутів валідації ──────────────────────────
    public class Contact
    {
        [Required(ErrorMessage = "Поле 'Ім'я' є обов'язковим.")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Ім'я має містити від 2 до 50 символів.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Поле 'Телефон' є обов'язковим.")]
        [RegularExpression(@"^\+?\d{10,15}$", ErrorMessage = "Телефон має містити від 10 до 15 цифр і може починатися з '+'.")]
        public string Phone { get; set; }

        // Email не обов'язковий, але якщо введений - перевіряємо формат
        [EmailAddress(ErrorMessage = "Некоректний формат Email-адреси.")]
        public string Email { get; set; }

        public ContactGroup Group { get; set; }

        public Contact() { }

        public Contact(string name, string phone, string email, ContactGroup group)
        {
            Name = name;
            Phone = phone;
            Email = email;
            Group = group;
        }

        public override string ToString() => $"{Name} [{Group}] | {Phone}";
    }

    public interface IRepository<T>
    {
        void Add(T item);
        IEnumerable<T> GetAll();
        void Update(T item);
        void Remove(T item);
        IEnumerable<T> Find(Func<T, bool> predicate);
    }

    public class ContactRepository : IRepository<Contact>
    {
        private List<Contact> _contacts;
        private readonly string _filePath = "contacts.json";

        public ContactRepository()
        {
            LoadFromFile();
        }

        private void SaveToFile()
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            string jsonString = JsonSerializer.Serialize(_contacts, options);
            File.WriteAllText(_filePath, jsonString);
        }

        private void LoadFromFile()
        {
            if (File.Exists(_filePath))
            {
                string jsonString = File.ReadAllText(_filePath);
                _contacts = JsonSerializer.Deserialize<List<Contact>>(jsonString) ?? new List<Contact>();
            }
            else
            {
                _contacts = new List<Contact>();
            }
        }

        // ── 1. Реалізація основного бізнес-правила ───────────────────
        public void Add(Contact item)
        {
            // Очищаємо номери від пробілів для точного порівняння
            string newPhone = item.Phone.Replace(" ", "");

            // Бізнес-правило: Унікальність номера телефону
            bool isDuplicate = _contacts.Any(c => c.Phone.Replace(" ", "") == newPhone);

            if (isDuplicate)
            {
                throw new InvalidOperationException($"Контакт з номером {item.Phone} вже існує в базі!");
            }

            _contacts.Add(item);
            SaveToFile();
        }

        public IEnumerable<Contact> GetAll() => _contacts.AsReadOnly();

        public void Update(Contact item)
        {
            int index = _contacts.FindIndex(c => c.Phone == item.Phone); // Шукаємо за телефоном
            if (index != -1)
            {
                _contacts[index] = item;
                SaveToFile();
            }
        }

        public void Remove(Contact item)
        {
            _contacts.Remove(item);
            SaveToFile();
        }

        public IEnumerable<Contact> Find(Func<Contact, bool> predicate)
        {
            return _contacts.Where(predicate).ToList();
        }
    }

    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}