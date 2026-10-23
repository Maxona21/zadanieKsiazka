using System.Windows;
using System.Windows.Controls;
using zadanko.Models;

namespace zadanko.Views
{
    public partial class BookDialog : Window
    {
        private readonly Book book;

        public BookDialog(Book book)
        {
            InitializeComponent();
            this.book = book;
            DataContext = book;

            var genres = new[] { "Fantastyka", "Akcja", "Przygodowe" };
            GenreComboBox.ItemsSource = genres;

            if (string.IsNullOrWhiteSpace(book.Genre))
            {
                book.Genre = genres[0];
            }

            GenreComboBox.SelectedItem = book.Genre;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(book.Title) || string.IsNullOrWhiteSpace(book.Author))
            {
                MessageBox.Show("Tytuł i autor nie mogą być puste.", "Błąd walidacji", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (GenreComboBox.SelectedItem is string selectedGenre)
            {
                book.Genre = selectedGenre;
            }

            DialogResult = true;
            Close();
        }
    }
}
