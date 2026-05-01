using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace PhoneBook
{
    public partial class Form1 : Form
    {
        private IRepository<Contact> _repository;

        public Form1()
        {
            _repository = new ContactRepository(); // Репозиторій автоматично зчитує файл тут
            InitializeUI();
            SetupEventHandlers();

            // Оновлюємо список одразу при старті, щоб показати завантажені дані
            UpdateListBox();
        }

        // ── Елементи інтерфейсу ──────────────────────────────────────
        private ListBox listBoxContacts;
        private Label labelSearch;
        private TextBox textBoxSearch;
        private Label labelName, labelPhone, labelEmail, labelGroup;
        private TextBox textBoxName, textBoxPhone, textBoxEmail;
        private ComboBox comboBoxGroup;
        private Button buttonAdd, buttonEdit, buttonDelete, buttonClear;

        // ── Ініціалізація інтерфейсу ─────────────────────────────────
        private void InitializeUI()
        {
            this.Text = "📒 PhoneBook — Практична 5 (JSON Збереження)";
            this.Size = new Size(620, 520);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.MinimumSize = new Size(620, 520);

            labelSearch = new Label { Text = "Пошук:", Location = new Point(12, 16), AutoSize = true, Font = new Font("Segoe UI", 9) };
            textBoxSearch = new TextBox { Location = new Point(65, 13), Size = new Size(327, 25), Font = new Font("Segoe UI", 10) };

            listBoxContacts = new ListBox
            {
                Location = new Point(12, 45),
                Size = new Size(380, 415),
                Font = new Font("Segoe UI", 10),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Bottom
            };

            labelName = new Label { Text = "Ім'я:", Location = new Point(410, 20), AutoSize = true, Font = new Font("Segoe UI", 9) };
            labelPhone = new Label { Text = "Телефон:", Location = new Point(410, 70), AutoSize = true, Font = new Font("Segoe UI", 9) };
            labelEmail = new Label { Text = "Email:", Location = new Point(410, 120), AutoSize = true, Font = new Font("Segoe UI", 9) };
            labelGroup = new Label { Text = "Група:", Location = new Point(410, 170), AutoSize = true, Font = new Font("Segoe UI", 9) };

            textBoxName = new TextBox { Location = new Point(410, 40), Size = new Size(180, 25), Font = new Font("Segoe UI", 10) };
            textBoxPhone = new TextBox { Location = new Point(410, 90), Size = new Size(180, 25), Font = new Font("Segoe UI", 10) };
            textBoxEmail = new TextBox { Location = new Point(410, 140), Size = new Size(180, 25), Font = new Font("Segoe UI", 10) };

            comboBoxGroup = new ComboBox
            {
                Location = new Point(410, 190),
                Size = new Size(180, 25),
                Font = new Font("Segoe UI", 10),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            comboBoxGroup.DataSource = Enum.GetValues(typeof(ContactGroup));

            buttonAdd = new Button { Text = "Додати", Location = new Point(410, 235), Size = new Size(180, 35), Font = new Font("Segoe UI", 10) };
            buttonEdit = new Button { Text = "Редагувати", Location = new Point(410, 280), Size = new Size(180, 35), Font = new Font("Segoe UI", 10), Enabled = false };
            buttonDelete = new Button { Text = "Видалити", Location = new Point(410, 325), Size = new Size(180, 35), Font = new Font("Segoe UI", 10) };
            buttonClear = new Button { Text = "Очистити поля", Location = new Point(410, 380), Size = new Size(180, 35), Font = new Font("Segoe UI", 10) };

            this.Controls.AddRange(new Control[]
            {
                labelSearch, textBoxSearch, listBoxContacts,
                labelName, textBoxName, labelPhone, textBoxPhone,
                labelEmail, textBoxEmail, labelGroup, comboBoxGroup,
                buttonAdd, buttonEdit, buttonDelete, buttonClear
            });
        }

        // ── Логіка роботи програми ───────────────────────────────────
        private void SetupEventHandlers()
        {
            buttonAdd.Click += ButtonAdd_Click;
            buttonEdit.Click += ButtonEdit_Click;
            buttonDelete.Click += ButtonDelete_Click;
            buttonClear.Click += ButtonClear_Click;

            listBoxContacts.SelectedIndexChanged += ListBoxContacts_SelectedIndexChanged;
            textBoxSearch.TextChanged += TextBoxSearch_TextChanged;
        }

        private void TextBoxSearch_TextChanged(object sender, EventArgs e)
        {
            string query = textBoxSearch.Text.ToLower();
            if (string.IsNullOrWhiteSpace(query))
            {
                UpdateListBox();
            }
            else
            {
                var filtered = _repository.Find(c => c.Name.ToLower().Contains(query) || c.Phone.Contains(query));
                UpdateListBox(filtered);
            }
        }

        private void ButtonAdd_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBoxName.Text) || string.IsNullOrWhiteSpace(textBoxPhone.Text))
            {
                MessageBox.Show("Заповніть ім'я та телефон.", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Contact newContact = new Contact(textBoxName.Text, textBoxPhone.Text, textBoxEmail.Text, (ContactGroup)comboBoxGroup.SelectedItem);
            _repository.Add(newContact); // Тепер цей метод автоматично зберігає у файл

            UpdateListBox();
            ClearInputFields();
        }

        private void ButtonEdit_Click(object sender, EventArgs e)
        {
            if (listBoxContacts.SelectedItem is Contact selectedContact)
            {
                if (string.IsNullOrWhiteSpace(textBoxName.Text) || string.IsNullOrWhiteSpace(textBoxPhone.Text))
                {
                    MessageBox.Show("Ім'я та телефон не можуть бути порожніми.", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                selectedContact.Name = textBoxName.Text;
                selectedContact.Phone = textBoxPhone.Text;
                selectedContact.Email = textBoxEmail.Text;
                selectedContact.Group = (ContactGroup)comboBoxGroup.SelectedItem;

                _repository.Update(selectedContact); // Тепер цей метод автоматично зберігає у файл

                UpdateListBox();
                ClearInputFields();
            }
        }

        private void ButtonDelete_Click(object sender, EventArgs e)
        {
            if (listBoxContacts.SelectedItem is Contact selectedContact)
            {
                _repository.Remove(selectedContact); // Тепер цей метод автоматично зберігає у файл
                UpdateListBox();
                ClearInputFields();
            }
        }

        private void ButtonClear_Click(object sender, EventArgs e) => ClearInputFields();

        private void ListBoxContacts_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBoxContacts.SelectedItem is Contact selectedContact)
            {
                textBoxName.Text = selectedContact.Name;
                textBoxPhone.Text = selectedContact.Phone;
                textBoxEmail.Text = selectedContact.Email;
                comboBoxGroup.SelectedItem = selectedContact.Group;
                buttonEdit.Enabled = true;
            }
            else
            {
                buttonEdit.Enabled = false;
            }
        }

        private void UpdateListBox(IEnumerable<Contact> contactsToDisplay = null)
        {
            listBoxContacts.Items.Clear();
            var items = contactsToDisplay ?? _repository.GetAll();

            foreach (var contact in items)
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
            buttonEdit.Enabled = false;
        }
    }
}