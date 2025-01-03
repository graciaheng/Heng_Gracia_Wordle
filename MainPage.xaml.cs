using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace wordleGame;

public partial class MainPage : ContentPage
{
    //LINK TO GITHUB https://github.com/graciaheng/Heng_Gracia_Wordle
	private readonly FileService _fileService;
	private WordleViewModel wordleViewModel;
	private int attempts = 0;
    private string guess = "";
    private bool isGridCreated = false;
    private bool isGameOver = false;
    private string[] feedback = new string[5];
     private bool gameStarted = false;
    private DateTime timeStarted;
    private int elapsedTimeInSeconds = 0;

	public MainPage(string playerName)
	{
		InitializeComponent();
		_fileService = new FileService();
		wordleViewModel = new WordleViewModel();
		this.BindingContext = wordleViewModel;
        CreateTheGrid();
        gameStarted = true;
        StartTimer();
	}

	protected override async void OnAppearing()
	{
		base.OnAppearing();

        try 
        {
			var words = await _fileService.ReadWords();
            if (words == null || words.Length == 0)
            {
				throw new Exception("No words available in the file.");
            }
			
            string chosenWord = words[new Random().Next(words.Length)];
            wordleViewModel.ChosenWord = chosenWord;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Failed to load words: {ex.Message}", "OK");
        }

	}

    private async Task<string> GetRandomWordAsync()
    {
        var words = await _fileService.ReadWords();
        if (words == null || words.Length == 0)
        {
            throw new Exception("No words available.");
        }

        return words[new Random().Next(words.Length)];
    }
    
    private async Task SetNewWordAsync()
    { 
        try 
        {
			var words = await _fileService.ReadWords();
            if (words == null || words.Length == 0)
            {
				throw new Exception("No words available in the file.");
            }
			
            string chosenWord = words[new Random().Next(words.Length)];
            wordleViewModel.ChosenWord = chosenWord;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Failed to load words: {ex.Message}", "OK");
        }
    }
	private void CreateTheGrid() 
	{
        if (isGridCreated == true)
        {
            return;
        }

        for (int i = 0; i < 6; ++i) 
		{
            GridPageContent.AddRowDefinition(new RowDefinition());
        }

		for (int i = 0; i < 5; ++i) 
		{
            GridPageContent.AddColumnDefinition(new ColumnDefinition());
        }

        GridPageContent.RowSpacing = 10;
        GridPageContent.ColumnSpacing = 10;

        for (int i = 0; i < 6; ++i) {
            for (int j = 0; j < 5; ++j) {
                
                Frame styledFrame = new Frame
                {
                    BackgroundColor = Colors.GhostWhite,
                    BorderColor = Colors.Grey,
                    CornerRadius = 10
                };

                Label styledLabel = new Label
                {
                    Text = "", // Initially no text
                    HorizontalTextAlignment = TextAlignment.Center,
                    VerticalTextAlignment = TextAlignment.Center,
                    FontSize = 24,
                    FontFamily = "Karnak Condensed",
                    FontAttributes = FontAttributes.Bold,
                    TextColor = Colors.White
                };

                styledFrame.Content = styledLabel;

				GridPageContent.Add(styledFrame, j, i);
            }
        }

        isGridCreated = true;
    }

    private void StartTimer()
	{
		Device.StartTimer(TimeSpan.FromSeconds(1), () =>
		{
			//timer only starts when the game starts
			if (gameStarted)
		    {
			    elapsedTimeInSeconds++;
				TimerLabel.Text = $"Time: {elapsedTimeInSeconds} seconds";
				return true;
			}

			else 
			{
				return false;
			}

		});
	}

    private async void showHistory(object sender, EventArgs args) 
	{
        await Navigation.PushAsync(new HistoryPage());
    }

	private async void submitGuess(object sender, EventArgs args) 
	{
		guess = $"{Letter1.Text}{Letter2.Text}{Letter3.Text}{Letter4.Text}{Letter5.Text}";

		//check if user has inputted correctly
        if (guess.Length != 5 || !guess.All(char.IsLetter))
        {
            DisplayAlert("Invalid Guess", "Please enter a valid 5-letter word.", "OK");
            return;
        }

		checkGuess();
        UpdateGrid();

        //correct word
        if (guess.Equals(wordleViewModel.ChosenWord, StringComparison.OrdinalIgnoreCase))
        {
            DisplayStatus.Text = "You Won!";
            gameStarted = false;
            isGameOver = true;
            await playAgain();
        }

        //wrong word
        else
        {
            attempts++;

            if (attempts == 6)
            {
                DisplayStatus.Text = $"Game Over! The word was {wordleViewModel.ChosenWord}";
                gameStarted = false;
                isGameOver = true;
                await playAgain();
            }
        }

        ClearEntryFields();
	}

