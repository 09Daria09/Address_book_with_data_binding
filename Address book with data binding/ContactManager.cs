using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Address_book_with_data_binding
{
    public class Person : INotifyPropertyChanged
    {
        private string _name;
        private string _address;
        private string _phone;

        public string Name
        {
            get => _name;
            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }

        public string Address
        {
            get => _address;
            set
            {
                if (_address != value)
                {
                    _address = value;
                    OnPropertyChanged(nameof(Address));
                }
            }
        }

        public string Phone
        {
            get => _phone;
            set
            {
                if (_phone != value)
                {
                    _phone = value;
                    OnPropertyChanged(nameof(Phone));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class ContactManager : INotifyPropertyChanged
    {
        private ObservableCollection<Person> _persons = new ObservableCollection<Person>();
        private Person _selectedPerson = new Person();

        public ObservableCollection<Person> Persons => _persons;

        public Person SelectedPerson
        {
            get => _selectedPerson;
            set
            {
                if (_selectedPerson != value)
                {
                    _selectedPerson = value;
                    OnPropertyChanged(nameof(SelectedPerson));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
