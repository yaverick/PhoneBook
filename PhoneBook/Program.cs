using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows.Forms;

namespace PhoneBook
{
    // Перелік для категоризації контактів у телефонній книзі
    public enum ContactGroup { Родина, Друзі, Робота, Інше }

    // Клас-модель для представлення одного контакту
    public class Contact
    {
        // Атрибути валідації для перевірки введених користувачем даних
        [Required(ErrorMessage = "Поле 'Ім'я' є обов'язковим.")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Ім'я має містити від 2 до 50 символів.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Поле 'Телефон' є обов'язковим.")]
        [RegularExpression(@"^\+?\d{10,15}$", ErrorMessage = "Телефон має містити від 10 до 15 цифр і може починатися з '+'.")]
        public string Phone { get; set; }

        [EmailAddress(ErrorMessage = "Некоректний формат Email-адреси.")]
        public string Email { get; set; }

        public ContactGroup Group { get; set; }

        // Порожній конструктор, який вимагає бібліотека System.Text.Json для завантаження з файлу
        public Contact() { }

        // Основний конструктор для створення об'єкта в програмі
        public Contact(string name, string phone, string email, ContactGroup group)
        {
            Name = name;
            Phone = phone;
            Email = email;
            Group = group;
        }

        // Перевизначення методу для гарного відображення у списку ListBox
        public override string ToString() => $"{Name} [{Group}] | {Phone}";
    }

    // Універсальний інтерфейс для реалізації CRUD-операцій
    public interface IRepository<T>
    {
        void Add(T item);
        IEnumerable<T> GetAll();
        void Update(T item);
        void Remove(T item);
        IEnumerable<T> Find(Func<T, bool> predicate);
    }

    // Клас для управління списком контактів та їх збереженням
    public class ContactRepository : IRepository<Contact>
    {
        private List<Contact> _contacts;

        // Шлях до файлу тепер можна змінювати, що корисно для тестування
        private readonly string _filePath;

        // Конструктор приймає шлях до файлу (за замовчуванням це contacts.json)
        public ContactRepository(string filePath = "contacts.json")
        {
            _filePath = filePath;
            LoadFromFile();
        }

        // Метод для збереження поточного списку у JSON файл
        private void SaveToFile()
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            string jsonString = JsonSerializer.Serialize(_contacts, options);
            File.WriteAllText(_filePath, jsonString);
        }

        // Метод для зчитування даних з файлу під час старту
        private void LoadFromFile()
        {
            if (File.Exists(_filePath))
            {
                string jsonString = File.ReadAllText(_filePath);
                _contacts = JsonSerializer.Deserialize<List<Contact>>(jsonString) ?? new List<Contact>();
            }
            else
            {
                // Якщо файлу ще немає, створюємо порожній список
                _contacts = new List<Contact>();
            }
        }

        // Додавання контакту з перевіркою бізнес-правила
        public void Add(Contact item)
        {
            string newPhone = item.Phone.Replace(" ", "");

            // Перевірка на унікальність номера телефону
            bool isDuplicate = _contacts.Any(c => c.Phone.Replace(" ", "") == newPhone);

            if (isDuplicate)
            {
                throw new InvalidOperationException($"Контакт з номером {item.Phone} вже існує в базі!");
            }

            _contacts.Add(item);
            SaveToFile();
        }

        // Повернення всіх контактів (доступ лише для читання)
        public IEnumerable<Contact> GetAll() => _contacts.AsReadOnly();

        // Оновлення існуючого контакту
        public void Update(Contact item)
        {
            int index = _contacts.FindIndex(c => c.Phone == item.Phone);
            if (index != -1)
            {
                _contacts[index] = item;
                SaveToFile();
            }
        }

        // Видалення контакту
        public void Remove(Contact item)
        {
            _contacts.Remove(item);
            SaveToFile();
        }

        // Пошук контактів за певним критерієм (ім'ям або телефоном)
        public IEnumerable<Contact> Find(Func<Contact, bool> predicate)
        {
            return _contacts.Where(predicate).ToList();
        }
    }

    static class Program
    {
        // Точка входу в програму
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}