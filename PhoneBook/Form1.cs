using System.Drawing;
using System.Windows.Forms;

namespace PhoneBook
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeUI();
        }

        // ── Елементи інтерфейсу ──────────────────────────────────────

        private ListBox listBoxContacts;

        private Label labelName;
        private Label labelPhone;
        private Label labelEmail;

        private TextBox textBoxName;
        private TextBox textBoxPhone;
        private TextBox textBoxEmail;

        private Button buttonAdd;
        private Button buttonEdit;
        private Button buttonDelete;
        private Button buttonClear;

        // ── Ініціалізація інтерфейсу ─────────────────────────────────

        private void InitializeUI()
        {
            // --- Вікно ---
            this.Text = "📒 PhoneBook — Телефонна книга";
            this.Size = new Size(620, 480);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.MinimumSize = new Size(620, 480);

            // --- ListBox ---
            listBoxContacts = new ListBox
            {
                Location = new Point(12, 12),
                Size = new Size(380, 410),
                Font = new Font("Segoe UI", 10),
                Anchor = AnchorStyles.Top | AnchorStyles.Left
                          | AnchorStyles.Bottom
            };

            // --- Labels ---
            labelName = new Label
            {
                Text = "Ім'я:",
                Location = new Point(410, 20),
                AutoSize = true,
                Font = new Font("Segoe UI", 9)
            };
            labelPhone = new Label
            {
                Text = "Телефон:",
                Location = new Point(410, 70),
                AutoSize = true,
                Font = new Font("Segoe UI", 9)
            };
            labelEmail = new Label
            {
                Text = "Email:",
                Location = new Point(410, 120),
                AutoSize = true,
                Font = new Font("Segoe UI", 9)
            };

            // --- TextBoxes ---
            textBoxName = new TextBox
            {
                Location = new Point(410, 40),
                Size = new Size(180, 25),
                Font = new Font("Segoe UI", 10),
            };
            textBoxPhone = new TextBox
            {
                Location = new Point(410, 90),
                Size = new Size(180, 25),
                Font = new Font("Segoe UI", 10),
            };
            textBoxEmail = new TextBox
            {
                Location = new Point(410, 140),
                Size = new Size(180, 25),
                Font = new Font("Segoe UI", 10),
            };

            // --- Buttons ---
            buttonAdd = new Button
            {
                Text = "Додати",
                Location = new Point(410, 185),
                Size = new Size(180, 35),
                Font = new Font("Segoe UI", 10)
            };
            buttonEdit = new Button
            {
                Text = "Редагувати",
                Location = new Point(410, 230),
                Size = new Size(180, 35),
                Font = new Font("Segoe UI", 10)
            };
            buttonDelete = new Button
            {
                Text = "Видалити",
                Location = new Point(410, 275),
                Size = new Size(180, 35),
                Font = new Font("Segoe UI", 10)
            };
            buttonClear = new Button
            {
                Text = "Очистити поля",
                Location = new Point(410, 330),
                Size = new Size(180, 35),
                Font = new Font("Segoe UI", 10)
            };

            // --- Додаємо на форму ---
            this.Controls.AddRange(new Control[]
            {
                listBoxContacts,
                labelName,  textBoxName,
                labelPhone, textBoxPhone,
                labelEmail, textBoxEmail,
                buttonAdd, buttonEdit, buttonDelete, buttonClear
            });
        }
    }
}