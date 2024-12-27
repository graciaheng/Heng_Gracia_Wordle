using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;

namespace wordleGame
{
    public class PlayerAttempt
    {
        private string CorrectWord { get; set; }
        private int NumOfGuesses { get; set; }
        private DateTime Timestamp { get; set; }

        public PlayerAttempt(string correctWord, int numOfGuesses)
        {
            CorrectWord = correctWord;
            NumOfGuesses = numOfGuesses;
            Timestamp = DateTime.Now;
        }

    }
}