using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace zadanko.Models
{
    public class Book : INotifyPropertyChanged
    {
        private string title = string.Empty;
        private string author = string.Empty;
        private string genre = string.Empty;

        public int Id { get; set; }

        public string Title
        {
            get => title;
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
            get => author;
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
            get => genre;
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

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
