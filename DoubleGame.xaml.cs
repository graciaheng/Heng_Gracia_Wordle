using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace wordleGame;

public partial class DoubleGame : ContentPage
{
	private readonly FileService _fileService;
	private WordleViewModel wordleViewModel;
	private int attempts1 = 0;
	private int attempts2 = 0;
    private string guess1 = "";
	private string guess2 = "";
    private bool isGridCreated = false;
    private bool isGameOver1 = false;
	private bool isGameOver2 = false;
    private Player player1;
    private Player player2;
	private string playerName1;
	private string playerName2;
	private bool player2Turn = false;
    private string[] feedback1 = new string[5];
	private string[] feedback2 = new string[5];
    private bool gameStarted = false;
    private DateTime timeStarted;
    private int elapsedTimeInSeconds1 = 0;
    private int elapsedTimeInSeconds2 = 0;

	public DoubleGame(string playerName1)
	{
		InitializeComponent();
		_fileService = new FileService();
		wordleViewModel = new WordleViewModel();
		this.BindingContext = wordleViewModel;
		this.playerName1 = playerName1;
        CreateTheGrid();
		DisplayStatus.Text = $"{playerName1}'s Turn";
		GridName1.Text = $"{playerName1}'s Grid";
		GridName2.Text = "";
		Player2InputSection.IsVisible = true;
		DisplayAlert("Introduction", "Both players will have a different word. First player to guess their respective word wins!", "OK");
	}

