using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;

namespace wordleGame
{
    public class PlayerAttempt
    {
        private bool IsWordGuessed { get; set; }
        private string CorrectWord { get; set; }
        private int NumOfGuesses { get; set; }
        private DateTime Timestamp { get; set; }

        public PlayerAttempt(bool isWordGuessed, string correctWord, int numOfGuesses)
        {
            IsWordGuessed = isWordGuessed;
            CorrectWord = correctWord;
            NumOfGuesses = numOfGuesses;
            Timestamp = DateTime.Now;
        }

    }
}