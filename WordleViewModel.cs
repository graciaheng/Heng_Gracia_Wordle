using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;

namespace wordleGame
{
    public class WordleViewModel : INotifyPropertyChanged
    {
        private string chosenWord;
        private string guess;
        private int attempts;
        private bool isGameOver = false;
        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public string ChosenWord
        {
            get => chosenWord;

            set
            {
                if (chosenWord != value)
                {
                    chosenWord = value;
                    OnPropertyChanged(nameof(ChosenWord));
                }
            }
        }
        public PlayerHistory PlayerHistory { get; set; }
        
        public WordleViewModel()
        {
            PlayerHistory = new PlayerHistory();
        }
        

    }
}