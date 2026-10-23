using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.VisualBasic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace zadanko
{

    public class Book : INotifyPropertyChanged
    {
        private string title = string.Empty;
        private string author = string.Empty;
        private string genre = string.Empty;

        public Book()
        {
        }

        public Book(string title, string author, string genre = "")
        {
            this.title = title;
            this.author = author;
            this.genre = genre;
        }

        public int Id { get; set; }

        public string Title
        {
            get { return title; }
            set
            {
                if (title != value)
                {
                    title = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Author
        {
            get { return author; }
            set
            {
                if (author != value)
                {
                    author = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Genre
        {
            get { return genre; }
            set
            {
                if (genre != value)
                {
                    genre = value;
                    OnPropertyChanged();
                }
            }
        }

        public DateTime AddedAt { get; set; } = DateTime.Now;

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    internal static class BookCollectionEditor
    {
        public static void AddBook(ObservableCollection<Book> books)
        {
            var title = Interaction.InputBox("Podaj tytuł książki:", "Dodaj książkę", string.Empty);
            if (string.IsNullOrWhiteSpace(title))
                return;

            var author = Interaction.InputBox("Podaj autora:", "Dodaj książkę", string.Empty);
            if (string.IsNullOrWhiteSpace(author))
                return;

            books.Add(new Book(title.Trim(), author.Trim()));
        }

        public static void EditSelectedBook(ObservableCollection<Book> books, object? selectedItem)
        {
            if (selectedItem is not Book selectedBook)
            {
                MessageBox.Show("Wybierz książkę z listy, aby ją edytować.", "Brak wyboru", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var title = Interaction.InputBox("Edytuj tytuł książki:", "Edytuj książkę", selectedBook.Title);
            if (string.IsNullOrWhiteSpace(title))
                return;

            var author = Interaction.InputBox("Edytuj autora:", "Edytuj książkę", selectedBook.Author);
            if (string.IsNullOrWhiteSpace(author))
                return;

            selectedBook.Title = title.Trim();
            selectedBook.Author = author.Trim();
        }

        public static void RemoveSelectedBook(ObservableCollection<Book> books, object? selectedItem)
        {
            if (selectedItem is not Book selectedBook)
            {
                MessageBox.Show("Wybierz książkę z listy, aby ją usunąć.", "Brak wyboru", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var result = MessageBox.Show($"Czy na pewno chcesz usunąć książkę \"{selectedBook.Title}\"?", "Potwierdzenie usunięcia", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result != MessageBoxResult.Yes)
                return;

            books.Remove(selectedBook);
        }
    }

    public partial class MainWindow : Window
    {
        public static ObservableCollection<Book> FantasyBooks { get; } = new()
        {
            new Book("Wiedźmin", "A. Sapkowski"),
            new Book("Władca Pierścieni", "J.R.R. Tolkien"),
            new Book("Dziady", "Adam Mickiewicz"),
            new Book("Harry Potter", "J.K. Rowling")
        };

        public static ObservableCollection<Book> ActionBooks { get; } = new()
        {
            new Book("Mroczne Widmo", "M. M. O. S."),
            new Book("Niewidzialny", "T. J. M."),
            new Book("Czarny Orzeł", "K. Wójcik"),
            new Book("Zejście", "M. Wróblewski")
        };

        public static ObservableCollection<Book> AdventureBooks { get; } = new()
        {
            new Book("Dolina", "L. Płatek"),
            new Book("Tajemniczy Ogród", "Frances Hodgson Burnett"),
            new Book("Odkrycie", "M. Fijałkowski"),
            new Book("Skarb na północy", "A. Piotrowski")
        };

        public MainWindow()
        {
            InitializeComponent();
        }

        private void but1_Click(object sender, RoutedEventArgs e)
        {
            CC.Content = new UserControl1();
        }

        private void but2_Click(object sender, RoutedEventArgs e)
        {
            CC.Content = new UserControl2();
        }

        private void but3_Click(object sender, RoutedEventArgs e)
        {
            CC.Content = new UserControl3();
        }
    }

}
