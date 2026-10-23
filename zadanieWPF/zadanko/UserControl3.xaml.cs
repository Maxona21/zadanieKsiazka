using System;
using System.Collections.Generic;
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

using System.Collections.ObjectModel;
using System.ComponentModel;

namespace zadanko
{
    public partial class UserControl3 : UserControl
    {
        private ObservableCollection<Book> Books => DataContext as ObservableCollection<Book> ?? throw new InvalidOperationException("Brak zbioru książek dla tej kategorii.");
        private ICollectionView View => CollectionViewSource.GetDefaultView(Books);

        public UserControl3()
        {
            InitializeComponent();
            DataContext = MainWindow.AdventureBooks;
            View.Filter = FilterBook;
            SearchTextBox.Text = string.Empty;
        }

        private bool FilterBook(object item)
        {
            if (item is not Book book)
                return false;

            var query = SearchTextBox?.Text ?? string.Empty;
            if (string.IsNullOrWhiteSpace(query))
                return true;

            return $"{book.Title} {book.Author}".Contains(query, StringComparison.OrdinalIgnoreCase);
        }

        private void ApplyFilter()
        {
            View.Refresh();
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyFilter();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            BookCollectionEditor.AddBook(Books);
            ApplyFilter();
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            BookCollectionEditor.EditSelectedBook(Books, BooksList.SelectedItem);
            ApplyFilter();
        }

        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            BookCollectionEditor.RemoveSelectedBook(Books, BooksList.SelectedItem);
            ApplyFilter();
        }

        private void BackButton3_Click(object sender, RoutedEventArgs e)
        {
            var main = Application.Current.MainWindow as MainWindow;
            if (main != null)
                main.CC.Content = null;
        }
    }
}
