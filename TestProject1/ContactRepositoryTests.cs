using Microsoft.VisualStudio.TestTools.UnitTesting;
using PhoneBook;
using System;
using System.IO;
using System.Linq;

namespace PhoneBook.Tests
{
    [TestClass]
    public class ContactRepositoryTests
    {
        // Спеціальний тестовий файл, щоб не чіпати реальні дані користувача
        private const string TestFilePath = "test_contacts.json";
        private ContactRepository _repository;

        // Цей метод виконується перед КОЖНИМ тестом для підготовки "чистого" середовища
        [TestInitialize]
        public void Setup()
        {
            // Видаляємо файл, якщо він залишився з минулого разу
            if (File.Exists(TestFilePath))
            {
                File.Delete(TestFilePath);
            }

            // Ініціалізуємо репозиторій з тестовим файлом
            _repository = new ContactRepository(TestFilePath);
        }

        // Цей метод виконується після кожного тесту, щоб прибрати за собою
        [TestCleanup]
        public void Cleanup()
        {
            if (File.Exists(TestFilePath))
            {
                File.Delete(TestFilePath);
            }
        }

        [TestMethod]
        public void Add_ValidContact_ShouldIncreaseCount()
        {
            // Arrange (Підготовка даних)
            var contact = new Contact("Іван", "+380501234567", "ivan@test.com", ContactGroup.Друзі);

            // Act (Дія, яку ми тестуємо)
            _repository.Add(contact);

            // Assert (Перевірка результату)
            var allContacts = _repository.GetAll().ToList();
            Assert.AreEqual(1, allContacts.Count);
            Assert.AreEqual("Іван", allContacts[0].Name);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Add_DuplicatePhone_ShouldThrowException()
        {
            // Arrange (Створюємо два контакти з однаковим номером)
            var contact1 = new Contact("Іван", "+380501234567", "", ContactGroup.Друзі);
            var contact2 = new Contact("Петро", "+380501234567", "", ContactGroup.Робота);

            // Act (Додаємо перший, а потім другий дублікат)
            _repository.Add(contact1);
            _repository.Add(contact2); // Тут метод має викинути помилку

            // Assert обробляється автоматично завдяки атрибуту [ExpectedException] над методом
        }

        [TestMethod]
        public void Remove_ExistingContact_ShouldDecreaseCount()
        {
            // Arrange (Підготовка бази з одним контактом)
            var contact = new Contact("Іван", "+380501234567", "", ContactGroup.Друзі);
            _repository.Add(contact);

            // Act (Видаляємо його)
            _repository.Remove(contact);

            // Assert (Перевіряємо, чи список знову порожній)
            Assert.AreEqual(0, _repository.GetAll().Count());
        }

        [TestMethod]
        public void Find_ByPartialName_ShouldReturnMatchingContacts()
        {
            // Arrange (Додаємо кілька тестових записів)
            _repository.Add(new Contact("Олександр", "+380501111111", "", ContactGroup.Друзі));
            _repository.Add(new Contact("Олексій", "+380502222222", "", ContactGroup.Друзі));
            _repository.Add(new Contact("Марія", "+380503333333", "", ContactGroup.Родина));

            // Act (Шукаємо частину слова "Олек")
            var results = _repository.Find(c => c.Name.Contains("Олек")).ToList();

            // Assert (Перевіряємо, чи знайшло саме двох людей)
            Assert.AreEqual(2, results.Count);
            Assert.IsTrue(results.Any(c => c.Name == "Олександр"));
            Assert.IsTrue(results.Any(c => c.Name == "Олексій"));
        }
    }
}