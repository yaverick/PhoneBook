using System;
using System.Collections.Generic;
using System.Linq;
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

        public Contact(string name, string phone, string email, ContactGroup group)
        {
            Name = name;
            Phone = phone;
            Email = email;
            Group = group;
        }

        public override string ToString() => $"{Name} [{Group}] | {Phone}";
    }

    // ── 2. Реалізація інтерфейсу IRepository<T> (Generics) ───────────
    public interface IRepository<T>
    {
        void Add(T item);
        void Remove(T item);
        IEnumerable<T> GetAll();
        IEnumerable<T> Find(Func<T, bool> predicate); // 3. Пошук об'єктів
    }

    // ── 4. Відокремлення логіки (Паттерн Repository) ─────────────────
    public class ContactRepository : IRepository<Contact>
    {
        // 1. Зберігання об'єктів у колекції
        private List<Contact> _contacts = new List<Contact>();

        public void Add(Contact item)
        {
            _contacts.Add(item);
        }

        public void Remove(Contact item)
        {
            _contacts.Remove(item);
        }

        public IEnumerable<Contact> GetAll()
        {
            return _contacts.AsReadOnly();
        }

        // Реалізація пошуку за допомогою LINQ
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