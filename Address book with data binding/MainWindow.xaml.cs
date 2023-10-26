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
            CommandBindings.Add(new CommandBinding(DataCommands.Add, AddCommand, AddCommand_CanExecute));
            CommandBindings.Add(new CommandBinding(DataCommands.Modify, ModifyCommand, ModifyCommand_CanExecute));
            CommandBindings.Add(new CommandBinding(DataCommands.Delete, DeleteCommand, DeleteCommand_CanExecute));
            CommandBindings.Add(new CommandBinding(DataCommands.Save, SaveCommand, SaveCommand_CanExecute));
            CommandBindings.Add(new CommandBinding(DataCommands.Load, LoadCommand, LoadCommand_CanExecute));

            InitializeComponent();
        }
        private void AddCommand(object sender, ExecutedRoutedEventArgs e)
        {
            AddButton_Click(sender, e);
        }

        private void AddCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(personName.Text)
                           && !string.IsNullOrWhiteSpace(personAddress.Text)
                           && !string.IsNullOrWhiteSpace(personPhone.Text))
            {
                e.CanExecute = true;
                return;
            }
            e.CanExecute = false;
        }
        private bool _isChanged = false;

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            _isChanged = true;
            CommandManager.InvalidateRequerySuggested();
        }

        private void ModifyCommand(object sender, ExecutedRoutedEventArgs e)
        {
            ModifyButton_Click(sender, e);
            _isChanged = false; 
        }

        private void ModifyCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = _isChanged; 
        }
        private void DeleteCommand(object sender, ExecutedRoutedEventArgs e)
        {
            DeleteButton_Click(sender, e);
        }

        private void DeleteCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            ContactManager contactManager = Resources["contactManager"] as ContactManager;
            if (contactManager.Persons.Count > 0 && contactManager.SelectedPerson != null)
            {
                e.CanExecute = true;
                return;
            }
            e.CanExecute = false;
        }


        private void SaveCommand(object sender, ExecutedRoutedEventArgs e)
        {
            SaveButton_Click(sender, e);
        }

        private void SaveCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            ContactManager contactManager = Resources["contactManager"] as ContactManager;
            e.CanExecute = contactManager?.IsDataChanged ?? false;
        }


        private void LoadCommand(object sender, ExecutedRoutedEventArgs e)
        {
            LoadButton_Click(sender, e);
        }

        private void LoadCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
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

