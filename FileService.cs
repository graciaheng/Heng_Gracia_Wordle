using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Maui.Storage;

namespace wordleGame
{
    public class FileService
    {
        private const string FileUrl = "https://raw.githubusercontent.com/DonH-ITS/jsonfiles/main/words.txt";
        private const string LocalFileName = "words.txt";

        //get words from file
        public async Task<string> GetWords()
        {
            var localFilePath = Path.Combine(FileSystem.AppDataDirectory, LocalFileName);

            //check if the file exists locally
            if (File.Exists(localFilePath))
            {
                return localFilePath;
            }

            // download file if no local source
            await DownloadFileAsync(localFilePath);
            return localFilePath;
        }

        //download file
        private async Task DownloadFileAsync(string localFilePath)
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    // download the file content
                    var fileContent = await httpClient.GetStringAsync(FileUrl);
                    Console.WriteLine($"File downloaded");

                    // write the content to a local file
                    await File.WriteAllTextAsync(localFilePath, fileContent);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error downloading file: {ex.Message}");
            }
        }

        // read words from the file
        public async Task<string[]> ReadWords()
        {
            var localFilePath = await GetWords();
            var fileContent = await File.ReadAllTextAsync(localFilePath);

            // split the content into words and return as an array for easier use
            return fileContent.Split(new[] { '\n', '\r', ',' }, StringSplitOptions.RemoveEmptyEntries);
        }
    }
}
