using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Maui.Storage;

namespace wordleGame
{
    public class FileService
    {
        // URL of the words file
        private const string FileUrl = "https://raw.githubusercontent.com/DonH-ITS/jsonfiles/main/words.txt";
        
        // Local file name
        private const string LocalFileName = "words.txt";

        // Method to get the words file (downloads if not found locally)
        public async Task<string> GetWordsFileAsync()
        {
            // Get the local file path using the FileSystem API
            var localFilePath = Path.Combine(FileSystem.AppDataDirectory, LocalFileName);

            // If the file already exists locally, return the local path
            if (File.Exists(localFilePath))
            {
                return localFilePath;
            }

            // Otherwise, download the file
            await DownloadFileAsync(localFilePath);

            // Return the path of the downloaded file
            return localFilePath;
        }

        // Method to download the file from the URL and save it locally
        private async Task DownloadFileAsync(string localFilePath)
        {
            try
            {
                // Create an HttpClient to fetch the file
                using (var httpClient = new HttpClient())
                {
                    // Download the file content
                    var fileContent = await httpClient.GetStringAsync(FileUrl);
                    Console.WriteLine($"File downloaded");

                    // Write the content to a local file
                    await File.WriteAllTextAsync(localFilePath, fileContent);
                }
            }
            catch (Exception ex)
            {
                // Handle errors (e.g., network issues)
                Console.WriteLine($"Error downloading file: {ex.Message}");
            }
        }

        // Method to read words from the downloaded file (returns an array of words)
        public async Task<string[]> ReadWordsAsync()
        {
            var localFilePath = await GetWordsFileAsync();

            // Read the content of the local file
            var fileContent = await File.ReadAllTextAsync(localFilePath);

            // Split the content into words and return as an array
            return fileContent.Split(new[] { '\n', '\r', ',' }, StringSplitOptions.RemoveEmptyEntries);
        }
    }
}
