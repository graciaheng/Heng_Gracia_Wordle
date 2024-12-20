using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace wordleGame
{
    public class PlayerHistory
    {
        private const string FileName = "wordleGameHistory.json";
        public ObservableCollection<PlayerAttempt> Attempts { get; set; } = new ObservableCollection<PlayerAttempt>();

        // serialization
        public async Task SaveHistoryAsync()
        {
            try
            {
                string json = JsonSerializer.Serialize(this);
                string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), FileName);
                await File.WriteAllTextAsync(filePath, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving history: {ex.Message}"); 
            }
        }

        // deserialization
        public async Task LoadHistoryAsync()
        {
            try
            {
                string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), FileName);

                if (File.Exists(filePath))
                {
                    string json = await File.ReadAllTextAsync(filePath);
                    var history = JsonSerializer.Deserialize<PlayerHistory>(json);

                    // if the history exists, update the Attempts list
                    if (history != null)
                    {
                        Attempts.Clear();
                        foreach (var attempt in history.Attempts)
                        {
                            Attempts.Add(attempt);
                        }
                    }
                }
                else
                {
                    Console.WriteLine("History file not found.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading history: {ex.Message}");
            }
        }

        public void addNewAttempt(PlayerAttempt newAttempt) 
        {
            //add new attempts to the list
            Attempts.Add(newAttempt);
        }
    }
}
