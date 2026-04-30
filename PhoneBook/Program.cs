using System;
using System.Windows.Forms;

namespace PhoneBook
{
    // ── Модель даних ─────────────────────────────────────────────────
    public class Contact
    {
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }

        public Contact(string name, string phone, string email)
        {
            Name = name;
            Phone = phone;
            Email = email;
        }

        public override string ToString()
        {
            return $"{Name} | {Phone} | {Email}";
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