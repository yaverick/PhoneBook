using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations; // ДОДАНО ДЛЯ ВАЛІДАЦІЇ
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace PhoneBook
{
    public partial class Form1 : Form
    {
        private IRepository<Contact> _repository;

        public Form1()
        {
            _repository = new ContactRepository();
            InitializeUI();
            SetupEventHandlers();
            UpdateListBox();
        }

        private ListBox listBoxContacts;
        private Label labelSearch;
        private TextBox textBoxSearch;
        private Label labelName, labelPhone, labelEmail, labelGroup;
        private TextBox textBoxName, textBoxPhone, textBoxEmail;
        private ComboBox comboBoxGroup;
        private Button buttonAdd, buttonEdit, buttonDelete, buttonClear;

        private void InitializeUI()
        {
            this.Text = "📒 PhoneBook — Практична 6 (Валідація)";
            this.Size = new Size(620, 520);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.MinimumSize = new Size(620, 520);

            labelSearch = new Label { Text = "Пошук:", Location = new Point(12, 16), AutoSize = true, Font = new Font("Segoe UI", 9) };
            textBoxSearch = new TextBox { Location = new Point(65, 13), Size = new Size(327, 25), Font = new Font("Segoe UI", 10) };

            listBoxContacts = new ListBox { Location = new Point(12, 45), Size = new Size(380, 415), Font = new Font("Segoe UI", 10), Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Bottom };

            labelName = new Label { Text = "Ім'я:", Location = new Point(410, 20), AutoSize = true, Font = new Font("Segoe UI", 9) };
            labelPhone = new Label { Text = "Телефон:", Location = new Point(410, 70), AutoSize = true, Font = new Font("Segoe UI", 9) };
            labelEmail = new Label { Text = "Email:", Location = new Point(410, 120), AutoSize = true, Font = new Font("Segoe UI", 9) };
            labelGroup = new Label { Text = "Група:", Location = new Point(410, 170), AutoSize = true, Font = new Font("Segoe UI", 9) };

            textBoxName = new TextBox { Location = new Point(410, 40), Size = new Size(180, 25), Font = new Font("Segoe UI", 10) };
            textBoxPhone = new TextBox { Location = new Point(410, 90), Size = new Size(180, 25), Font = new Font("Segoe UI", 10) };
            textBoxEmail = new TextBox { Location = new Point(410, 140), Size = new Size(180, 25), Font = new Font("Segoe UI", 10) };

            comboBoxGroup = new ComboBox { Location = new Point(410, 190), Size = new Size(180, 25), Font = new Font("Segoe UI", 10), DropDownStyle = ComboBoxStyle.DropDownList };
            comboBoxGroup.DataSource = Enum.GetValues(typeof(ContactGroup));

            buttonAdd = new Button { Text = "Додати", Location = new Point(410, 235), Size = new Size(180, 35), Font = new Font("Segoe UI", 10) };
            buttonEdit = new Button { Text = "Редагувати", Location = new Point(410, 280), Size = new Size(180, 35), Font = new Font("Segoe UI", 10), Enabled = false };
            buttonDelete = new Button { Text = "Видалити", Location = new Point(410, 325), Size = new Size(180, 35), Font = new Font("Segoe UI", 10) };
            buttonClear = new Button { Text = "Очистити поля", Location = new Point(410, 380), Size = new Size(180, 35), Font = new Font("Segoe UI", 10) };

            this.Controls.AddRange(new Control[] { labelSearch, textBoxSearch, listBoxContacts, labelName, textBoxName, labelPhone, textBoxPhone, labelEmail, textBoxEmail, labelGroup, comboBoxGroup, buttonAdd, buttonEdit, buttonDelete, buttonClear });
        }

        private void SetupEventHandlers()
        {
            buttonAdd.Click += ButtonAdd_Click;
            buttonEdit.Click += ButtonEdit_Click;
            buttonDelete.Click += ButtonDelete_Click;
            buttonClear.Click += ButtonClear_Click;
            listBoxContacts.SelectedIndexChanged += ListBoxContacts_SelectedIndexChanged;
            textBoxSearch.TextChanged += TextBoxSearch_TextChanged;
        }

        // ── 2 та 4. Метод для валідації та збору повідомлень про помилки ─
        private bool ValidateContact(Contact contact, out string errorMessage)
        {
            var context = new ValidationContext(contact, serviceProvider: null, items: null);
            var results = new List<ValidationResult>();

            // Цей метод перевіряє всі атрибути [Required], [StringLength] і т.д.
            bool isValid = Validator.TryValidateObject(contact, context, results, true);

            if (!isValid)
            {
                // Збираємо всі помилки в один текст з нових рядків
                errorMessage = string.Join("\n- ", results.Select(r => r.ErrorMessage));
                errorMessage = "Помилки валідації:\n- " + errorMessage;
                return false;
            }

            // Якщо email не пустий, але введений просто пробіл
            if (!string.IsNullOrEmpty(contact.Email) && string.IsNullOrWhiteSpace(contact.Email))
            {
                errorMessage = "Email не може складатися лише з пробілів.";
                return false;
            }

            errorMessage = string.Empty;
            return true;
        }

        private void ButtonAdd_Click(object sender, EventArgs e)
        {
            // Створюємо тестовий об'єкт для перевірки
            Contact tempContact = new Contact(textBoxName.Text.Trim(), textBoxPhone.Text.Trim(), textBoxEmail.Text.Trim(), (ContactGroup)comboBoxGroup.SelectedItem);

            // Перевіряємо атрибути валідації
            if (!ValidateContact(tempContact, out string errorMsg))
            {
                MessageBox.Show(errorMsg, "Помилка введення", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // Намагаємося додати (тут може спрацювати бізнес-правило на унікальність)
                _repository.Add(tempContact);
                UpdateListBox();
                ClearInputFields();
            }
            catch (InvalidOperationException ex)
            {
                // 4. Реалізація повідомлення про помилку бізнес-логіки
                MessageBox.Show(ex.Message, "Порушення бізнес-правила", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ButtonEdit_Click(object sender, EventArgs e)
        {
            if (listBoxContacts.SelectedItem is Contact selectedContact)
            {
                // Створюємо копію для перевірки
                Contact tempContact = new Contact(textBoxName.Text.Trim(), textBoxPhone.Text.Trim(), textBoxEmail.Text.Trim(), (ContactGroup)comboBoxGroup.SelectedItem);

                if (!ValidateContact(tempContact, out string errorMsg))
                {
                    MessageBox.Show(errorMsg, "Помилка введення", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // --- ПЕРЕВІРКА БІЗНЕС-ПРАВИЛА ДЛЯ РЕДАГУВАННЯ ---
                // Якщо телефон змінили, перевіряємо, чи немає такого ж у базі в ІНШОГО контакту
                if (selectedContact.Phone != tempContact.Phone)
                {
                    string newPhoneClean = tempContact.Phone.Replace(" ", "");
                    bool isDuplicate = _repository.GetAll().Any(c => c.Phone.Replace(" ", "") == newPhoneClean);

                    if (isDuplicate)
                    {
                        MessageBox.Show($"Контакт з номером {tempContact.Phone} вже існує!", "Порушення бізнес-правила", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return; // Зупиняємо збереження
                    }
                }
                // ------------------------------------------------

                // Якщо все добре, оновлюємо оригінальний об'єкт
                selectedContact.Name = tempContact.Name;
                selectedContact.Phone = tempContact.Phone;
                selectedContact.Email = tempContact.Email;
                selectedContact.Group = tempContact.Group;

                _repository.Update(selectedContact);
                UpdateListBox();
                ClearInputFields();
            }
        }

        // --- Інші методи без змін ---
        private void TextBoxSearch_TextChanged(object sender, EventArgs e)
        {
            string query = textBoxSearch.Text.ToLower();
            UpdateListBox(string.IsNullOrWhiteSpace(query) ? null : _repository.Find(c => c.Name.ToLower().Contains(query) || c.Phone.Contains(query)));
        }

        private void ButtonDelete_Click(object sender, EventArgs e)
        {
            if (listBoxContacts.SelectedItem is Contact selectedContact)
            {
                _repository.Remove(selectedContact);
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
            else buttonEdit.Enabled = false;
        }

        private void UpdateListBox(IEnumerable<Contact> contactsToDisplay = null)
        {
            listBoxContacts.Items.Clear();
            var items = contactsToDisplay ?? _repository.GetAll();
            foreach (var contact in items) listBoxContacts.Items.Add(contact);
        }

        private void ClearInputFields()
        {
            textBoxName.Clear(); textBoxPhone.Clear(); textBoxEmail.Clear();
            comboBoxGroup.SelectedIndex = 0; listBoxContacts.ClearSelected(); buttonEdit.Enabled = false;
        }
    }
}