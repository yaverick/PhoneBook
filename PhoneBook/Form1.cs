using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations; // Необхідно для роботи атрибутів валідації
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace PhoneBook
{
    // Клас головної форми застосунку, який успадковує стандартний Form з Windows Forms
    public partial class Form1 : Form
    {
        // Поле для зберігання посилання на репозиторій, який керує даними
        private IRepository<Contact> _repository;

        // Конструктор форми, який викликається при її створенні
        public Form1()
        {
            // Ініціалізація репозиторію
            _repository = new ContactRepository();

            // Налаштування зовнішнього вигляду вікна та його елементів
            InitializeUI();

            // Підключення обробників подій (натискання кнопок, введення тексту)
            SetupEventHandlers();

            // Завантаження збережених контактів у список на екрані при старті
            UpdateListBox();
        }

        // Оголошення елементів інтерфейсу: списку, полів вводу, кнопок та надписів
        private ListBox listBoxContacts;
        private Label labelSearch;
        private TextBox textBoxSearch;
        private Label labelName, labelPhone, labelEmail, labelGroup;
        private TextBox textBoxName, textBoxPhone, textBoxEmail;
        private ComboBox comboBoxGroup;
        private Button buttonAdd, buttonEdit, buttonDelete, buttonClear;

        // Метод, який створює та розміщує всі елементи на формі вручну
        private void InitializeUI()
        {
            // Базові налаштування самого вікна програми
            this.Text = "Телефонна книга";
            this.Size = new Size(620, 520);
            this.StartPosition = FormStartPosition.CenterScreen; // Вікно з'являється по центру екрана
            this.MinimumSize = new Size(620, 520);

            // Створення елементів для поля пошуку
            labelSearch = new Label { Text = "Пошук:", Location = new Point(12, 16), AutoSize = true, Font = new Font("Segoe UI", 9) };
            textBoxSearch = new TextBox { Location = new Point(65, 13), Size = new Size(327, 25), Font = new Font("Segoe UI", 10) };

            // Створення списку для відображення контактів
            listBoxContacts = new ListBox { Location = new Point(12, 45), Size = new Size(380, 415), Font = new Font("Segoe UI", 10), Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Bottom };

            // Створення надписів для полів вводу
            labelName = new Label { Text = "Ім'я:", Location = new Point(410, 20), AutoSize = true, Font = new Font("Segoe UI", 9) };
            labelPhone = new Label { Text = "Телефон:", Location = new Point(410, 70), AutoSize = true, Font = new Font("Segoe UI", 9) };
            labelEmail = new Label { Text = "Email:", Location = new Point(410, 120), AutoSize = true, Font = new Font("Segoe UI", 9) };
            labelGroup = new Label { Text = "Група:", Location = new Point(410, 170), AutoSize = true, Font = new Font("Segoe UI", 9) };

            // Створення текстових полів для вводу даних контакту
            textBoxName = new TextBox { Location = new Point(410, 40), Size = new Size(180, 25), Font = new Font("Segoe UI", 10) };
            textBoxPhone = new TextBox { Location = new Point(410, 90), Size = new Size(180, 25), Font = new Font("Segoe UI", 10) };
            textBoxEmail = new TextBox { Location = new Point(410, 140), Size = new Size(180, 25), Font = new Font("Segoe UI", 10) };

            // Створення випадаючого списку (ComboBox) для вибору групи
            comboBoxGroup = new ComboBox { Location = new Point(410, 190), Size = new Size(180, 25), Font = new Font("Segoe UI", 10), DropDownStyle = ComboBoxStyle.DropDownList };
            // Прив'язка значень перерахування ContactGroup до ComboBox
            comboBoxGroup.DataSource = Enum.GetValues(typeof(ContactGroup));

            // Створення кнопок управління
            buttonAdd = new Button { Text = "Додати", Location = new Point(410, 235), Size = new Size(180, 35), Font = new Font("Segoe UI", 10) };
            buttonEdit = new Button { Text = "Редагувати", Location = new Point(410, 280), Size = new Size(180, 35), Font = new Font("Segoe UI", 10), Enabled = false }; // Вимкнена за замовчуванням
            buttonDelete = new Button { Text = "Видалити", Location = new Point(410, 325), Size = new Size(180, 35), Font = new Font("Segoe UI", 10) };
            buttonClear = new Button { Text = "Очистити поля", Location = new Point(410, 380), Size = new Size(180, 35), Font = new Font("Segoe UI", 10) };

            // Додавання всіх створених елементів на площину форми
            this.Controls.AddRange(new Control[] { labelSearch, textBoxSearch, listBoxContacts, labelName, textBoxName, labelPhone, textBoxPhone, labelEmail, textBoxEmail, labelGroup, comboBoxGroup, buttonAdd, buttonEdit, buttonDelete, buttonClear });
        }

        // Прив'язка методів-обробників до відповідних подій елементів інтерфейсу
        private void SetupEventHandlers()
        {
            buttonAdd.Click += ButtonAdd_Click;
            buttonEdit.Click += ButtonEdit_Click;
            buttonDelete.Click += ButtonDelete_Click;
            buttonClear.Click += ButtonClear_Click;

            // Подія, що спрацьовує при виборі іншого контакту в списку
            listBoxContacts.SelectedIndexChanged += ListBoxContacts_SelectedIndexChanged;

            // Подія, що спрацьовує при кожній зміні тексту в полі пошуку
            textBoxSearch.TextChanged += TextBoxSearch_TextChanged;
        }

        // Універсальний метод для перевірки правильності введених даних (валідації)
        private bool ValidateContact(Contact contact, out string errorMessage)
        {
            // Створення контексту для перевірки об'єкта
            var context = new ValidationContext(contact, serviceProvider: null, items: null);
            var results = new List<ValidationResult>();

            // Метод TryValidateObject автоматично перевіряє атрибути [Required], [RegularExpression] тощо
            bool isValid = Validator.TryValidateObject(contact, context, results, true);

            // Якщо є помилки, формуємо єдиний текст повідомлення
            if (!isValid)
            {
                errorMessage = string.Join("\n- ", results.Select(r => r.ErrorMessage));
                errorMessage = "Помилки валідації:\n- " + errorMessage;
                return false;
            }

            // Додаткова перевірка: щоб Email не складався лише з пробілів, якщо він не пустий
            if (!string.IsNullOrEmpty(contact.Email) && string.IsNullOrWhiteSpace(contact.Email))
            {
                errorMessage = "Email не може складатися лише з пробілів.";
                return false;
            }

            errorMessage = string.Empty;
            return true;
        }

        // Обробник події натискання на кнопку "Додати"
        private void ButtonAdd_Click(object sender, EventArgs e)
        {
            // Створюємо тимчасовий об'єкт із даних, введених у текстові поля
            Contact tempContact = new Contact(textBoxName.Text.Trim(), textBoxPhone.Text.Trim(), textBoxEmail.Text.Trim(), (ContactGroup)comboBoxGroup.SelectedItem);

            // Якщо дані не пройшли валідацію, показуємо попередження і припиняємо виконання
            if (!ValidateContact(tempContact, out string errorMsg))
            {
                MessageBox.Show(errorMsg, "Помилка введення", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // Намагаємося додати контакт у репозиторій
                _repository.Add(tempContact);

                // Якщо додавання успішне, оновлюємо список і очищаємо поля
                UpdateListBox();
                ClearInputFields();
            }
            catch (InvalidOperationException ex)
            {
                // Якщо порушено бізнес-правило (наприклад, дублікат номера), показуємо помилку
                MessageBox.Show(ex.Message, "Порушення бізнес-правила", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Обробник події натискання на кнопку "Редагувати"
        private void ButtonEdit_Click(object sender, EventArgs e)
        {
            // Перевіряємо, чи дійсно вибрано якийсь контакт у списку
            if (listBoxContacts.SelectedItem is Contact selectedContact)
            {
                // Створюємо тимчасовий об'єкт для валідації нових даних
                Contact tempContact = new Contact(textBoxName.Text.Trim(), textBoxPhone.Text.Trim(), textBoxEmail.Text.Trim(), (ContactGroup)comboBoxGroup.SelectedItem);

                // Перевіряємо формат введених даних
                if (!ValidateContact(tempContact, out string errorMsg))
                {
                    MessageBox.Show(errorMsg, "Помилка введення", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Перевірка бізнес-правила: якщо номер телефону змінили, переконуємося, що він не зайнятий
                if (selectedContact.Phone != tempContact.Phone)
                {
                    string newPhoneClean = tempContact.Phone.Replace(" ", "");
                    bool isDuplicate = _repository.GetAll().Any(c => c.Phone.Replace(" ", "") == newPhoneClean);

                    if (isDuplicate)
                    {
                        MessageBox.Show($"Контакт з номером {tempContact.Phone} вже існує!", "Порушення бізнес-правила", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return; // Зупиняємо редагування, якщо номер зайнятий
                    }
                }

                // Застосовуємо нові дані до вибраного об'єкта
                selectedContact.Name = tempContact.Name;
                selectedContact.Phone = tempContact.Phone;
                selectedContact.Email = tempContact.Email;
                selectedContact.Group = tempContact.Group;

                // Зберігаємо зміни та оновлюємо інтерфейс
                _repository.Update(selectedContact);
                UpdateListBox();
                ClearInputFields();
            }
        }

        // Обробник події зміни тексту в полі пошуку
        private void TextBoxSearch_TextChanged(object sender, EventArgs e)
        {
            string query = textBoxSearch.Text.ToLower();

            // Якщо поле пусте - показуємо всіх, інакше - фільтруємо за іменем або номером
            UpdateListBox(string.IsNullOrWhiteSpace(query) ? null : _repository.Find(c => c.Name.ToLower().Contains(query) || c.Phone.Contains(query)));
        }

        // Обробник події натискання на кнопку "Видалити"
        private void ButtonDelete_Click(object sender, EventArgs e)
        {
            // Якщо контакт вибрано, видаляємо його з репозиторію
            if (listBoxContacts.SelectedItem is Contact selectedContact)
            {
                _repository.Remove(selectedContact);
                UpdateListBox();
                ClearInputFields();
            }
        }

        // Обробник події натискання на кнопку "Очистити поля"
        private void ButtonClear_Click(object sender, EventArgs e) => ClearInputFields();

        // Обробник події вибору іншого елемента у списку контактів
        private void ListBoxContacts_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Якщо вибрано контакт, заповнюємо його даними текстові поля для редагування
            if (listBoxContacts.SelectedItem is Contact selectedContact)
            {
                textBoxName.Text = selectedContact.Name;
                textBoxPhone.Text = selectedContact.Phone;
                textBoxEmail.Text = selectedContact.Email;
                comboBoxGroup.SelectedItem = selectedContact.Group;
                buttonEdit.Enabled = true; // Робимо кнопку "Редагувати" активною
            }
            else
            {
                // Якщо нічого не вибрано, вимикаємо кнопку редагування
                buttonEdit.Enabled = false;
            }
        }

        // Метод для оновлення відображення списку контактів
        private void UpdateListBox(IEnumerable<Contact> contactsToDisplay = null)
        {
            listBoxContacts.Items.Clear();

            // Використовуємо переданий список (наприклад, після пошуку) або беремо всі контакти
            var items = contactsToDisplay ?? _repository.GetAll();

            // Додаємо контакти у ListBox
            foreach (var contact in items)
            {
                listBoxContacts.Items.Add(contact);
            }
        }

        // Допоміжний метод для очищення всіх полів вводу
        private void ClearInputFields()
        {
            textBoxName.Clear();
            textBoxPhone.Clear();
            textBoxEmail.Clear();
            comboBoxGroup.SelectedIndex = 0; // Повертаємо випадаючий список на перше значення
            listBoxContacts.ClearSelected(); // Знімаємо виділення зі списку
            buttonEdit.Enabled = false; // Вимикаємо кнопку редагування
        }
    }
}