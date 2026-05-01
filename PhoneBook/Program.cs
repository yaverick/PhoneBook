using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace PhoneBook
{
    // ── 5. Використання перерахування (enum) ─────────────────────────
    public enum ContactGroup
    {
        Родина,
        Друзі,
        Робота,
        Інше
    }

    // ── 1. Класи предметної області ──────────────────────────────────
    public class Contact
    {
        // ── 2. Інкапсуляція даних ────────────────────────────────────
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public ContactGroup Group { get; set; }

        // ── 3. Реалізація конструкторів ──────────────────────────────
        public Contact(string name, string phone, string email, ContactGroup group)
        {
            Name = name;
            Phone = phone;
            Email = email;
            Group = group;
        }

        public override string ToString()
        {
            return $"{Name} [{Group}] | {Phone}";
        }
    }

    // ── 4. Зв'язки між класами (PhoneBookManager містить Contact) ────
    public class PhoneBookManager
    {
        // Прихований список контактів (інкапсуляція)
        private List<Contact> _contacts;

        public PhoneBookManager()
        {
            _contacts = new List<Contact>();
        }

        // Публічний доступ до списку тільки для читання, щоб уникнути пошкодження даних
        public IReadOnlyList<Contact> Contacts => _contacts.AsReadOnly();

        public void AddContact(Contact contact)
        {
            _contacts.Add(contact);
        }

        public void RemoveContact(Contact contact)
        {
            if (_contacts.Contains(contact))
            {
                _contacts.Remove(contact);
            }
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