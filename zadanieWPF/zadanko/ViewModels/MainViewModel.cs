using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Windows;
using System.Windows.Input;
using zadanko.Models;
using zadanko.Views;

namespace zadanko.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly string storagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "books.json");
        private string selectedGenre = "Fantastyka";
        private string searchText = string.Empty;
        private string sortBy = "Tytuł";
        private Book? selectedBook;

        public ObservableCollection<Book> AllBooks { get; } = new();
        public ObservableCollection<Book> FilteredBooks { get; } = new();
        public ObservableCollection<string> Genres { get; } = new() { "Fantastyka", "Akcja", "Przygodowe" };
        public ObservableCollection<string> SortOptions { get; } = new() { "Tytuł", "Autor", "Data dodania" };

        public string SelectedGenre
        {
            get => selectedGenre;
            set
            {
                if (selectedGenre != value)
                {
                    selectedGenre = value;
                    OnPropertyChanged();
                    RefreshBooks();
                }
            }
        }

        public string SearchText
        {
            get => searchText;
            set
            {
                if (searchText != value)
                {
                    searchText = value;
                    OnPropertyChanged();
                    RefreshBooks();
                }
            }
        }

        public string SortBy
        {
            get => sortBy;
            set
            {
                if (sortBy != value)
                {
                    sortBy = value;
                    OnPropertyChanged();
                    RefreshBooks();
                }
            }
        }

        public Book? SelectedBook
        {
            get => selectedBook;
            set
            {
                selectedBook = value;
                OnPropertyChanged();
                ((RelayCommand)EditCommand).RaiseCanExecuteChanged();
                ((RelayCommand)DeleteCommand).RaiseCanExecuteChanged();
            }
        }

        public ICommand AddCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand DeleteCommand { get; }

        public MainViewModel()
        {
            AddCommand = new RelayCommand(_ => AddBook());
            EditCommand = new RelayCommand(_ => EditBook(), _ => SelectedBook is not null);
            DeleteCommand = new RelayCommand(_ => DeleteBook(), _ => SelectedBook is not null);

            LoadBooks();
        }

        public void LoadBooks()
        {
            try
            {
                AllBooks.Clear();

                if (File.Exists(storagePath))
                {
                    var json = File.ReadAllText(storagePath);
                    if (!string.IsNullOrWhiteSpace(json))
                    {
                        var list = JsonSerializer.Deserialize<List<Book>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                        if (list is not null)
                        {
                            foreach (var book in list)
                            {
                                AllBooks.Add(book);
                            }
                        }
                    }
                }

                if (AllBooks.Count == 0)
                {
                    SeedData();
                }

                SaveBooks();
                RefreshBooks();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd podczas wczytywania danych: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void RefreshBooks()
        {
            var filtered = AllBooks
                .Where(book => string.Equals(book.Genre, SelectedGenre, StringComparison.OrdinalIgnoreCase))
                .Where(book => string.IsNullOrWhiteSpace(SearchText)
                    || book.Title.Contains(SearchText, StringComparison.OrdinalIgnoreCase)
                    || book.Author.Contains(SearchText, StringComparison.OrdinalIgnoreCase))
                .ToList();

            filtered = SortBooks(filtered, SortBy);
            FilteredBooks.Clear();
            foreach (var item in filtered)
            {
                FilteredBooks.Add(item);
            }
        }

        private static List<Book> SortBooks(IEnumerable<Book> books, string sortBy)
        {
            return sortBy switch
            {
                "Autor" => books.OrderBy(b => b.Author).ThenBy(b => b.Title).ToList(),
                "Data dodania" => books.OrderByDescending(b => b.AddedAt).ToList(),
                _ => books.OrderBy(b => b.Title).ToList(),
            };
        }

        private void SeedData()
        {
            var seed = new[]
            {
                new Book { Id = 1, Title = "Wiedźmin", Author = "Andrzej Sapkowski", Genre = "Fantastyka", AddedAt = DateTime.Now.AddDays(-10) },
                new Book { Id = 2, Title = "Władca Pierścieni", Author = "J.R.R. Tolkien", Genre = "Fantastyka", AddedAt = DateTime.Now.AddDays(-8) },
                new Book { Id = 3, Title = "Dziady", Author = "Adam Mickiewicz", Genre = "Fantastyka", AddedAt = DateTime.Now.AddDays(-6) },
                new Book { Id = 4, Title = "Mroczne Widmo", Author = "M. Kwiatkowski", Genre = "Akcja", AddedAt = DateTime.Now.AddDays(-9) },
                new Book { Id = 5, Title = "Niewidzialny", Author = "T. M. Olszewski", Genre = "Akcja", AddedAt = DateTime.Now.AddDays(-7) },
                new Book { Id = 6, Title = "Skarb na północy", Author = "A. Piotrowski", Genre = "Przygodowe", AddedAt = DateTime.Now.AddDays(-5) },
                new Book { Id = 7, Title = "Tajemniczy Ogród", Author = "Frances Hodgson Burnett", Genre = "Przygodowe", AddedAt = DateTime.Now.AddDays(-3) }
            };

            foreach (var item in seed)
            {
                AllBooks.Add(item);
            }
        }

        private void SaveBooks()
        {
            var dir = Path.GetDirectoryName(storagePath);
            if (!string.IsNullOrEmpty(dir))
            {
                Directory.CreateDirectory(dir);
            }

            var json = JsonSerializer.Serialize(AllBooks.ToList(), new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(storagePath, json);
        }

        private void AddBook()
        {
            try
            {
                var newBook = new Book { Genre = SelectedGenre, AddedAt = DateTime.Now };
                var dialog = new BookDialog(newBook);
                if (dialog.ShowDialog() == true)
                {
                    var validationErrors = ValidateBook(newBook);
                    if (validationErrors.Any())
                    {
                        MessageBox.Show(string.Join(Environment.NewLine, validationErrors), "Błąd walidacji", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    newBook.Id = AllBooks.Count == 0 ? 1 : AllBooks.Max(b => b.Id) + 1;
                    AllBooks.Add(newBook);
                    SaveBooks();
                    RefreshBooks();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd podczas dodawania książki: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void EditBook()
        {
            if (SelectedBook is null)
            {
                return;
            }

            try
            {
                var clone = new Book
                {
                    Id = SelectedBook.Id,
                    Title = SelectedBook.Title,
                    Author = SelectedBook.Author,
                    Genre = SelectedBook.Genre,
                    AddedAt = SelectedBook.AddedAt
                };

                var dialog = new BookDialog(clone);
                if (dialog.ShowDialog() == true)
                {
                    var validationErrors = ValidateBook(clone);
                    if (validationErrors.Any())
                    {
                        MessageBox.Show(string.Join(Environment.NewLine, validationErrors), "Błąd walidacji", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    SelectedBook.Title = clone.Title;
                    SelectedBook.Author = clone.Author;
                    SelectedBook.Genre = clone.Genre;
                    SaveBooks();
                    RefreshBooks();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd podczas edycji książki: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DeleteBook()
        {
            if (SelectedBook is null)
            {
                return;
            }

            try
            {
                var result = MessageBox.Show($"Czy na pewno chcesz usunąć książkę \"{SelectedBook.Title}\"?", "Potwierdzenie", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    AllBooks.Remove(SelectedBook);
                    SaveBooks();
                    SelectedBook = null;
                    RefreshBooks();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd podczas usuwania książki: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private static List<string> ValidateBook(Book book)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(book.Title))
            {
                errors.Add("Tytuł nie może być pusty.");
            }

            if (string.IsNullOrWhiteSpace(book.Author))
            {
                errors.Add("Autor nie może być pusty.");
            }

            if (string.IsNullOrWhiteSpace(book.Genre))
            {
                errors.Add("Kategoria nie może być pusta.");
            }

            if (book.Title.Length < 2)
            {
                errors.Add("Tytuł musi mieć co najmniej 2 znaki.");
            }

            return errors;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class RelayCommand : ICommand
    {
        private readonly Action<object?> execute;
        private readonly Predicate<object?>? canExecute;

        public RelayCommand(Action<object?> execute, Predicate<object?>? canExecute = null)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter) => canExecute == null || canExecute(parameter);

        public void Execute(object? parameter) => execute(parameter);

        public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}