    private void ClearEntryFields()
    {
        Letter1.Text = string.Empty;
        Letter2.Text = string.Empty;
        Letter3.Text = string.Empty;
        Letter4.Text = string.Empty;
        Letter5.Text = string.Empty;
    }

    private void checkGuess() 
    {
        Array.Fill(feedback, string.Empty);
        bool[] matched = new bool[5];

        for (int i = 0; i < 5; i++)
        {
            char guessedLetter = char.ToUpper(guess[i]);
            char correctLetter = char.ToUpper(wordleViewModel.ChosenWord[i]);

            if (guessedLetter == correctLetter)
            {
                feedback[i] = "Correct";
                matched[i] = true;
            }
        }

        for (int i = 0; i < 5; i++) 
        {
            if (feedback[i] == "Correct")
            continue; //skip if already matched

            char guessedLetter = char.ToUpper(guess[i]);

            for (int j = 0; j < 5; j++)
            {
                //only updates if this letter has not been matched yet
                if (!matched[j] && char.ToUpper(wordleViewModel.ChosenWord[j]) == guessedLetter)
                {
                    feedback[i] = "WrongPlace"; 
                    matched[j] = true;
                    break;
                }
            }

            if (feedback[i] == string.Empty)
            {
                feedback[i] = "Incorrect";  
            }
        }
    }

	private void UpdateGrid()
	{
		for (int i = 0; i < 5; i++) {
			 
			  // Get the correct Border from the grid
			int index = i + (attempts * 5);

            if (GridPageContent.Children[index] is Frame frame)
			{
                if (frame.Content is Label label)
                {
                    label.Text = guess[i].ToString().ToUpper();

				    switch (feedback[i])
			        {
				        case "Correct":
                        frame.BackgroundColor = Colors.YellowGreen;
                        break;
                
				        case "WrongPlace":
                        frame.BackgroundColor = Colors.Gold;
                        break;

                        case "Incorrect":
                        frame.BackgroundColor = Colors.Gray;
                        break;
                    }
                }
			}
			else
			{
				Console.WriteLine($"Unexpected type: {GridPageContent.Children[index].GetType()}");
			}
		}
	}

    private void EnableSubmitButton()
    {
        Letter1.IsEnabled = true;
        Letter2.IsEnabled = true;
        Letter3.IsEnabled = true;
        Letter4.IsEnabled = true;
        Letter5.IsEnabled = true;
        submitButton.IsEnabled = true; // enable the submit button after game over
    }

    private void DisableSubmitButton()
    {
        Letter1.IsEnabled = false;
        Letter2.IsEnabled = false;
        Letter3.IsEnabled = false;
        Letter4.IsEnabled = false;
        Letter5.IsEnabled = false;
        submitButton.IsEnabled = false; // Disable the submit button after game over
    }

    private async void RestartGame() 
    {
        attempts = 0;
        isGameOver = false;
        elapsedTimeInSeconds = 0;
        Array.Fill(feedback, string.Empty);
        guess = string.Empty;

        savePlayerAttempt(correctWord: wordleViewModel.ChosenWord, numOfGuesses: attempts);

        for (int i = 0; i < 6; i++) {
            for (int j = 0; j < 5; j++) {
            
                int index = i * 5 + j;

                if (GridPageContent.Children[index] is Frame frame)
                {
                    if (frame.Content is Label label)
                    {
                        //reset background color
                        frame.BackgroundColor = Colors.GhostWhite;
                        label.Text = string.Empty; 
                    }
                }
            }
        }
        EnableSubmitButton();
        DisplayStatus.Text = "";
        
        await SetNewWordAsync();  
    }

    private async Task playAgain()
    {
        bool playAgain = await DisplayAlert("Congratulations!", "Would you like to play another round?", "Yes", "No");

        if (playAgain)
        {
            RestartGame();
        }

        else
        {
            DisplayStatus.Text = "Thank you for playing!";
            await Task.Delay(1000);
            gameStarted = false;
            savePlayerAttempt(correctWord: wordleViewModel.ChosenWord, numOfGuesses: attempts);
            //await Navigation.PopAsync();
        }
    }


    private async Task savePlayerAttempt(string correctWord, int numOfGuesses)
    {
        var attempt = new PlayerAttempt(correctWord, numOfGuesses);
        wordleViewModel.PlayerHistory.Attempts.Add(attempt);
        await wordleViewModel.PlayerHistory.SaveHistoryAsync();
    }
}

