using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;

namespace wordleGame
{
    public class WordleViewModel : INotifyPropertyChanged
    {
        private string chosenWord;
        private char[] guess = new char[5];
        private int attempts;

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

        public char[] Guess
        {
            get => guess;

            set
            {
                if (!guess.SequenceEqual(value))
                {
                    guess = value;
                    OnPropertyChanged(nameof(Guess));
                }
            }
        }

        public int Attempts
        {
            get => attempts;

            set
            {
                if (attempts != value)
                {
                    attempts = value;
                    OnPropertyChanged(nameof(Attempts));
                }
            }
        }

        public string[] CheckGuess() 
        {
            string[] feedback = new string[5];
            HashSet<char> chosenWordChars = new HashSet<char>(chosenWord);  // Store letters for quick lookup
             
            for(int i = 0; i < 5; i++) 
            {
                if (guess[i] == chosenWord[i])
                {
                    feedback[i] = "Correct";
                }
                else if (chosenWord.Contains(guess[i]))
                {
                    feedback[i] = "WrongPlace";
                }
                else 
                {
                    feedback[i] = "Incorrect";
                }
            }
            return feedback;
        }

        public void IncrementAttempts()
        {
            Attempts++;  // Increment attempts by 1
        }
    }
}