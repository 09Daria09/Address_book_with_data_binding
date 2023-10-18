using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Address_book_with_data_binding
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            ContactManager contactManager = Resources["contactManager"] as ContactManager;

            contactManager?.Persons.Add(new Person
            {
                Name = personName.Text,
                Address = personAddress.Text,
                Phone = personPhone.Text
            });
            personName.Text = "";
            personAddress.Text = "";
            personPhone.Text = "";
        }
        #region  Я разорвала здесь связь, так как эта привязка противоречит моей логике. Мне не нравилось, когда изменения происходили автоматически. Я хотела, чтобы изменения применились только после нажатия на кнопку "Изменить".

        private Person _tempPerson; 

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ContactManager contactManager = Resources["contactManager"] as ContactManager;

            Person selectedPerson = contactManager.SelectedPerson;
            if (selectedPerson == null)
            {
                return;
            }

            _tempPerson = new Person
            {
                Name = selectedPerson.Name,
                Address = selectedPerson.Address,
                Phone = selectedPerson.Phone
            };

            personName.Text = _tempPerson.Name;
            personAddress.Text = _tempPerson.Address;
            personPhone.Text = _tempPerson.Phone;
        }

        private void ModifyButton_Click(object sender, RoutedEventArgs e)
        {

            ContactManager contactManager = Resources["contactManager"] as ContactManager;

            Person selectedPerson = contactManager.SelectedPerson;
            if (selectedPerson == null)
            {
                MessageBox.Show("Пожалуйста, выберите контакт для изменения.");
                return;
            }

            selectedPerson.Name = personName.Text;
            selectedPerson.Address = personAddress.Text;
            selectedPerson.Phone = personPhone.Text;

            personName.Text = "";
            personAddress.Text = "";
            personPhone.Text = "";

            _tempPerson = null; 
        }

        #endregion

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            ContactManager contactManager = Resources["contactManager"] as ContactManager;

            if (contactManager?.SelectedPerson == null)
            {
                MessageBox.Show("Пожалуйста, выберите контакт для удаления.");
                return;
            }

            MessageBoxResult result = MessageBox.Show("Вы уверены, что хотите удалить этот контакт?", "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                contactManager.Persons.Remove(contactManager.SelectedPerson);
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            ContactManager contactManager = Resources["contactManager"] as ContactManager;

            var json = JsonConvert.SerializeObject(contactManager.Persons);

            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*",
                DefaultExt = "json",
                FileName = "contacts.json"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                File.WriteAllText(saveFileDialog.FileName, json);
            }
        }

        private void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            ContactManager contactManager = Resources["contactManager"] as ContactManager;

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "JSON файлы (*.json)|*.json|Все файлы (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                string jsonText = File.ReadAllText(openFileDialog.FileName);
                try
                {
                    ObservableCollection<Person> loadedPersons = JsonConvert.DeserializeObject<ObservableCollection<Person>>(jsonText);

                    if (loadedPersons != null)
                    {
                        contactManager.Persons.Clear();

                        foreach (var person in loadedPersons)
                        {
                            contactManager.Persons.Add(person);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Ошибка при загрузке контактов.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Произошла ошибка: {ex.Message}");
                }
            }

        }
    }

    }