	private void OnPlayer2NameSubmit(object sender, EventArgs e)
    {
        playerName2 = Player2NameEntry.Text?.Trim();

        if (string.IsNullOrEmpty(playerName2))
        {
            DisplayAlert("Error", "Please enter Player 2's name.", "OK");
            return;
        }

        // hide Player 2's input section once the name is submitted
        Player2InputSection.IsVisible = false;
		GridName2.Text = $"{playerName2}'s Grid";
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
			string chosenWord2 = words[new Random().Next(words.Length)];

            wordleViewModel.ChosenWord = chosenWord;
			wordleViewModel.ChosenWord2 = chosenWord2;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Failed to load words: {ex.Message}", "OK");
        }

	}

    private async Task LoadOrCreatePlayer(string playerName)
    {
        string directory = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        string playerFilePath = Path.Combine(directory, $"{playerName}.json");

        if (File.Exists(playerFilePath))
        {
            // Load the player data from the file
            string json = await File.ReadAllTextAsync(playerFilePath);
            player1 = JsonSerializer.Deserialize<Player>(json);
        }
        else
        {
            // Create a new player
            player1 = new Player(playerName);
            string json = JsonSerializer.Serialize(player1);
            await File.WriteAllTextAsync(playerFilePath, json);
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
			string chosenWord2 = words[new Random().Next(words.Length)];
			
            wordleViewModel.ChosenWord = chosenWord;
			wordleViewModel.ChosenWord2 = chosenWord2;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Failed to load words: {ex.Message}", "OK");
        }
    }

	private void CreateTheGrid() 
	{

        for (int i = 0; i < 6; ++i) 
		{
            GridPageContent1.AddRowDefinition(new RowDefinition());
			GridPageContent2.AddRowDefinition(new RowDefinition());
        }

		for (int i = 0; i < 5; ++i) 
		{
            GridPageContent1.AddColumnDefinition(new ColumnDefinition());
			GridPageContent2.AddColumnDefinition(new ColumnDefinition());
        }

        GridPageContent1.RowSpacing = 5;
        GridPageContent1.ColumnSpacing = 5;
		GridPageContent2.RowSpacing = 5;
        GridPageContent2.ColumnSpacing = 5;

        for (int i = 0; i < 6; ++i) 
		{
            for (int j = 0; j < 5; ++j) 
			{
                
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
                    FontSize = 15,
                    FontFamily = "Karnak Condensed",
                    FontAttributes = FontAttributes.Bold,
                    TextColor = Colors.White
                };

                styledFrame.Content = styledLabel;

				GridPageContent1.Add(styledFrame, j, i);
            }
        }

		for (int i = 0; i < 6; ++i) 
		{
            for (int j = 0; j < 5; ++j) 
			{
                
                Frame styledFrame2 = new Frame
                {
                    BackgroundColor = Colors.GhostWhite,
                    BorderColor = Colors.Grey,
                    CornerRadius = 10
                };

                Label styledLabel2 = new Label
                {
                    Text = "", // Initially no text
                    HorizontalTextAlignment = TextAlignment.Center,
                    VerticalTextAlignment = TextAlignment.Center,
                    FontSize = 15,
                    FontFamily = "Karnak Condensed",
                    FontAttributes = FontAttributes.Bold,
                    TextColor = Colors.White
                };

				styledFrame2.Content = styledLabel2;
				GridPageContent2.Add(styledFrame2, j, i);
            }
        }

		GridPageContent1.IsVisible = true;
        GridPageContent2.IsVisible = true;
    }

    //leads players to the history page
    private async void showHistory(object sender, EventArgs args) 
	{
        await Navigation.PushAsync(new HistoryPage());
    }

    private void StartTimer()
	{
		Device.StartTimer(TimeSpan.FromSeconds(1), () =>
		{
			//timer only starts when the game starts
			if (gameStarted)
		    {
                //player 1 turn
                if (!player2Turn)
                {
                    elapsedTimeInSeconds1++;
				    TimerLabel.Text = $"Time Taken: {elapsedTimeInSeconds1} seconds";
				    return true;
                }

                //player 2 turn
                else
                {
                    elapsedTimeInSeconds2++;
				    TimerLabel.Text = $"Time Taken: {elapsedTimeInSeconds2} seconds";
				    return true;
                }
			    
			}

			else 
			{
				return false;
			}

		});
	}

	private async void submitGuess(object sender, EventArgs args) 
	{
		//player 1 turn
		if (!player2Turn)
		{
			guess1 = $"{Letter1.Text}{Letter2.Text}{Letter3.Text}{Letter4.Text}{Letter5.Text}";

			//check if user has inputted correctly
            if (guess1.Length != 5 || !guess1.All(char.IsLetter))
            {
                DisplayAlert("Invalid Guess", "Please enter a valid 5-letter word. Please ensure the correct player is playing.", "OK");
                return;
            }

		    checkGuess();
            UpdateGrid();

            //correct word
            if (guess1.Equals(wordleViewModel.ChosenWord, StringComparison.OrdinalIgnoreCase))
            {
                DisplayStatus.Text = $"{playerName1} Won!";
                gameStarted = false;
                isGameOver1 = true;
            }

            //wrong word
            else
            {
                attempts1++;

                if (attempts1 == 6)
                {
                    DisplayStatus.Text = $"Game Over! The word was {wordleViewModel.ChosenWord}";
                    gameStarted = false;
                    isGameOver1 = true;
                }
            }

			player2Turn = true;
			DisplayStatus.Text = $"{playerName2}'s Turn";
			ClearEntryFields1();
		}

		//player 2 turn
		else if (player2Turn)
		{
			guess2 = $"{Letter6.Text}{Letter7.Text}{Letter8.Text}{Letter9.Text}{Letter10.Text}";

			//check if user has inputted correctly
            if (guess2.Length != 5 || !guess2.All(char.IsLetter))
            {
                DisplayAlert("Invalid Guess", "Please enter a valid 5-letter word. Please ensure the correct player is playing.", "OK");
                return;
            }

		    checkGuess();
            UpdateGrid();

            //correct word
            if (guess2.Equals(wordleViewModel.ChosenWord2, StringComparison.OrdinalIgnoreCase))
            {
                DisplayStatus.Text = $"{playerName2} Won!";
                gameStarted = false;
                isGameOver2 = true;
            }

            //wrong word
            else
            {
                attempts2++;

                if (attempts2 == 6)
                {
                    DisplayStatus.Text = $"Game Over! The word was {wordleViewModel.ChosenWord2}";
                    gameStarted = false;
                    isGameOver2 = true;
                }
            }
			player2Turn = false;
			DisplayStatus.Text = $"{playerName1}'s Turn";
			ClearEntryFields2();
		}

		IsGameOver();
	}

	private async void IsGameOver() 
	{
		if (isGameOver1 || isGameOver2)
		{
			string winnerMessage = isGameOver1 ? $"{playerName1} Won!" : $"{playerName2} Won!";
            //DisplayAlert("Game Over", winnerMessage, "OK");
			DisplayStatus.Text = winnerMessage;
			DisableSubmitButton();
		    await playAgain();
		}

		if (attempts1 == 6 && attempts2 == 6)
		{
			DisplayAlert("Game Over", "The words were {wordleViewModel.ChosenWord} & {wordleViewModel.ChosenWord2}", "OK");
			DisableSubmitButton();
		    await playAgain();
		}
	}

    private void ClearEntryFields1()
    {
        Letter1.Text = string.Empty;
        Letter2.Text = string.Empty;
        Letter3.Text = string.Empty;
        Letter4.Text = string.Empty;
        Letter5.Text = string.Empty;
    }

	private void ClearEntryFields2()
    {
        Letter6.Text = string.Empty;
        Letter7.Text = string.Empty;
        Letter8.Text = string.Empty;
        Letter9.Text = string.Empty;
        Letter10.Text = string.Empty;
    }

    private void checkGuess() 
    {
		//player 1 turn
		if (!player2Turn)
		{
			string[] currentFeedback = feedback1;
			// ensures that the correct feedback array is chosen based on the current player
			Array.Fill(currentFeedback, string.Empty);
			bool[] matched = new bool[5];
			// ensures that the correct guess string is chosen based on the current player
			string currentGuess = guess1;
			string correctWord = wordleViewModel.ChosenWord;

			for (int i = 0; i < 5; i++)
            {
                char guessedLetter = char.ToUpper(currentGuess[i]);
                char correctLetter = char.ToUpper(correctWord[i]);

                if (guessedLetter == correctLetter)
                {
                    currentFeedback[i] = "Correct";
                    matched[i] = true;
                }
            }

            for (int i = 0; i < 5; i++) 
            {
                if (currentFeedback[i] == "Correct")
                continue;

                char guessedLetter = char.ToUpper(currentGuess[i]);

                for (int j = 0; j < 5; j++)
                {
                    if (!matched[j] && char.ToUpper(wordleViewModel.ChosenWord[j]) == guessedLetter)
                    {
                        currentFeedback[i] = "WrongPlace"; 
                        matched[j] = true;
                        break;
                    }
                }

                if (currentFeedback[i] == string.Empty)
                {
                    currentFeedback[i] = "Incorrect";  
                }
            }
		}

		//player 2 turn
		else if (player2Turn)
		{
			string[] currentFeedback = feedback2;
			// ensures that the correct feedback array is chosen based on the current player
			Array.Fill(currentFeedback, string.Empty);
			bool[] matched = new bool[5];
			// ensures that the correct guess string is chosen based on the current player
			string currentGuess = guess2;
			string correctWord = wordleViewModel.ChosenWord2;

			for (int i = 0; i < 5; i++)
            {
                char guessedLetter = char.ToUpper(currentGuess[i]);
                char correctLetter = char.ToUpper(correctWord[i]);

                if (guessedLetter == correctLetter)
                {
                    currentFeedback[i] = "Correct";
                    matched[i] = true;
                }
            }

            for (int i = 0; i < 5; i++) 
            {
                if (currentFeedback[i] == "Correct")
                continue;

                char guessedLetter = char.ToUpper(currentGuess[i]);

                for (int j = 0; j < 5; j++)
                {
                    if (!matched[j] && char.ToUpper(wordleViewModel.ChosenWord2[j]) == guessedLetter)
                    {
                        currentFeedback[i] = "WrongPlace"; 
                        matched[j] = true;
                        break;
                    }
                }

                if (currentFeedback[i] == string.Empty)
                {
                    currentFeedback[i] = "Incorrect";  
                }
            }
		}
    }

	private void UpdateGrid()
	{
		//player 1 turn
		if (!player2Turn)
		{
			string[] currentFeedback = feedback1;
			int currentAttempts = attempts1;
            string currentGuess = guess1;
			// Update the relevant grid (Player 1 or Player 2)
            Grid currentGrid = GridPageContent1;

			for (int i = 0; i < 5; i++) 
		    {
			    // Get the correct Border from the grid
			    int index = i + (currentAttempts * 5);

                if (currentGrid.Children[index] is Frame frame)
			    {
                    if (frame.Content is Label label)
                    {
                        label.Text = currentGuess[i].ToString().ToUpper();

				        switch (currentFeedback[i])
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
		    }
		}
        //player 2 turn
		else if (player2Turn)
		{
			string[] currentFeedback = feedback2;
			int currentAttempts = attempts2;
            string currentGuess = guess2;
			// Update the relevant grid (Player 1 or Player 2)
            Grid currentGrid = GridPageContent2;

			for (int i = 0; i < 5; i++) 
		    {
			    // Get the correct Border from the grid
			    int index = i + (currentAttempts * 5);

                if (currentGrid.Children[index] is Frame frame)
			    {
                    if (frame.Content is Label label)
                    {
                        label.Text = currentGuess[i].ToString().ToUpper();

				        switch (currentFeedback[i])
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
		Letter6.IsEnabled = false;
        Letter7.IsEnabled = false;
        Letter8.IsEnabled = false;
        Letter9.IsEnabled = false;
        Letter10.IsEnabled = false;
        submitButton.IsEnabled = false; // Disable the submit button after game over
    }

    private async void RestartGame() 
    {
        attempts1 = 0;
		attempts2 = 0;
        isGameOver1 = false;
		isGameOver2 = false;
		player2Turn = false;
        elapsedTimeInSeconds1 = 0;
        elapsedTimeInSeconds2 = 0;
        guess1 = string.Empty;
		guess2 = string.Empty;
        gameStarted = true;
		Array.Fill(feedback1, string.Empty);
        Array.Fill(feedback2, string.Empty);

		DisplayStatus.Text = $"{playerName1}'s Turn";

        StartTimer();
        TimerLabel.Text = $"Time Taken: 0 seconds";

        savePlayerAttempt(correctWord: wordleViewModel.ChosenWord, numOfGuesses: attempts1);
		savePlayerAttempt(correctWord: wordleViewModel.ChosenWord2, numOfGuesses: attempts2);

		ResetGrid(GridPageContent1);
		ResetGrid(GridPageContent2);

        EnableSubmitButton();
        
        await SetNewWordAsync();  
    }

	private void ResetGrid(Grid grid)
    {
        for (int i = 0; i < 6; i++) 
        {
            for (int j = 0; j < 5; j++)
            {
                int index = i * 5 + j;

                // access the frame in the current grid
                if (grid.Children[index] is Frame frame)
                {
                    if (frame.Content is Label label)
                    {
						//reset background color & clear text
                        frame.BackgroundColor = Colors.GhostWhite; 
                        label.Text = string.Empty;
                    }
                }
            }
        }
    }

    private async Task playAgain()
    {
        string winnerMessage = isGameOver1 ? $"{playerName1} won in {elapsedTimeInSeconds1} seconds!" : $"{playerName2} won in {elapsedTimeInSeconds2} seconds!";
        bool playAgain = await DisplayAlert(winnerMessage, "Would you like to play another round?", "Yes", "No");

        if (playAgain)
        {
            RestartGame();
        }

        else
        {
            DisplayStatus.Text = $"Thank you for playing! The words were {wordleViewModel.ChosenWord} & {wordleViewModel.ChosenWord2}";
            await Task.Delay(1000);
            gameStarted = false;
            savePlayerAttempt(correctWord: wordleViewModel.ChosenWord, numOfGuesses: attempts1);
		    savePlayerAttempt(correctWord: wordleViewModel.ChosenWord2, numOfGuesses: attempts2);
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