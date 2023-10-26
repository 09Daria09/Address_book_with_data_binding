using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Address_book_with_data_binding
{
    public class DataCommands
    {
        private static RoutedUICommand requery;
        private static RoutedUICommand add; 
        private static RoutedUICommand modify;
        private static RoutedUICommand delete;
        private static RoutedUICommand save;
        private static RoutedUICommand load;

        static DataCommands()
        {
            requery = new RoutedUICommand("Requery", "Requery", typeof(DataCommands));
            add = new RoutedUICommand("Добавить", "Add", typeof(DataCommands));
            modify = new RoutedUICommand("Изменить", "Modify", typeof(DataCommands));
            delete = new RoutedUICommand("Удалить", "Delete", typeof(DataCommands));
            save = new RoutedUICommand("Сохранить", "Save", typeof(DataCommands));
            load = new RoutedUICommand("Загрузить", "Load", typeof(DataCommands));
        }

        public static RoutedUICommand Requery => requery;
        public static RoutedUICommand Add => add;
        public static RoutedUICommand Modify => modify;
        public static RoutedUICommand Delete => delete;
        public static RoutedUICommand Save => save;
        public static RoutedUICommand Load => load;
    }
}
