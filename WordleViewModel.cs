using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;

namespace wordleGame
{
    public class WordleViewModel : INotifyPropertyChanged
    {
        private string chosenWord;
        private string chosenWord2;
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

        public string ChosenWord2
        {
            get => chosenWord2;

            set
            {
                if (chosenWord2 != value)
                {
                    chosenWord2 = value;
                    OnPropertyChanged(nameof(ChosenWord2));
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