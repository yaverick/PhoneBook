using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows.Forms;

namespace PhoneBook
{
    // ── Модель даних ─────────────────────────────────────────────────
    public enum ContactGroup { Родина, Друзі, Робота, Інше }

    public class Contact
    {
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public ContactGroup Group { get; set; }

        // Порожній конструктор обов'язковий для JSON десеріалізації
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

    // ── Абстракція ───────────────────────────────────────────────────
    public interface IRepository<T>
    {
        void Add(T item);
        IEnumerable<T> GetAll();
        void Update(T item);
        void Remove(T item);
        IEnumerable<T> Find(Func<T, bool> predicate);
    }

    // ── Реалізація репозиторію з підтримкою файлів (Практична №5) ────
    public class ContactRepository : IRepository<Contact>
    {
        private List<Contact> _contacts;
        private readonly string _filePath = "contacts.json"; // Ім'я файлу для збереження

        public ContactRepository()
        {
            LoadFromFile(); // 3. Завантаження даних при запуску застосунку
        }

        // 1 та 2. Серіалізація об'єктів у формат JSON та збереження у файл
        private void SaveToFile()
        {
            // Налаштування для красивого форматування (з відступами)
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
                // Якщо файлу ще немає (перший запуск)
                _contacts = new List<Contact>();
            }
        }

        // 4. Інтеграція механізму збереження з логікою програми
        public void Add(Contact item)
        {
            _contacts.Add(item);
            SaveToFile(); // Зберігаємо після додавання
        }

        public IEnumerable<Contact> GetAll() => _contacts.AsReadOnly();

        public void Update(Contact item)
        {
            int index = _contacts.FindIndex(c => c.Name == item.Name && c.Phone == item.Phone);
            if (index != -1)
            {
                _contacts[index] = item;
                SaveToFile(); // Зберігаємо після редагування
            }
        }

        public void Remove(Contact item)
        {
            _contacts.Remove(item);
            SaveToFile(); // Зберігаємо після видалення
        }

        public IEnumerable<Contact> Find(Func<Contact, bool> predicate)
        {
            return _contacts.Where(predicate).ToList();
        }
    }

    // ── Точка входу ──────────────────────────────────────────────────
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