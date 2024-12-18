using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;

namespace wordleGame
{
    public class WordleViewModel : INotifyPropertyChanged
    {
        private string chosenWord;
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
                    OnPropertyChanged();
                }
            }
        }

    }
}