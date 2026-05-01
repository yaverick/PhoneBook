using System;
using System.Drawing;
using System.Windows.Forms;

namespace PhoneBook
{
    public partial class Form1 : Form
    {
        // Екземпляр нашого менеджера контактів (зв'язок класів)
        private PhoneBookManager _manager;

        public Form1()
        {
            _manager = new PhoneBookManager();
            InitializeUI();
            SetupEventHandlers(); // Підключаємо логіку кнопок
        }

        // ── Елементи інтерфейсу ──────────────────────────────────────

        private ListBox listBoxContacts;

        private Label labelName;
        private Label labelPhone;
        private Label labelEmail;
        private Label labelGroup; // Новий Label для Enum

        private TextBox textBoxName;
        private TextBox textBoxPhone;
        private TextBox textBoxEmail;
        private ComboBox comboBoxGroup; // Dropdown для Enum

        private Button buttonAdd;
        private Button buttonEdit;
        private Button buttonDelete;
        private Button buttonClear;

        // ── Ініціалізація інтерфейсу ─────────────────────────────────

        private void InitializeUI()
        {
            // --- Вікно ---
            this.Text = "📒 PhoneBook — Телефонна книга";
            this.Size = new Size(620, 520); // Трохи збільшив висоту для ComboBox
            this.StartPosition = FormStartPosition.CenterScreen;
            this.MinimumSize = new Size(620, 520);

            // --- ListBox ---
            listBoxContacts = new ListBox
            {
                Location = new Point(12, 12),
                Size = new Size(380, 450),
                Font = new Font("Segoe UI", 10),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Bottom
            };

            // --- Labels ---
            labelName = new Label { Text = "Ім'я:", Location = new Point(410, 20), AutoSize = true, Font = new Font("Segoe UI", 9) };
            labelPhone = new Label { Text = "Телефон:", Location = new Point(410, 70), AutoSize = true, Font = new Font("Segoe UI", 9) };
            labelEmail = new Label { Text = "Email:", Location = new Point(410, 120), AutoSize = true, Font = new Font("Segoe UI", 9) };
            labelGroup = new Label { Text = "Група:", Location = new Point(410, 170), AutoSize = true, Font = new Font("Segoe UI", 9) };

            // --- TextBoxes & ComboBox ---
            textBoxName = new TextBox { Location = new Point(410, 40), Size = new Size(180, 25), Font = new Font("Segoe UI", 10) };
            textBoxPhone = new TextBox { Location = new Point(410, 90), Size = new Size(180, 25), Font = new Font("Segoe UI", 10) };
            textBoxEmail = new TextBox { Location = new Point(410, 140), Size = new Size(180, 25), Font = new Font("Segoe UI", 10) };

            comboBoxGroup = new ComboBox
            {
                Location = new Point(410, 190),
                Size = new Size(180, 25),
                Font = new Font("Segoe UI", 10),
                DropDownStyle = ComboBoxStyle.DropDownList // Щоб не можна було вводити свій текст
            };
            // Заповнюємо ComboBox значеннями з нашого Enum
            comboBoxGroup.DataSource = Enum.GetValues(typeof(ContactGroup));

            // --- Buttons ---
            buttonAdd = new Button { Text = "Додати", Location = new Point(410, 235), Size = new Size(180, 35), Font = new Font("Segoe UI", 10) };
            buttonEdit = new Button { Text = "Редагувати", Location = new Point(410, 280), Size = new Size(180, 35), Font = new Font("Segoe UI", 10), Enabled = false }; // Вимкнена, поки не вибрано контакт
            buttonDelete = new Button { Text = "Видалити", Location = new Point(410, 325), Size = new Size(180, 35), Font = new Font("Segoe UI", 10) };
            buttonClear = new Button { Text = "Очистити поля", Location = new Point(410, 380), Size = new Size(180, 35), Font = new Font("Segoe UI", 10) };

            // --- Додаємо на форму ---
            this.Controls.AddRange(new Control[]
            {
                listBoxContacts,
                labelName,  textBoxName,
                labelPhone, textBoxPhone,
                labelEmail, textBoxEmail,
                labelGroup, comboBoxGroup,
                buttonAdd, buttonEdit, buttonDelete, buttonClear
            });
        }

        // ── Логіка роботи програми (Events) ──────────────────────────

        private void SetupEventHandlers()
        {
            buttonAdd.Click += ButtonAdd_Click;
            buttonClear.Click += ButtonClear_Click;
            buttonDelete.Click += ButtonDelete_Click;
            listBoxContacts.SelectedIndexChanged += ListBoxContacts_SelectedIndexChanged;
        }

        private void ButtonAdd_Click(object sender, EventArgs e)
        {
            // Перевірка, чи не порожні поля
            if (string.IsNullOrWhiteSpace(textBoxName.Text) || string.IsNullOrWhiteSpace(textBoxPhone.Text))
            {
                MessageBox.Show("Будь ласка, заповніть хоча б ім'я та телефон.", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Створюємо новий об'єкт
            Contact newContact = new Contact(
                textBoxName.Text,
                textBoxPhone.Text,
                textBoxEmail.Text,
                (ContactGroup)comboBoxGroup.SelectedItem // Беремо значення Enum
            );

            // Додаємо через менеджера і оновлюємо список
            _manager.AddContact(newContact);
            UpdateListBox();
            ClearInputFields();
        }

        private void ButtonDelete_Click(object sender, EventArgs e)
        {
            if (listBoxContacts.SelectedItem is Contact selectedContact)
            {
                _manager.RemoveContact(selectedContact);
                UpdateListBox();
                ClearInputFields();
            }
        }

        private void ButtonClear_Click(object sender, EventArgs e)
        {
            ClearInputFields();
        }

        // Відображення даних при кліку на контакт у списку
        private void ListBoxContacts_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBoxContacts.SelectedItem is Contact selectedContact)
            {
                textBoxName.Text = selectedContact.Name;
                textBoxPhone.Text = selectedContact.Phone;
                textBoxEmail.Text = selectedContact.Email;
                comboBoxGroup.SelectedItem = selectedContact.Group;
                buttonEdit.Enabled = true; // Можна додати логіку редагування пізніше
            }
            else
            {
                buttonEdit.Enabled = false;
            }
        }

        // ── Допоміжні методи ─────────────────────────────────────────

        private void UpdateListBox()
        {
            listBoxContacts.Items.Clear();
            foreach (var contact in _manager.Contacts)
            {
                listBoxContacts.Items.Add(contact);
            }
        }

        private void ClearInputFields()
        {
            textBoxName.Clear();
            textBoxPhone.Clear();
            textBoxEmail.Clear();
            comboBoxGroup.SelectedIndex = 0;
            listBoxContacts.ClearSelected();
        }
    }
}